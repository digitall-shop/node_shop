using Application.Models.NodeManager;

namespace Application.Client.NodeManager;

public interface INodeManagerApiClient
{
    Task<ProvisionResponseDto> ProvisionContainerAsync(ProvisionRequestDto request);
    
     //Task<DeprovisionResponseDto> DeprovisionContainerAsync(DeprovisionRequestDto request);

     /// <summary>
     /// this for stop a container
     /// </summary>
     /// <returns></returns>
     Task PauseContainerAsync(ContainerActionRequestDto  request);
     
     /// <summary>
     /// this for rePause a container 
     /// </summary>
     /// <returns></returns>
     Task ResumeContainerAsync(ContainerActionRequestDto request);

     /// <summary>
     /// this for delete an instance from nodemanager 
     /// </summary>
     /// <param name="ssh"></param>
     /// <param name="instanceId"></param>
     /// <returns></returns>
     Task DeprovisionInstanceAsync(string? ssh,long instanceId);
}