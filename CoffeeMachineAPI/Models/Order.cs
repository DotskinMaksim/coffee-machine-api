using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public string Status { get; set; }
    
    public int SugarLevel { get; set; }

    public int Quantity { get; set; }
    
    [ForeignKey("CupSize")]
    public int CupSizeId { get; set; }
    public CupSize CupSize { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("Drink")]
    public int DrinkId { get; set; }
    public Drink Drink { get; set; }

    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; } = false;
    
}