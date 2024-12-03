using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Models;
using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.DTOs;
using CoffeeMachineAPI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoffeeMachineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrinksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageUploadService _imageUploadService;

        public DrinksController(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        // GET: api/drinks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DrinkReadDTO>>> GetDrinks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var drinks = await _context.Drinks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Маппинг из сущностей в DTO
            var drinkDTOs = drinks.Select(drink => new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            }).ToList();

            return Ok(drinkDTOs);
        }

        // GET: api/drinks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DrinkReadDTO>> GetDrink(int id)
        {
            var drink = await _context.Drinks.FindAsync(id);

            if (drink == null)
            {
                return NotFound();
            }

            var drinkDTO = new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            };

            return Ok(drinkDTO);
        }

        // POST: api/drinks
        [HttpPost]
        public async Task<ActionResult<DrinkReadDTO>> CreateDrink(DrinkCreateDTO model)
        {
            if (model == null)
            {
                return BadRequest("Drink data is null.");
            }

            string imageUrl;
            try
            {
                imageUrl = await _imageUploadService.UploadImageAsync(model.Image);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }

            var drink = new Drink
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = imageUrl
            };

            _context.Drinks.Add(drink);
            await _context.SaveChangesAsync();

            var drinkDTO = new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            };

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, drinkDTO);
        }

        // PUT: api/drinks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDrink(int id, DrinkUpdateDTO model)
        {
            var drink = await _context.Drinks.FindAsync(id);
            if (drink == null)
            {
                return NotFound();
            }

            // Обновляем поля
            drink.Name = model.Name;
            drink.Description = model.Description;

            _context.Entry(drink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DrinkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/drinks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrink(int id)
        {
            var drink = await _context.Drinks.FindAsync(id);

            if (drink == null)
            {
                return NotFound();
            }

            _context.Drinks.Remove(drink);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DrinkExists(int id)
        {
            return _context.Drinks.Any(d => d.Id == id);
        }
    }
}