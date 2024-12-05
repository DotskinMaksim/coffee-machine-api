using System.ComponentModel.DataAnnotations.Schema;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Data;

public class LoginLog
{
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; } // Идентификатор пользователя
    public User User { get; set; }

    public DateTime LoginTime { get; set; } =  DateTime.Now; // Время входа
    public string IpAddress { get; set; } // IP-адрес
    public bool IsSuccess { get; set; } // Успешность входа (успешно/неудачно)
    public string LoginMethod { get; set; } // Метод входа (например, "пароль", "OAuth" и т.д.)
    
}