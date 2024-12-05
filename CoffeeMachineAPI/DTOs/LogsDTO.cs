namespace CoffeeMachineAPI.DTOs;

public class LoginLogReadDTO
{
    public string UserEmail { get; set; } // Идентификатор пользователя

    public DateTime LoginTime { get; set; } =  DateTime.Now; // Время входа
    public string IpAddress { get; set; } // IP-адрес
    
    public string Result  { get; set; } // Успешность входа (успешно/неудачно)
    public string LoginMethod { get; set; } // Метод входа (например, 
}
public class AuditLogReadDTO
{
    public string TableName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Action { get; set; } // Тип действия: Добавление, Удаление, Обновление
    public string EntityId { get; set; } // ID объекта
    public string OldValues { get; set; }
    public string NewValues { get; set; }
}