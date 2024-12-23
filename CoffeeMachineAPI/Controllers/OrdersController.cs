using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.DTOs;
using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;  // Andmebaasi kontekst
    private readonly HttpClient _httpClient;  // HttpClient teenus, et suhelda väliste API-dega

    // Konstruktor, et vastu võtta kontekst ja HttpClient teenus
    public OrdersController(ApplicationDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    // GET: api/orders - Kõikide tellimuste kuvamine
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Drink)
            .Include(o => o.CupSize) 
            .ToListAsync();
        
        if (orders == null || !orders.Any())
        {
            return NotFound("No orders found.");  // Kui tellimusi pole, tagastame NotFound vastuse
        }

        var orderDtos = orders.Select(order => new OrderReadDTO
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            UserEmail = order.User.Email,
            DrinkName = order.Drink.Name,
            SugarLevel = order.SugarLevel,
            Quantity = order.Quantity,
            CupSize = order.CupSize.Code,
            TotalPrice = order.TotalPrice,
            IsPaid = order.IsPaid
        }).ToList();

        return Ok(orderDtos);  // Tagastame tellimused vastusena
    }

    // POST: api/orders - Uue tellimuse algatamine
    [HttpPost]
    public async Task<IActionResult> InitializeOrder(OrderCreateDTO orderDTO, bool isLoggedIn)
    {
        if (orderDTO == null)
        {
            return BadRequest("Invalid order data.");  // Kui tellimus on vale, tagastame BadRequest vastuse
        }

        User user;
        if (!isLoggedIn)
        {
            user = await _context.Users.FindAsync(SystemUserIds.GuestUser);

        }
        else
        {
            user = await _context.Users.FindAsync(orderDTO.UserId);
            if (user == null)
            {
                return NotFound("User not found.");  // Kui kasutajat ei leita, tagastame NotFound vastuse
            }
        }

       
        var drink = await _context.Drinks.FindAsync(orderDTO.DrinkId);

        decimal price = drink.Price;
        
        var cupSize = await _context.CupSizes.FindAsync(orderDTO.CupSizeId);
        if (isLoggedIn)
        {
            price *= ClientDiscount.Value;
        }
        
        price *= cupSize.Multiplier;
        price *= orderDTO.Quantity;
        
        var order = new Order
        {
            UserId = user.Id,
            Status = OrderStatuses.InPayment, 
            TotalPrice = price,
            SugarLevel = orderDTO.SugarLevel,
            Quantity = orderDTO.Quantity,
            CupSizeId = orderDTO.CupSizeId,
            DrinkId = orderDTO.DrinkId, 
        };

        _context.Orders.Add(order);  // Lisame tellimuse andmebaasi
        await _context.SaveChangesAsync();
       
        // Loome makse objekti
        var payment = new Payment
        {
            OrderId = order.Id,
            Subtotal = price,
            Status = PaymentStatuses.Pending,
        };
        payment.CalculateTotal();  // Arvutame kogusumma
        
        if (orderDTO.UseBalance && user.BonusBalance > 0)
        {
            decimal bonusToUse = user.BonusBalance >= price ? price : user.BonusBalance;
            payment.Total -= bonusToUse;
            payment.IsUsedBonus = true;
        }
        _context.Payments.Add(payment);  // Lisame makse andmebaasi
        await _context.SaveChangesAsync();  // Salvestame muudatused andmebaasi

        var paymentResponse = await GeneratePaymentLink(payment.Total);  // Loome makselingi
        if (paymentResponse == null)
        {
            return StatusCode(500, "Failed to generate payment link.");
        }

        // Tagastame loodud tellimuse ja makselingi
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new
        {
            OrderId = order.Id,
            TotalPrice = order.TotalPrice,
            PaymentLink = paymentResponse
        });
    }

    // POST: api/orders/payment/{orderId} - Makse staatuse värskendamine
    [HttpPost("confirm/{orderId}")]
    public async Task<IActionResult> ConfirmOrder(int orderId, [FromBody] PaymentResultDTO paymentResultDTO)
    {
        var order = await _context.Orders
            .Include(o => o.User)  // Загружаем связанную сущность User
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            return NotFound($"Order with ID {orderId} not found.");
        }
        
        var payment = _context.Payments.OrderByDescending(p => p.Date).FirstOrDefault();  // Viimane makse
        payment.Success = paymentResultDTO.Success;
        payment.Status = paymentResultDTO.PaymentStatus;

        if (paymentResultDTO.Success)
        {
            order.IsPaid = true;
            order.Status = OrderStatuses.Processing;
            if (payment.IsUsedBonus)
            {
                decimal bonusToUse = order.User.BonusBalance >= payment.Total ? payment.Total : order.User.BonusBalance;
                order.User.BonusBalance -= bonusToUse;
                if (order.User.BonusBalance < 0) order.User.BonusBalance = 0;
            }
            order.User.BonusBalance += order.TotalPrice * BonusRate.Value;
        }
        else
        {
            order.IsPaid = false;
            order.Status = OrderStatuses.PaymentError;
        }
        

        await _context.SaveChangesAsync();

        return Ok(new
        {
            OrderId = order.Id,
            OrderStatus = order.Status,
            PaymentStatus = payment.Status
        });
    }

    // GET: api/orders/{id} - Ühe tellimuse kuvamine ID järgi
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderReadDTO>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Drink)
            .Include(o => o.CupSize)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return new OrderReadDTO
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            UserEmail = order.User.Email,
            TotalPrice = order.TotalPrice,
            IsPaid = order.IsPaid,
            Quantity = order.Quantity,
            SugarLevel = order.SugarLevel,
            DrinkName = order.Drink.Name,
            CupSize = order.CupSize.Code,
        };
    }

    // DELETE: api/orders/{id} - Tellimuse kustutamine
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        _context.Orders.Remove(order);  // Kustutame tellimuse

        await _context.SaveChangesAsync();  // Salvestame muudatused andmebaasi

        return NoContent();  // Tagastame NoContent vastuse
    }

    // Aitame luua makselinki
    private async Task<string?> GeneratePaymentLink(decimal totalPrice)
    {
        var paymentData = new
        {
            api_username = "e36eb40f5ec87fa2",
            account_name = "EUR3D1",
            amount = totalPrice,
            order_reference = Guid.NewGuid().ToString("N"),
            nonce = Guid.NewGuid().ToString("N"),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            customer_url = "https://maksmine.web.app/makse"
        };

        var json = JsonSerializer.Serialize(paymentData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic", "ZTM2ZWI0MGY1ZWM4N2ZhMjo3YjkxYTNiOWUxYjc0NTI0YzJlOWZjMjgyZjhhYzhjZA==");

        var response = await _httpClient.PostAsync("https://igw-demo.every-pay.com/api/v4/payments/oneoff", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);

            if (jsonDoc.RootElement.TryGetProperty("payment_link", out var paymentLink))
            {
                return paymentLink.GetString();
            }
        }

        return null;
    }
}