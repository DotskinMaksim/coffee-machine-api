using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string Status { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }
    
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

}