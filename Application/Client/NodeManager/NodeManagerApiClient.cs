using System.Net.Http.Headers;
using System.Text;
using Application.Models.NodeManager;
using Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Client.NodeManager;

public class NodeManagerApiClient(
    ILogger<INodeManagerApiClient> logger,
    INodeService service,
    IConfiguration configuration) : INodeManagerApiClient
{
    private readonly string _apiKey = configuration["ApiSecret:Key"] ?? 
                                     throw new ArgumentNullException($"ApiSecret:Key is not configured.");
    public async Task<ProvisionResponseDto> ProvisionContainerAsync(ProvisionRequestDto request)
    {
       
        var node = await service.GetNodeByIdAsync(request.NodeId);
        request.XrayContainerImage = "gozargah/marzban-node:latest";

        logger.LogInformation(
            "⏩ Sending provision request to Node-Manager for Customer {CustomerId}", request.CustomerId);

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        httpClient.BaseAddress = new Uri($"http://{node.SshHost}:5050");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Post, NodeManagerUrlPath.CreateContainer);

        var jsonContent = JsonConvert.SerializeObject(request);
        requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();


        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("NM API ProvisionContainer failed with status code {StatusCode}. Details: {Details}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}. Details: {responseContent}", null,
                response.StatusCode);
        }

        var provisionResponse = JsonConvert.DeserializeObject<ProvisionResponseDto>(responseContent);

        return provisionResponse!;
    }

    public async Task PauseContainerAsync(ContainerActionRequestDto request)
    {
        var node = await service.GetNodeByIdAsync(request.NodeId);
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        
        httpClient.BaseAddress = new Uri($"http://{node.SshHost}:5050");
        
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var url = NodeManagerUrlPath.StopContainer.Replace("{containerId}", request.ContainerId);
        
        HttpRequestMessage requestMessage = new(HttpMethod.Post, url);
        
        var jsonContent = JsonConvert.SerializeObject(request);
        requestMessage.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        var response = await httpClient.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode) logger.LogInformation("container is paused.");
        else
        {
            logger.LogError("NM API ProvisionContainer failed with status code {StatusCode}.",
                response.StatusCode);
            throw new HttpRequestException(
                $"API request failed with status code {response.StatusCode}.", null,
                response.StatusCode);
        }
    }

    public async Task ResumeContainerAsync(ContainerActionRequestDto request)
    {
        var node = await service.GetNodeByIdAsync(request.NodeId);
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

        httpClient.BaseAddress = new Uri($"http://{node.SshHost}:5050");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var url = NodeManagerUrlPath.ResumeContainer.Replace("{containerId}", request.ContainerId);
        HttpRequestMessage requestMessage = new(HttpMethod.Post, url);
        
        var jsonContent = JsonConvert.SerializeObject(request);
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

    public async Task DeprovisionInstanceAsync(string? sshHost, long instanceId)
    {
        using var httpClient = new HttpClient();
        
        httpClient.BaseAddress = new Uri($"http://{sshHost}:5050");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);

        
        var url = NodeManagerUrlPath.DeleteContainer.Replace("{containerId}",instanceId.ToString());
        HttpRequestMessage requestMessage = new(HttpMethod.Delete, url);
        var jsonContent = JsonConvert.SerializeObject(instanceId);
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