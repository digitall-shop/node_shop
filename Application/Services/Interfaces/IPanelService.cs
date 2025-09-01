using Domain.DTOs.Panel;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IPanelService
{
    /// <summary>
    /// this for get a panel by id
    /// </summary>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task<PanelDto> GetPanelAsync(long panelId);
    
    /// <summary>
    /// this for get all panels for admin
    /// </summary>
    /// <returns></returns>
    Task<List<PanelDto>> GetAllPanelsAsync();
    
    /// <summary>
    /// this for get all user panels
    /// </summary>
    /// <returns></returns>
    Task<List<PanelDto>> GetAllUserPanelsAsync();
    
    /// <summary>
    /// this for create a new panel
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    
    Task<PanelOverviewDto> CreatePanelAsync(PanelCreateDto create);

    /// <summary>
    /// this for update a panel and get certificate kay
    /// </summary>
    /// <param name="panel"></param>
    /// <returns></returns>
    Task UpdatePanelCertificateKey(Panel panel);
    
    /// <summary>
    /// this for delete a panel 
    /// </summary>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task DeletePanelAsync(long panelId);
    
    /// <summary>
    /// this for an update panel 
    /// </summary>
    /// <param name="panel"></param>
    /// <returns></returns>
    Task<PanelOverviewDto> UpdatePanelAsync(PanelUpdateDto panel, long id);
    
    /// <summary>
    /// this for update marzban inbound when a client changes it
    /// </summary>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task UpdateMarzbanInboundsAsync(long panelId);
}