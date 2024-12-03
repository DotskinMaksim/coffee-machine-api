using System.ComponentModel.DataAnnotations;

namespace CoffeeMachineAPI.DTOs;
public class DrinkReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}
public class DrinkCreateDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    public IFormFile Image { get; set; }
}
public class DrinkUpdateDTO
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
}