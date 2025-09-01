using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;
[Table("Logs", Schema = "System")]
public class Log :BaseEntity<long>
{
    public string Message { get; set; } = null!;
    public string? MessageTemplate { get; set; }
    public string Level { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; } 
}