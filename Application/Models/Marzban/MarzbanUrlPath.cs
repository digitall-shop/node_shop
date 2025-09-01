namespace Application.Models.Marzban;

public class MarzbanUrlPath
{
    internal const string AdminToken = "/api/admin/token";
    
    internal const string AddNode = "/api/node";

    internal const string GetSetting = "/api/node/settings";

    internal const string GetCoreJson = "/api/core/config";
    
    internal const string UpdateCoreJson = "/api/core/config";
    
    internal const string GetHostSetting = "/api/hosts";
    
    internal const string UpdateHostSetting = "/api/hosts";
    
    internal const string DeleteNode = "/api/node/{node_Id}";

}