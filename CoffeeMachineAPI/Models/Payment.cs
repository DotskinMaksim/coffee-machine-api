using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeMachineAPI.Data;

namespace CoffeeMachineAPI.Models;

public class Payment
{
    public int Id { get; set; }

    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get;  set; }
    public decimal VATAmount => Subtotal * VATRate.Value;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public bool Success { get; set; } = false;
    
    public string Status { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
    public bool IsUsedBonus { get; set; } = false;
    
    public void CalculateTotal()
    {
        this.Total = Subtotal + (Subtotal * VATRate.Value);
    }
}