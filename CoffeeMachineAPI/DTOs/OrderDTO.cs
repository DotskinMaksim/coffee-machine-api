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

    public bool IsPaid { get; set; }
}
public class OrderWithItemsReadDTO
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Status { get; set; }
    public string UserEmail { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsPaid { get; set; }
    public List<OrderItemReadDTO> OrderItems { get; set; } = new List<OrderItemReadDTO>();
}
public class OrderItemReadDTO
{
    public string DrinkName { get; set; }
    public int Quantity { get; set; }
    public int SugarLevel { get; set; }
    public string CupSize { get; set; }
    public decimal ItemPrice { get; set; }
}

public class OrderCreateDTO
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public List<OrderItemCreateDTO> Items { get; set; }
}