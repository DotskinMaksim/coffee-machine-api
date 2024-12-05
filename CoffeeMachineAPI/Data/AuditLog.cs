using CoffeeMachineAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoffeeMachineAPI.Data;

public class AuditLog
{
    public int Id { get; set; }
    public string TableName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Action { get; set; } // Тип действия: Добавление, Удаление, Обновление
    public string EntityId { get; set; } // ID объекта
    public string OldValues { get; set; }
    public string NewValues { get; set; }
}

public class AuditEntry
{
    private readonly EntityEntry _entry;

    public AuditEntry(EntityEntry entry)
    {
        _entry = entry;
        TemporaryProperties = new List<PropertyEntry>();
        OldValues = new Dictionary<string, object>();
        NewValues = new Dictionary<string, object>();
    }

    public string TableName { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } // Тип действия
    public string EntityId { get; set; } // ID объекта
    public Dictionary<string, object> OldValues { get; set; }
    public Dictionary<string, object> NewValues { get; set; }
    public List<PropertyEntry> TemporaryProperties { get; set; }

    public AuditLog ToAuditLog()
    {
        return new AuditLog
        {
            TableName = TableName,
            Timestamp = Timestamp,
            Action = Action,
            EntityId = EntityId,
            OldValues = JsonSerializer.Serialize(OldValues),
            NewValues = JsonSerializer.Serialize(NewValues)
        };
    }
}