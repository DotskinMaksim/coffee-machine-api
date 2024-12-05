using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.DTOs;

public class OrderReadDTO
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Now;

    public string Status { get; set; }

    public string UserEmail { get; set; }

    public decimal TotalPrice { get; set; }
    
    public string DrinkName { get; set; }    
    public int SugarLevel { get; set; }

    public int Quantity { get; set; }
    public bool IsPaid { get; set; }
    
    public char CupSize { get; set; }

}

public class OrderCreateDTO
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public int DrinkId { get; set; }

    [Required]
    [Range(0, 4, ErrorMessage = "SugarLevel must be between 0 and 4.")]
    public int SugarLevel { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Quantity must be between 1 and 5.")]
    public int Quantity { get; set; }

    [Required]
    public int CupSizeId { get; set; }
}