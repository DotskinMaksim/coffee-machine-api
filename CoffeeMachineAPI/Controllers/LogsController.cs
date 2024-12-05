using CoffeeMachineAPI.DTOs;
using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace CoffeeMachineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LogsController(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoginLogs([FromQuery] string token, [FromQuery] string logs)
        {
            bool isAdmin = token == _configuration["Admin:token"];

            if (!isAdmin)
            {
                return Unauthorized("Access denied. Admins only.");
            }

            if (logs == "login")
            {
                var loginLogs = await _context.LoginLogs
                    .Include(log => log.User) // Загрузка пользователя
                    .ToListAsync();

                // Преобразуем LoginLog в LoginLogReadDTO
                var loginLogsDTO = loginLogs.Select(log => new LoginLogReadDTO
                {
                    UserEmail = log.User?.Email ?? "Unknown", // Проверка на null
                    LoginTime = log.LoginTime,
                    IpAddress = log.IpAddress,
                    Result = log.Result,
                    LoginMethod = log.LoginMethod
                }).ToList();

                return Ok(loginLogsDTO);
            }
            if (logs == "audit")
            {
                var auditLogs = await _context.AuditLogs.ToListAsync();
                
                // Преобразуем AuditLog в AuditLogReadDTO
                var auditLogsDTO = auditLogs.Select(log => new AuditLogReadDTO
                {
                    TableName = log.TableName,
                    Timestamp = log.Timestamp,
                    Action = log.Action,
                    EntityId = log.EntityId,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues
                }).ToList();

                return Ok(auditLogsDTO);
            }
            else
            {
                return BadRequest("Invalid logs parameter. Please specify 'login' or 'audit'.");
            }
        }
    }
}