using CoffeeMachineAPI.Data;

namespace CoffeeMachineAPI.Services;

public class LoginLogService
{
    private readonly ApplicationDbContext _context;

    public LoginLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogUserLoginAsync(int userId, string ipAddress, bool isSuccess, string loginMethod)
    {
        var userLogin = new LoginLog
        {
            UserId = userId,
            IpAddress = ipAddress,
            IsSuccess = isSuccess,
            LoginMethod = loginMethod
        };

        _context.LoginLogs.Add(userLogin);
        await _context.SaveChangesAsync();
    }
}