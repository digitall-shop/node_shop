namespace Domain.Enumes.Node;

public enum LoginMethod
{
    Ssh=0,
    
    Password=1,
}

public enum NodeStatus
{
    Active=0,
    Inactive=1,
    Inprogress=2,
}

// New provisioning lifecycle status for Node Agent
public enum NodeProvisioningStatus
{
    Pending = 0,
    Installing = 1,
    Ready = 2,
    Failed = 3
}

// Installation method for agent
public enum AgentInstallMethod
{
    Docker = 0,
    Binary = 1
}
