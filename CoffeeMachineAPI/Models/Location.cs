using System.ComponentModel.DataAnnotations;

namespace CoffeeMachineAPI.Models;

public class Location
{
    public int Id { get; set; } 

    [StringLength(255)]
    public string Address { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string State { get; set; }

    [StringLength(100)]
    public string Country { get; set; }

    [StringLength(255)]
    public string AdditionalDetails { get; set; }
}
