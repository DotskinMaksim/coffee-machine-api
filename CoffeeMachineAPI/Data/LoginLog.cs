using System.ComponentModel.DataAnnotations.Schema;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.Data;

public class LoginLog
{
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; } 
    public User User { get; set; }

    public DateTime LoginTime { get; set; } =  DateTime.Now; 
    public string IpAddress { get; set; }
    public string Result { get; set; } 
    public string LoginMethod { get; set; } 
    
}