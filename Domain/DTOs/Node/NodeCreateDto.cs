using System.ComponentModel.DataAnnotations;
using Domain.Enumes.Node;

namespace Domain.DTOs.Node;

public class NodeCreateDto
{
    public string NodeName { get; set; } = null!;
    public string SshHost { get; set; } = null!;
    public int SshPort { get; set; }
    public string SshUsername { get; set; } = null!;
    public LoginMethod Method { get; set; }
    
    public string? SshKeyCertKey { get; set; }
    
    public string? SshPassword { get; set; }
    
    public decimal Price { get; set; }
    public NodeStatus Status { get; set; }
    public bool IsAvailableForShow { get; set; }
}