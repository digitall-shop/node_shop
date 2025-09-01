using Application.Models;
using Domain.DTOs.Instance;
using Domain.DTOs.Panel;

namespace Application.Services.Interfaces;

public interface INodeProvisioningService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeId"></param>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task<InstanceDto> ProvisionAsync(long nodeId,long panelId);
    

}