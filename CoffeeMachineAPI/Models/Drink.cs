using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class Drink
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public string Description { get; set; }
    public string ImageUrl { get; set; }


    
}