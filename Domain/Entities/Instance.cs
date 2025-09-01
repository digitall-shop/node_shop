using Domain.Common; 
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enumes.Container; 

namespace Domain.Entities;

[Table("Instances", Schema = "Commerce")]

public class Instance : BaseEntity<long>
{
    public string? ContainerDockerId { get; set; }
    public string? ProvisionedInstanceId { get; set; } 
    public ContainerProvisionStatus Status { get; set; } = ContainerProvisionStatus.Pending; 
    public long MarzbanNodeId { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public int? XrayPort { get; set; }
    public int? ApiPort { get; set; }
    public int? InboundPort { get; set; }
    public long LastBilledUsage { get; set; }
    public DateTime LastBillingTimestamp { get; set; }= DateTime.UtcNow;
    
    //relations
    public long NodeId { get; set; }
    public Node Node { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public long PanelId { get; set; }
    public Panel Panel { get; set; } = null!; 
    
}


