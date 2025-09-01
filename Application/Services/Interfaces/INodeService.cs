using Domain.DTOs.Node;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface INodeService
{
    /// <summary>
    /// this for get a node by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<NodeDto> GetNodeByIdAsync(long id);
    /// <summary>
    /// this for get all nodes 
    /// </summary>
    /// <returns></returns>
    Task<List<NodeDto>> GetNodesAsync();
    /// <summary>
    /// this for create new node server
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<NodeDto> CreateNodeAsync(NodeCreateDto create);
    /// <summary>
    /// this is
    /// </summary>
    /// <param name="panelId"></param>
    /// <returns></returns>
    Task<List<NodeDto>> GetNodesForPanelAsync(long panelId);
    /// <summary>
    /// Deletes a node and all related Marzban nodes and instances
    /// </summary>
    /// <param name="nodeId"></param>
    /// <returns></returns>
    Task DeleteNodeAsync(long nodeId);

    // internal helper to fetch raw entity for provisioning worker
    Task<List<Node>> GetProvisioningCandidatesAsync();
    Task UpdateNodeAsync(Node node);
    Task<List<NodeDto>> GetAllAdminNodesAsync();
}