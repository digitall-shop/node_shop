using System; // added for DateTime
using Domain.Enumes.Node;

namespace Domain.DTOs.Node;

public class NodeDto
{
    public long Id { get; set; }
    public string NodeName { get; set; }
    public string SshHost { get; set; }

    public int SshPort { get; set; }

    public string SshUsername { get; set; }

    public LoginMethod Method { get; set; }
    public int ServerPort { get; set; }

    public decimal Price { get; set; }
    public NodeStatus Status { get; set; }

    public bool IsAvailableForShow { get; set; } = true;
    public string? XrayContainerImage { get; set; }

    // Agent health fields (for admin display)
    public bool IsEnabled { get; set; }
    public NodeProvisioningStatus ProvisioningStatus { get; set; }
    public string? ProvisioningMessage { get; set; }
    public string? AgentVersion { get; set; }
    public string? TargetAgentVersion { get; set; }
    public DateTime? LastSeenUtc { get; set; }
    public AgentInstallMethod InstallMethod { get; set; }
    public string? MarzbanEndpoint { get; set; }
}