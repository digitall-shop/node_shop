using Domain.Enumes.Container;

namespace Domain.DTOs.Instance;

public class InstanceCreateDto
{
    public string? ContainerDockerId { get; set; } = null!;

    public string? ProvisionedInstanceId { get; set; } = null!;
    public long InstanceId { get; set; }
    public long NodeId { get; set; }
    public long UserId { get; set; }
    public long PanelId { get; set; }

    public int InboundPort { get; set; }

    public int XrayPort { get; set; }

    public int ApiPort { get; set; }
    public ContainerProvisionStatus Status { get; set; } = ContainerProvisionStatus.Running;
}