using Application.Models.NodeManager;
using Domain.DTOs.Instance;
using Domain.DTOs.Panel;
using Domain.Enumes.Container;

namespace Application.Services.Interfaces;

public interface IInstanceService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<InstanceDto> CreateInstanceAsync(InstanceCreateDto create);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<InstanceDto?> GetInstanceByIdAsync(long id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provisionedId"></param>
    /// <returns></returns>
    Task<InstanceDto?> GetInstanceByProvisionedIdAsync(string provisionedId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newStatus"></param>
    /// <returns></returns>
    Task<string> UpdateInstanceStatusAsync(long id, ContainerProvisionStatus newStatus);
    

    /// <summary>
    /// this for get a report from node manager
    /// </summary>
    Task ProcessUsageReportAsync(UsageReportDto report);

    /// <summary>
    /// this for Resume services after Credit balance 
    /// </summary>
    /// <returns></returns>
    Task ResumeStopedServices(long userId);
    
    /// <summary>
    /// this for get all instances to show for user
    /// </summary>
    /// <returns></returns>
    Task<List<InstanceDto>> GetAllUserInstancesAsync();
    
    /// <summary>
    /// this for get all instances 
    /// </summary>
    /// <returns></returns>
    Task<List<InstanceDto>> GetAllInstancesAsync();
    
    /// <summary>
    /// this for get all nodes that connect to a panel
    /// </summary>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task<List<InstanceDto>> GetNodesByPanelIdAsync(long panelId);

    /// <summary>
    /// this for stop container when balance less than limited balance 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task CheckAndSuspendInstancesAsync(long userId);

    /// <summary>
    /// this for manual pause 
    /// </summary>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    Task ManuallyPauseInstanceAsync(long instanceId);
    
    /// <summary>
    /// this for manual resume
    /// </summary>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    Task ManuallyResumeInstanceAsync(long instanceId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    Task<InstanceDto> CreatePreliminaryInstanceAsync(InstanceCreateDto instance);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="response"></param>
    /// <param name="panel"></param>
    /// <returns></returns>
    Task FinalizeInstanceAsync(long instanceId, ProvisionResponseDto response, PanelDto panel);
    
    /// <summary>
    /// this for delete an instance 
    /// </summary>
    /// <param name="instanceId"></param>
    /// <returns></returns>
    Task DeleteInstanceAsync(long instanceId);
}