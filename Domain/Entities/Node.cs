using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enumes.Node;

namespace Domain.Entities;

[Table("Node", Schema = "Commerce")]
public class Node : BaseEntity<long>
{
    public string? NodeName { get; set; } 
    public int SshPort { get; set; } 
    public string? SshHost { get; set; } 
    public string? SshUsername { get; set; }
    public string? SshPassword { get; set; }
    public LoginMethod Method { get; set; } 

    public string SshKeyCertKey { get; set; } = string.Empty;
    public string? XrayContainerImage { get; set; } 
    public ICollection<Instance> Instances { get; set; } = new List<Instance>();
    public decimal Price { get; set; }
    public NodeStatus Status { get; set; } 
    public bool IsAvailableForShow { get; set; } 

    // --- New Agent / Provisioning Fields ---
    public bool IsEnabled { get; set; } = true; // drives auto-provisioning
    public NodeProvisioningStatus ProvisioningStatus { get; set; } = NodeProvisioningStatus.Pending;
    public string? ProvisioningMessage { get; set; }
    public string? AgentVersion { get; set; }
    public string? TargetAgentVersion { get; set; }
    public DateTime? LastSeenUtc { get; set; }
    public AgentInstallMethod InstallMethod { get; set; } = AgentInstallMethod.Docker;
    public string? MarzbanEndpoint { get; set; }
    public string? AgentEnrollmentToken { get; set; }
    public DateTime? AgentEnrollmentTokenExpiresUtc { get; set; }
}