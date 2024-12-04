// Kasutame vajalikud teegid, et töötada API päringutega, andmebaasi ja teenustega
using Microsoft.AspNetCore.Mvc;
using CoffeeMachineAPI.Models;
using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.DTOs;
using CoffeeMachineAPI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoffeeMachineAPI.Controllers
{
    // Määrame, et see on API kontroller ja määrame tee, mille kaudu päringud tehakse
    [ApiController]
    [Route("api/[controller]")]
    public class DrinksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Andmebaasi kontekst
        private readonly IImageUploadService _imageUploadService;  // Teenus piltide üleslaadimiseks

        // Konstruktor, kus võtame vastu konteksti ja teenuse sõltuvused
        public DrinksController(ApplicationDbContext context, IImageUploadService imageUploadService)
        {
            _context = context;
            _imageUploadService = imageUploadService;
        }

        // GET: api/drinks - Kõikide jookide kuvamine, toetab lehe suuruse määramist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DrinkReadDTO>>> GetDrinks([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            IQueryable<Drink> query = _context.Drinks;

            // Kui määratakse leht ja lehe suurus, rakendame lehe kaudu lehtede jaotamise
            if (page.HasValue && pageSize.HasValue)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            var drinks = await query.ToListAsync();

            // Muudame joogid DTO-deks (Data Transfer Objects), et need oleksid API kaudu tagastatud
            var drinkDTOs = drinks.Select(drink => new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            }).ToList();

            // Tagastame kõik joogid vastusena
            return Ok(drinkDTOs);
        }

        // GET: api/drinks/{id} - Ühe joogi detailide kuvamine ID järgi
        [HttpGet("{id}")]
        public async Task<ActionResult<DrinkReadDTO>> GetDrink(int id)
        {
            var drink = await _context.Drinks.FindAsync(id);

            // Kui jook ei ole leitud, tagastame 404 Not Found vastuse
            if (drink == null)
            {
                return NotFound();
            }

            // Muudame jooki DTO-ks ja tagastame vastuse
            var drinkDTO = new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            };

            return Ok(drinkDTO);
        }

        // POST: api/drinks - Uue joogi lisamine
        [HttpPost]
        public async Task<ActionResult<DrinkReadDTO>> CreateDrink(DrinkCreateDTO model)
        {
            // Kui saadud mudel on tühi, tagastame BadRequest vastuse
            if (model == null)
            {
                return BadRequest("Drink data is null.");
            }

            string imageUrl;
            try
            {
                // Üritame üles laadida pildi ja salvestada pildi URL-i
                imageUrl = await _imageUploadService.UploadImageAsync(model.Image);
            }
            catch (Exception ex)
            {
                // Kui pildi üleslaadimine ebaõnnestub, tagastame vea
                return StatusCode(500, $"Image upload failed: {ex.Message}");
            }

            // Loome uue joogi ja lisame selle andmebaasi
            var drink = new Drink
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = imageUrl
            };

            _context.Drinks.Add(drink);
            await _context.SaveChangesAsync();

            // Loome DTO ja tagastame loodud joogi andmed koos loodud ID-ga
            var drinkDTO = new DrinkReadDTO
            {
                Id = drink.Id,
                Name = drink.Name,
                Description = drink.Description,
                ImageUrl = drink.ImageUrl
            };

            return CreatedAtAction(nameof(GetDrink), new { id = drink.Id }, drinkDTO);
        }

        // PUT: api/drinks/{id} - Olemasoleva joogi värskendamine
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDrink(int id, DrinkUpdateDTO model)
        {
            // Otsime joogitandmeid andmebaasist
            var drink = await _context.Drinks.FindAsync(id);
            if (drink == null)
            {
                return NotFound();  // Kui jook ei leitud, tagastame 404 Not Found
            }

            // Värskendame joogi nime ja kirjeldust
            drink.Name = model.Name;
            drink.Description = model.Description;

            _context.Entry(drink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();  // Salvestame muudatused andmebaasi
            }
            catch (DbUpdateConcurrencyException)
            {
                // Kui esineb konkurentsiviga, siis kontrollime, kas jook eksisteerib veel andmebaasis
                if (!DrinkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Tagastame No Content, kuna muudatus on tehtud
        }

        // DELETE: api/drinks/{id} - Joogi kustutamine ID järgi
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDrink(int id)
        {
            // Otsime joogitandmeid andmebaasist
            var drink = await _context.Drinks.FindAsync(id);

            if (drink == null)
            {
                return NotFound();  // Kui jook ei leitud, tagastame 404 Not Found
            }

            _context.Drinks.Remove(drink);  // Eemaldame joogi andmebaasist
            await _context.SaveChangesAsync();

            return NoContent();  // Tagastame No Content, kuna jook on kustutatud
        }

        // Abimeetod kontrollimaks, kas jook eksisteerib andmebaasis
        private bool DrinkExists(int id)
        {
            return _context.Drinks.Any(d => d.Id == id);
        }
    }
}