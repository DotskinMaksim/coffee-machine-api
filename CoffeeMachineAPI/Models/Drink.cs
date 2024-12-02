using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class Drink
{
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
    public double Price { get; set; }

    [Range(0, 4, ErrorMessage = "Sugar level must be between 0 and 4.")]
    public int SugarLevel { get; set; }

    [Range(1, 3, ErrorMessage = "Cannot order more than 3 drinks at a time.")]
    public int Quantity { get; set; }

    [ForeignKey("Cup")]
    public int CupId { get; set; }
    public CupSize CupSize { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }
    public Order Order { get; set; }
}