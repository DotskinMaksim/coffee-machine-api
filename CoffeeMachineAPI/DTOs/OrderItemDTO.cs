using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoffeeMachineAPI.Models;

namespace CoffeeMachineAPI.DTOs;

public class OrderItemCreateDTO
{
    [Required]
    public int DrinkId { get; set; }

    [Required]
    public int CupSizeId { get; set; }

    [Required]
    [Range(1, 10)]
    public int Quantity { get; set; }

    [Range(0, 5)]
    public int SugarLevel { get; set; }
}