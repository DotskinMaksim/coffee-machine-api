using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CoffeeMachineAPI.Models;

public class Payment
{
    public int Id { get; set; }

    private const decimal VATRate = 0.20m;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get;  set; }
    public decimal VATAmount => Subtotal * VATRate;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public bool Success { get; set; } = false;
    
    public string Status { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
    public void CalculateTotal()
    {
        this.Total = Subtotal + (Subtotal * VATRate);
    }
}