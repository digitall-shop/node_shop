using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using Domain.Entities;
using Domain.Enumes.Node;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application.Services.Interfaces;

namespace Application.Services.Workers;

/// <summary>
/// Background hosted service that detects Nodes needing an agent installation (or re-install)
/// and provisions the NodeShop Agent remotely (SSH + Docker/Binary).
/// This is a skeleton with placeholders for real SSH / remote execution logic.
/// </summary>
public class NodeAgentProvisioningWorker : BackgroundService
{
    private readonly ILogger<NodeAgentProvisioningWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(30);

    public NodeAgentProvisioningWorker(ILogger<NodeAgentProvisioningWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NodeAgentProvisioningWorker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var nodeService = scope.ServiceProvider.GetRequiredService<INodeService>();
                var candidates = await nodeService.GetProvisioningCandidatesAsync();
                foreach (var node in candidates)
                {
                    await HandleNodeAsync(nodeService, node, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in provisioning loop");
            }
            await Task.Delay(ScanInterval, stoppingToken);
        }
    }

    private async Task HandleNodeAsync(INodeService nodeService, Node node, CancellationToken ct)
    {
        if (!node.IsEnabled) return;

        try
        {
            if (node.ProvisioningStatus == NodeProvisioningStatus.Pending)
            {
                node.ProvisioningStatus = NodeProvisioningStatus.Installing;
                node.ProvisioningMessage = "Starting installation";
                await nodeService.UpdateNodeAsync(node);
            }

            _logger.LogInformation("Provisioning agent on node {Id} via {Method}", node.Id, node.InstallMethod);

            // Generate enrollment token if missing or expired
            if (string.IsNullOrWhiteSpace(node.AgentEnrollmentToken) || node.AgentEnrollmentTokenExpiresUtc < DateTime.UtcNow)
            {
                node.AgentEnrollmentToken = GenerateToken();
                node.AgentEnrollmentTokenExpiresUtc = DateTime.UtcNow.AddMinutes(15);
                node.ProvisioningMessage = "Generated enrollment token";
                await nodeService.UpdateNodeAsync(node);
            }

            // Pseudo steps - replace with real SSH command execution
            bool success = await SimulateRemoteInstall(node, ct);

            if (success)
            {
                node.ProvisioningStatus = NodeProvisioningStatus.Ready;
                node.AgentVersion = node.TargetAgentVersion ?? "1.0.0"; // placeholder
                node.LastSeenUtc = DateTime.UtcNow;
                node.ProvisioningMessage = "Agent installed and heartbeat pending";
            }
            else
            {
                node.ProvisioningStatus = NodeProvisioningStatus.Failed;
                node.ProvisioningMessage = "Installation failed (simulation)";
            }

            await nodeService.UpdateNodeAsync(node);
        }
        catch (Exception ex)
        {
            node.ProvisioningStatus = NodeProvisioningStatus.Failed;
            node.ProvisioningMessage = "Exception: " + ex.Message;
            await nodeService.UpdateNodeAsync(node);
            _logger.LogError(ex, "Provisioning failed for node {Id}", node.Id);
        }
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private Task<bool> SimulateRemoteInstall(Node node, CancellationToken ct)
    {
        // NOTE: Replace this with real SSH (e.g., using Renci.SshNet) to:
        // 1. Check/install docker or place binary
        // 2. Upload env/config
        // 3. Start systemd service or docker compose
        // 4. Verify health endpoint
        return Task.FromResult(true);
    }
}

