using CoffeeMachineAPI.Data;

namespace CoffeeMachineAPI.Services;

public class LoginLogService
{
    private readonly ApplicationDbContext _context;

    public LoginLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogUserLoginAsync(int userId, string ipAddress, string result, string loginMethod)
    {
        var userLogin = new LoginLog
        {
            UserId = userId,
            IpAddress = ipAddress,
            Result = result,
            LoginMethod = loginMethod
        };

        _context.LoginLogs.Add(userLogin);
        await _context.SaveChangesAsync();
    }
}