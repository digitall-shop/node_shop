using Application.Client.Marzban;
using Application.DomainEvents.Events;
using Application.Models.Marzban;
using Application.Services.Interfaces;
using Domain.Contract;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Host = Application.Models.Marzban.Host;
using Application.Statics;
using Domain.Events.DomainEvents.Events; // ← اضافه

namespace Application.DomainEvents.Handlers;

public class MarzbanHandler(
    IPanelService panelService,
    INodeService nodeService,
    IMarzbanApiClient client,
    ILogger<MarzbanHandler> logger,
    IEncryptionService encryptionService,
    IAsyncRepository<Instance,long> instanceRepo,
    IAsyncRepository<Panel, long> panelRepo
) : INotificationHandler<InstanceProvisionedEvent>
{
    public async Task Handle(InstanceProvisionedEvent notification, CancellationToken cancellationToken)
    {
        var panel = await panelService.GetPanelAsync(notification.PanelId);
        var node  = await nodeService.GetNodeByIdAsync(notification.NodeId);

        string targetTag;
        bool inboundCreated = false;

        try
        {
            // --- ensure inbound exists & get tag
            var baseRequest   = new MarzbanUpdateCoreConfigRequest { Path = panel.Url, Token = panel.Token };
            var currentConfig = await client.GetCoreConfigAsync(baseRequest);
            var inbounds      = currentConfig["inbounds"] as JArray;
            var existingInbound = inbounds?.FirstOrDefault(i => i["port"]?.ToObject<int>() == panel.InboundPort);

            if (existingInbound != null)
            {
                targetTag = existingInbound["tag"]?.ToString() ?? "NodeShop";
                logger.PanelInboundEnsured(panel.Id, panel.InboundPort, targetTag, createdNew: false);
            }
            else
            {
                await panelService.UpdateMarzbanInboundsAsync(panel.Id);
                targetTag = "NodeShop";
                inboundCreated = true;
                logger.PanelInboundEnsured(panel.Id, panel.InboundPort, targetTag, createdNew: true);
            }

            // --- create node in marzban
            var createRequest = new MarzbanNodeCreateRequest
            {
                Path = panel.Url,
                Token = panel.Token,
                Port = panel.ApiPort,
                ApiPort = panel.XrayPort,
                Address = node.SshHost,
                UsageCoefficient = 1,
                AddAsNewHost = false,
                Name = node.NodeName
            };

            var marzbanNodeResponse = await client.AddNodeAsync(createRequest);
            logger.PanelNodeCreatedInMarzban(panel.Id, node.Id, node.NodeName, marzbanNodeResponse.Id);

            // save MarzbanNodeId on instance
            var instance = await instanceRepo.GetByIdAsync(notification.InstanceId);
            if (instance != null)
            {
                instance.MarzbanNodeId = marzbanNodeResponse.Id;
                await instanceRepo.UpdateEntity(instance);
                await instanceRepo.SaveChangesAsync();
            }

            // --- add host to inbound tag if not exists
            var hostsByTag = await client.GetHostsAsync(new MarzbanNodeGetSettingRequest
            { Path = panel.Url, Token = panel.Token });

            var nodeshopHosts = hostsByTag.TryGetValue(targetTag, out var currentHosts)
                ? currentHosts.ToList()
                : new List<Host>();

            if (nodeshopHosts.All(h => h.Address != node.SshHost))
            {
                var newHost = new Host
                {
                    Address = node.SshHost,
                    Remark = node.NodeName,
                    Security = "inbound_default",
                    Alpn = "",
                    Fingerprint = ""
                };
                nodeshopHosts.Add(newHost);

                var hostsToUpdate = new Dictionary<string, List<Host>> { [targetTag] = nodeshopHosts };
                await client.ModifyHostsAsync(panel.Url, panel.Token, hostsToUpdate);

                logger.PanelHostAssignedToInbound(panel.Id, targetTag, node.SshHost, node.NodeName);
            }
        }
        catch (Exception ex)
        {
            // مرحله اول خطا → تلاش برای رفرش توکن
            logger.PanelOperationError(panel.Id, "initial", ex);
            logger.PanelTokenRefreshStarted(panel.Id);

            // تلاش مجدد با توکن تازه
            // 1) پیدا/ایجاد inbound و tag
            var baseRequestRetry   = new MarzbanUpdateCoreConfigRequest { Path = panel.Url, Token = panel.Token };
            var currentConfigRetry = await client.GetCoreConfigAsync(baseRequestRetry);
            var inboundsRetry      = currentConfigRetry["inbounds"] as JArray;
            var existingInboundRetry = inboundsRetry?.FirstOrDefault(i => i["port"]?.ToObject<int>() == panel.InboundPort);

            if (existingInboundRetry != null)
            {
                targetTag = existingInboundRetry["tag"]?.ToString() ?? "NodeShop";
                logger.PanelInboundEnsured(panel.Id, panel.InboundPort, targetTag, createdNew: false);
            }
            else
            {
                await panelService.UpdateMarzbanInboundsAsync(panel.Id);
                targetTag = "NodeShop";
                inboundCreated = true;
                logger.PanelInboundEnsured(panel.Id, panel.InboundPort, targetTag, createdNew: true);
            }

            var plainUser = encryptionService.Decrypt(panel.UserName);
            var plainPass = encryptionService.Decrypt(panel.Password);
            var model = new MarzbanLoginRequest
            {
                Path = panel.Url,
                Username = plainUser,
                Password = plainPass,
                GrantType = "password",
            };

            var answer = await client.LoginAsync(model) ?? throw new NotFoundException("login error");
            if (string.IsNullOrWhiteSpace(answer.AccessToken))
                throw new UnauthorizedAccessException("login error");

            panel.Token = answer.AccessToken;
            await panelRepo.SaveChangesAsync();
            logger.PanelTokenRefreshSucceeded(panel.Id);

            // 3) دوباره ایجاد نود
            var createRequest2 = new MarzbanNodeCreateRequest
            {
                Path = panel.Url,
                Token = panel.Token,
                Port = panel.ApiPort,
                ApiPort = panel.XrayPort,
                Address = node.SshHost,
                UsageCoefficient = 1,
                AddAsNewHost = false,
                Name = node.NodeName
            };
            var marzbanNodeResponse2 = await client.AddNodeAsync(createRequest2);
            logger.PanelNodeCreatedInMarzban(panel.Id, node.Id, node.NodeName, marzbanNodeResponse2.Id);

            var instanceRetry = await instanceRepo.GetByIdAsync(notification.InstanceId);
            if (instanceRetry != null)
            {
                instanceRetry.MarzbanNodeId = marzbanNodeResponse2.Id;
                await instanceRepo.UpdateEntity(instanceRetry);
                await instanceRepo.SaveChangesAsync();
            }

            var hostsByTag2 = await client.GetHostsAsync(new MarzbanNodeGetSettingRequest
            { Path = panel.Url, Token = panel.Token });

            var nodeshopHosts2 = hostsByTag2.TryGetValue(targetTag, out var currentHosts2)
                ? currentHosts2.ToList()
                : new List<Host>();

            if (nodeshopHosts2.All(h => h.Address != node.SshHost))
            {
                var newHost2 = new Host
                {
                    Address = node.SshHost,
                    Remark = node.NodeName,
                    Security = "inbound_default",
                    Alpn = "",
                    Fingerprint = ""
                };
                nodeshopHosts2.Add(newHost2);

                var hostsToUpdate2 = new Dictionary<string, List<Host>> { [targetTag] = nodeshopHosts2 };
                await client.ModifyHostsAsync(panel.Url, panel.Token, hostsToUpdate2);

                logger.PanelHostAssignedToInbound(panel.Id, targetTag, node.SshHost, node.NodeName);
            }
        }
    }
}
