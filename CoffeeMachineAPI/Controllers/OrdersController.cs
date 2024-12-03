using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using coffee_machine_api.Models;
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
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;


    public OrdersController(ApplicationDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;

    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderReadDTO>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.User)  
            .Include(o => o.OrderItems)  
            .ThenInclude(oi => oi.Drink) 
            .ToListAsync();

        if (orders == null || !orders.Any())
        {
            return NotFound("No orders found.");
        }

        var orderDtos = orders.Select(order => new OrderReadDTO
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            UserEmail = order.User.Email,
            TotalPrice = order.TotalPrice,
            IsPaid = order.IsPaid
        }).ToList();

        return Ok(orderDtos); 
    }
    [HttpGet("with-items")]
    public async Task<ActionResult<IEnumerable<OrderWithItemsReadDTO>>> GetAllOrdersWithItems()
    {
        var orders = await _context.Orders
            .Include(o => o.User)  // Загружаем данные о пользователе
            .ToListAsync();

        if (orders == null || !orders.Any())
        {
            return NotFound("No orders found.");
        }

        var orderDtos = new List<OrderWithItemsReadDTO>();

        foreach (var order in orders)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Drink)  
                .Include(oi => oi.CupSize)  
                .Where(oi => oi.OrderId == order.Id)  
                .ToListAsync();

            if (orderItems == null || !orderItems.Any())
            {
                return NotFound($"No items found for Order ID {order.Id}");
            }

            var orderDto = new OrderWithItemsReadDTO
            {
                Id = order.Id,
                Date = order.Date,
                Status = order.Status,
                UserEmail = order.User?.Email,  
                TotalPrice = order.TotalPrice,
                IsPaid = order.IsPaid,
                OrderItems = orderItems.Select(oi => new OrderItemReadDTO
                {
                    DrinkName = oi.Drink?.Name, 
                    Quantity = oi.Quantity,
                    SugarLevel = oi.SugarLevel,
                    CupSize = oi.CupSize?.Name,  
                    ItemPrice = (oi.Drink?.Price ?? 0) * oi.Quantity * (oi.CupSize?.Multiplier ?? 1)  
                }).ToList()
            };

            orderDtos.Add(orderDto);
        }

        return Ok(orderDtos);
    }


    [HttpPost]
    public async Task<IActionResult> InitializeOrder(OrderCreateDTO orderDTO)
        {
        if (orderDTO == null || !orderDTO.Items.Any())
        {
            return BadRequest("Invalid order data.");
        }

        var user = await _context.Users.FindAsync(orderDTO.UserId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var order = new Order
        {
            UserId = orderDTO.UserId,
            Status = OrderStatuses.InPayment, 
            IsPaid = false, 
            TotalPrice = 0.00m, 
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        decimal totalPrice = 0;

        foreach (var itemDTO in orderDTO.Items)
        {
            var drink = await _context.Drinks.FindAsync(itemDTO.DrinkId);
            if (drink == null)
            {
                return BadRequest($"Drink with ID {itemDTO.DrinkId} not found.");
            }

            var cupSize = await _context.CupSizes.FindAsync(itemDTO.CupSizeId);
            if (cupSize == null)
            {
                return BadRequest($"Cup size with ID {itemDTO.CupSizeId} not found.");
            }

            var itemPrice = drink.Price * itemDTO.Quantity * cupSize.Multiplier;
            totalPrice += itemPrice; 

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                DrinkId = itemDTO.DrinkId,
                CupSizeId = itemDTO.CupSizeId,
                Quantity = itemDTO.Quantity,
                SugarLevel = itemDTO.SugarLevel
            };

            _context.OrderItems.Add(orderItem);
        }

        order.TotalPrice = totalPrice;
        _context.Orders.Update(order); 

        var payment = new Payment
        {
            OrderId = order.Id,
            Subtotal = totalPrice, 
            Status = PaymentStatuses.Pending,
        };
        payment.CalculateTotal();

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(); 

        var paymentResponse = await GeneratePaymentLink(payment.Total);
        if (paymentResponse == null)
        {
            return StatusCode(500, "Failed to generate payment link.");
        }

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new
        {
            OrderId = order.Id,
            TotalPrice = order.TotalPrice, 
            PaymentLink = paymentResponse
        });
    }
    [HttpPost("payment/{orderId}")]
    public async Task<IActionResult> UpdatePaymentStatus(int orderId, [FromBody] PaymentResultDTO paymentResultDTO)
    {
       
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return NotFound($"Order with ID {orderId} not found.");
        }
        
        var payment = _context.Payments.OrderByDescending(p => p.Date).FirstOrDefault();
        payment.Success = paymentResultDTO.Success;
        payment.Status = paymentResultDTO.Status;
        
        if (paymentResultDTO.Success)
        {
            order.IsPaid = true;
            order.Status = OrderStatuses.Processing;
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

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderReadDTO>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Drink)
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
            IsPaid = order.IsPaid
        };
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
        
        if (order == null)
        {
            return NotFound($"Order with ID {id} not found.");
        }

        _context.OrderItems.RemoveRange(order.OrderItems);

        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return NoContent(); 
    }
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