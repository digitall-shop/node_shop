using Application.Models.Marzban;
using Application.Models.NodeManager;
using Newtonsoft.Json.Linq;

namespace Application.Client.Marzban;

public interface IMarzbanApiClient
{
    /// <summary>
    /// this for get token from Marzban for doing any processes 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<MarzbanLoginResponse> LoginAsync(MarzbanLoginRequest request);

    /// <summary>
    /// this for add new node in Marzban Panel
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<MarzbanNodeCreateResponse> AddNodeAsync(MarzbanNodeCreateRequest request);

    /// <summary>
    /// this for get node setting for a user marzban panel
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<string> GetNodeSettingAsync(MarzbanNodeGetSettingRequest request);

    /// <summary>
    /// this for get core config 
    /// </summary>
    /// <returns></returns>
    Task<JObject> GetCoreConfigAsync(MarzbanUpdateCoreConfigRequest request);

    /// <summary>
    /// this for update core config
    /// </summary>
    /// <param name="request"></param>
    /// <param name="newConfig"></param>
    /// <returns></returns>
    Task UpdateCoreConfigAsync(MarzbanUpdateCoreConfigRequest request, JObject newConfig);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Dictionary<string, List<Host>>> GetHostsAsync(MarzbanNodeGetSettingRequest request);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="token"></param>
    /// <param name="hostsByTag"></param>
    /// <returns></returns>
    Task ModifyHostsAsync(string path, string token, Dictionary<string, List<Host>> hostsByTag);

    /// <summary>
    /// this for delete node from marzban 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="token"></param>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    Task DeleteNodeAsync(string path, string token, long nodeId);
}