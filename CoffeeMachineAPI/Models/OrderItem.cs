using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Range(0, 4, ErrorMessage = "Sugar level must be between 0 and 4.")]
    public int SugarLevel { get; set; }

    [Range(1, 3, ErrorMessage = "Cannot order more than 3 drinks at a time.")]
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