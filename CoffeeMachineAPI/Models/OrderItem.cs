using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class OrderItem
{
    public int Id { get; set; }

    public int SugarLevel { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("Drink")]
    public int DrinkId { get; set; }
    public Drink Drink { get; set; }

    [ForeignKey("CupSize")]
    public int CupSizeId { get; set; }
    public CupSize CupSize { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}