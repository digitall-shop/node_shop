using Domain.Enumes.Container;

namespace Domain.DTOs.Instance;

public class InstanceDto
{
    public long Id { get; set; }
    
    public string ContainerDockerId { get; set; } = null!;
    
    public string ProvisionedInstanceId { get; set; } = null!;
    
    public ContainerProvisionStatus Status { get; set; }
    
    public DateTime CreateDate { get; set; }
    public string? NodeName { get; set; }
    public string? Username { get; set; }
    public string? PanelName { get; set; }
    
    public long MarzbanNodeId { get; set; }

}