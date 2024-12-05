using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class CupSize
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public char Code { get; set; }
    public int VolumeInMl { get; set; }
    
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Multiplier { get; set; }

}