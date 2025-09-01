using System.Net.Http.Headers;
using System.Text;
using Application.Models.Marzban;
using Application.Models.NodeManager;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Client.Marzban;

public class MarzbanApiClient(
    HttpClient client,
    ILogger<IMarzbanApiClient> logger,
    IInstanceService service,
    IHttpClientFactory factory) : IMarzbanApiClient
{
    public async Task<MarzbanLoginResponse> LoginAsync(MarzbanLoginRequest model)
    {
        var formData = new List<KeyValuePair<string, string?>>
        {
            new("username", model.Username),
            new("password", model.Password),
            new("grant_type", model.GrantType)
        };


        var requestContent = new FormUrlEncodedContent(formData);
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(model.Path ?? throw new ArgumentNullException(nameof(model.Path)));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Post, MarzbanUrlPath.AdminToken);

        requestMessage.Content = requestContent;

        var response = await httpClient.SendAsync(requestMessage);

        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Details: {responseContent}", null,
                response.StatusCode);
        }

        if (typeof(MarzbanLoginResponse) == typeof(object) || string.IsNullOrEmpty(responseContent))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<MarzbanLoginResponse>(responseContent);
    }

    public async Task<MarzbanNodeCreateResponse> AddNodeAsync(MarzbanNodeCreateRequest request)
    {
        logger.LogInformation("Adding node {NodeName} to {BaseUrl}", request.Name, request.Path);
        using var httpClient = new HttpClient();


        httpClient.BaseAddress = new Uri(request.Path ?? throw new ArgumentNullException(nameof(request.Path)));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Post, MarzbanUrlPath.AddNode);

        if (!string.IsNullOrEmpty(request.Token))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
        }

        var serializeObject = JsonConvert.SerializeObject(request);
        requestMessage.Content = new StringContent(serializeObject, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage);

        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Marzban AddNode failed with status code {StatusCode}. Details: {Details}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Details: {responseContent}", null,
                response.StatusCode);
        }

        return JsonConvert.DeserializeObject<MarzbanNodeCreateResponse>(responseContent);
    }

    public async Task<string> GetNodeSettingAsync(MarzbanNodeGetSettingRequest request)
    {
        logger.LogInformation("Getting node setting for node");

        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(request.Path ?? throw new ArgumentNullException(nameof(request.Path)));
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Get, MarzbanUrlPath.GetSetting);

        if (!string.IsNullOrEmpty(request.Token))
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

        var response = await httpClient.SendAsync(requestMessage);

        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Marzban GetNodeSetting failed with status code {StatusCode}. Details: {Details}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Details: {responseContent}", null,
                response.StatusCode);
        }

        var getSettingResponse = JsonConvert.DeserializeObject<MarzbanNodeGetSettingResponse>(responseContent);
        return getSettingResponse.Certificate;
    }

    public async Task<JObject> GetCoreConfigAsync(MarzbanUpdateCoreConfigRequest request)
    {
        logger.LogInformation("get core configs for node");

        using var httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(request.Path ?? throw new ArgumentNullException(nameof(request.Path)));

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Get, MarzbanUrlPath.GetCoreJson);

        if (!string.IsNullOrEmpty(request.Token))
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

        var response = await httpClient.SendAsync(requestMessage);

        string responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Marzban GetNodeSetting failed with status code {StatusCode}. Details: {Details}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Details: {responseContent}", null,
                response.StatusCode);
        }

        return JObject.Parse(responseContent);
    }

    public async Task UpdateCoreConfigAsync(MarzbanUpdateCoreConfigRequest request, JObject newConfig)
    {
        logger.LogInformation("Updating core config for {Path}", request.Path);
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(request.Path);

        var requestMessage = new HttpRequestMessage(HttpMethod.Put, MarzbanUrlPath.UpdateCoreJson);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
        requestMessage.Content =
            new StringContent(newConfig.ToString(Formatting.None), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Marzban UpdateCoreConfig failed: {Details}", responseContent);
            throw new HttpRequestException($"API request failed: {responseContent}", null, response.StatusCode);
        }
    }

    public async Task<Dictionary<string, List<Host>>> GetHostsAsync(MarzbanNodeGetSettingRequest request)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(request.Path);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);

        var response = await httpClient.GetAsync("/api/hosts");
        response.EnsureSuccessStatusCode();
    
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Dictionary<string, List<Host>>>(responseBody) ?? new Dictionary<string, List<Host>>();
    }

    public async Task ModifyHostsAsync(string path, string token, Dictionary<string, List<Host>> hostsByTag)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(path);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var jsonContent = new StringContent(
            JsonConvert.SerializeObject(hostsByTag), 
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PutAsync("/api/hosts", jsonContent);
    
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Marzban ModifyHosts failed. Status: {StatusCode}, Details: {Details}", response.StatusCode, errorContent);
            throw new HttpRequestException($"API request failed with status code {response.StatusCode}. Details: {errorContent}");
        }
    }

    public async Task? DeleteNodeAsync(string path, string token,long nodeId)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(path);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var url = MarzbanUrlPath.DeleteNode.Replace("{node-Id}",nodeId.ToString());
        HttpRequestMessage requestMessage = new(HttpMethod.Delete, url);
        var jsonContent = JsonConvert.SerializeObject(nodeId);
        requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)logger.LogInformation("container is resumed.");
        else
        {
            logger.LogError("NM API stop failed with status code {StatusCode}. Details",
                response.StatusCode);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}", null,
                response.StatusCode);
        }
    }
}