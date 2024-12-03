using System.ComponentModel.DataAnnotations;

namespace CoffeeMachineAPI.Models;

public class Location
{
    public int Id { get; set; } 

    public string Address { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string AdditionalDetails { get; set; }
}
