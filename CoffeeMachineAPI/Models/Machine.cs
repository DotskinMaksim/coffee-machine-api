using System.ComponentModel.DataAnnotations.Schema;
using Google.Protobuf.Reflection;

namespace CoffeeMachineAPI.Models;

public class Machine
{
    public int Id { get; set; }

    [ForeignKey("Location")]
    public int LocationId { get; set; }
    public SourceCodeInfo.Types.Location Location { get; set; }

    public string Status { get; set; }

    public DateTime LastServiceDate { get; set; }
    public DateTime LastUsedDate { get; set; }
    public DateTime InstallationDate { get; set; }
}