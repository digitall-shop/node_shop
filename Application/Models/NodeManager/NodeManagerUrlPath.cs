namespace Application.Models.NodeManager;

public class NodeManagerUrlPath
{
    internal const string CreateContainer = "/api/provisioning/container";

    internal const string DeleteContainer = "/api/provisioning/container/{containerId}";
    
    internal const string StopContainer = "/api/provisioning/container/{containerId}/pause";
    
    internal const string ResumeContainer= "/api/provisioning/container/{containerId}/unpause";
}