using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeMachineAPI.Models;

public class CupStock
{
    public int Id { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Amount must be a non-negative number.")]
    public int Amount { get; set; }

    [ForeignKey("CupSize")]
    public int CupSizeId { get; set; }
    public CupSize CupSize { get; set; }

    [ForeignKey("Machine")]
    public int MachineId { get; set; }
    public Machine Machine { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.Now; 
}