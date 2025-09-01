using Domain.Contract;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Extensiones;

public static class PortCreationExtension
{
    private static readonly Random Random = new ();
    
    private const int MinAllocatablePort = 20000;
    private const int MaxAllocatablePort = 30000;
    private const int NumberOfPortsToAllocate = 3;
    
    private static readonly HashSet<int> ReservedSystemPorts = [22, 80, 443, 5283, 8443, 5000, 5001];
    
    public static async Task<List<int>> AllocateUniquePortsAsync(
        this IAsyncRepository<Panel,long> repository,
        ILogger logger) 
    {
        var occupiedPorts = new HashSet<int>(ReservedSystemPorts);
        
        var existingPanelPorts = await repository.GetQuery()
                                                 .Where(p => p.XrayPort.HasValue || p.ApiPort.HasValue || p.InboundPort.HasValue)
                                                 .Select(p => new { p.XrayPort, p.ApiPort, p.InboundPort })
                                                 .ToListAsync();

        foreach (var p in existingPanelPorts)
        {
            if (p.XrayPort.HasValue) occupiedPorts.Add(p.XrayPort.Value);
            if (p.ApiPort.HasValue) occupiedPorts.Add(p.ApiPort.Value);
            if (p.InboundPort.HasValue) occupiedPorts.Add(p.InboundPort.Value);
        }
        
        var allocatedPorts = new List<int>();
        var availablePorts = Enumerable.Range(MinAllocatablePort, MaxAllocatablePort - MinAllocatablePort + 1)
                                       .Where(p => !occupiedPorts.Contains(p))
                                       .ToList();

        if (availablePorts.Count < NumberOfPortsToAllocate)
        {
            logger.LogError("Not enough unique ports available for allocation. Needed {NeededPorts}, found {AvailablePorts}.", 
                            NumberOfPortsToAllocate, availablePorts.Count);
            throw new InvalidOperationException("Not enough unique ports available to allocate. Please contact support.");
        }

        for (var i = 0; i < NumberOfPortsToAllocate; i++)
        {
            var index = Random.Next(availablePorts.Count);
            allocatedPorts.Add(availablePorts[index]);
            availablePorts.RemoveAt(index);
        }
        allocatedPorts.Sort(); 

        logger.LogInformation("Allocated Ports: {Ports}", string.Join(", ", allocatedPorts));
        
        return allocatedPorts;
    }
}
