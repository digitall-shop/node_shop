using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

[Table("Panels", Schema = "Commerce")]

public class Panel : BaseEntity<long>
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    public string Url { get; set; }
    public long UserId { get; set; }
    public int? XrayPort { get; set; }
    public int? ApiPort { get; set; }
    public int? InboundPort { get; set; }
    public string? CertificateKey { get; set; }
    public bool SSL { get; set; }
    
    //Relations by other entities
    public ICollection<Instance> Instances { get; set; }
    public User User { get; set; }
    public string Token { get; set; }
}