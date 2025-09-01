using Domain.Enumes.Node;

namespace Domain.DTOs.Node;

public class NodeUpdateDto
{
    public string? NodeName { get; set; }
    
    public string? SshHost { get; set; }
    
    public int? SshPort { get; set; } 
    
    public string? SshUsername { get; set; }

    public LoginMethod? Method { get; set; }
    
    public string? SshPassword { get; set; }
    
    public decimal? Price { get; set; } 

    public NodeStatus? Status { get; set; } 

    public bool? IsAvailableForShow { get; set; }
}