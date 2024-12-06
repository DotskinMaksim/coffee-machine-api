using CoffeeMachineAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachineAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;  // Andmebaasi kontekst

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet("guest-id")]
    public ActionResult GetGuestUserId()
    {
        return Ok(new {Id = SystemUserIds.GuestUser});
    }
    [HttpGet("unknown-id")]
    public ActionResult GetUnknownUserId()
    {
        return Ok(new {Id = SystemUserIds.UnknownUser});
    }
    
    [HttpGet("balance/{userId}")]
    public ActionResult GetUserBalance(int userId)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == userId);
    
        if (user == null)
        {
            return NotFound(new { Message = "User not found" });
        }
    
        return Ok(new { Balance = user.BonusBalance });
    }
}