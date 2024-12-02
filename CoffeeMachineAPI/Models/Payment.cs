using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CoffeeMachineAPI.Models;

public class Payment
{
    public int Id { get; set; }

    private const decimal VATRate = 0.20m;
    public decimal Subtotal { get; private set; }
    public decimal VATAmount => Subtotal * VATRate;
    public decimal Total => Subtotal + VATAmount;

    public DateTime Date { get; set; } = DateTime.Now;

    [StringLength(50)]
    public string Result { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}