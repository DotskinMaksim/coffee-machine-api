namespace CoffeeMachineAPI.DTOs;

public class RegisterUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    
}

public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}