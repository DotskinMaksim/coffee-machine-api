using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public bool Status { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Total Price must be a non-negative number.")]
    public decimal TotalPrice { get; set; }

    public bool IsPaid { get; set; }
}