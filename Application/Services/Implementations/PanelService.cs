using Application.Client.Marzban;
using Application.Extensiones;
using Application.Models.Marzban;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.Panel;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Application.Statics; // <-- فقط اینو اضافه کردیم

namespace Application.Services.Implementations;

public class PanelService(
    IAsyncRepository<Panel, long> repository,
    ILogger<IPanelService> logger,
    IUserContextService userContextService,
    IMarzbanApiClient client,
    IEncryptionService encryptionService,
    IMapper mapper)
    : IPanelService
{
    public async Task<PanelDto> GetPanelAsync(long panelId)
    {
        logger.PanelFetched(panelId, userContextService.UserId);

        var panel = await repository.GetSingleAsync(p => p.Id == panelId && p.UserId == userContextService.UserId)
                    ?? throw new NotFoundException("panel not found");

        return mapper.Map<PanelDto>(panel);
    }

    public async Task<List<PanelDto>> GetAllPanelsAsync()
    {
        logger.AdminPanelsListed();

        var panels = await repository.GetAllAsync();
        return mapper.Map<List<PanelDto>>(panels);
    }

    public async Task<List<PanelDto>> GetAllUserPanelsAsync()
    {
        logger.UserPanelsListed(userContextService.UserId);

        var panels = await repository
            .GetQuery()
            .Where(p => p.UserId == userContextService.UserId && p.IsDelete == false)
            .ToListAsync();

        return mapper.Map<List<PanelDto>>(panels);
    }

    public async Task<PanelOverviewDto> CreatePanelAsync(PanelCreateDto create)
    {
        var existing = await repository.GetSingleAsync(
            p => p.UserId == userContextService.UserId && p.Url == create.Url
        );
        if (existing != null) throw new ExistsException("panel already exists");

        var model = new MarzbanLoginRequest
        {
            Path = create.Url,
            Username = create.UserName,
            Password = create.Password,
            GrantType = "password",
        };

        var answer = await client.LoginAsync(model) ?? throw new NotFoundException("login error");
        if (answer.AccessToken == null) throw new UnauthorizedAccessException("login error");

        var allocatedPorts = await repository.AllocateUniquePortsAsync(logger);
        logger.PanelPortsAllocated(
            allocatedPorts.ElementAtOrDefault(0),
            allocatedPorts.ElementAtOrDefault(1),
            allocatedPorts.ElementAtOrDefault(2)
        );

        var panel = mapper.Map<Panel>(create);
        panel.UserId = userContextService.UserId;
        panel.Token = answer.AccessToken;
        panel.UserName = encryptionService.Encrypt(create.UserName);
        panel.Password = encryptionService.Encrypt(create.Password);

        if (allocatedPorts.Count >= 3)
        {
            panel.XrayPort = allocatedPorts[0];
            panel.ApiPort = allocatedPorts[1];
            panel.InboundPort = allocatedPorts[2];
        }
        else
        {
            throw new InvalidOperationException("Internal error: Failed to allocate sufficient unique ports.");
        }

        await repository.AddEntity(panel);
        await UpdatePanelCertificateKey(panel);

        var dup = await repository.GetSingleAsync(
            p => p.CertificateKey == panel.CertificateKey && p.Id != panel.Id && !p.IsDelete
        );
        if (dup != null)
            throw new ExistsException(
                $"A panel with this certificate key already exists. It is registered with domain: {dup.Url}",
                dup.Url
            );

        await repository.SaveChangesAsync();
        logger.PanelCreated(panel.Id, panel.UserId, panel.Url);

        await UpdateMarzbanInboundsAsync(panel.Id);
        logger.PanelInboundsUpdated(panel.Id, panel.InboundPort, false);

        return mapper.Map<PanelOverviewDto>(panel);
    }

    public async Task UpdatePanelCertificateKey(Panel panel)
    {
        try
        {
            var accessToken = panel.Token
                              ?? throw new InvalidOperationException(
                                  "Failed to get access token from Marzban for panel.");

            var req = new MarzbanNodeGetSettingRequest { Token = accessToken, Path = panel.Url };
            var secretKeyResponse = await client.GetNodeSettingAsync(req);

            panel.CertificateKey = secretKeyResponse;
            logger.PanelCertificateUpdated(panel.Id);
            return;
        }
        catch
        {
            var plainUser = encryptionService.Decrypt(panel.UserName);
            var plainPass = encryptionService.Decrypt(panel.Password);

            var login = new MarzbanLoginRequest
            {
                Path = panel.Url,
                Username = plainUser,
                Password = plainPass,
                GrantType = "password",
            };

            var answer = await client.LoginAsync(login) ?? throw new NotFoundException("login error");
            if (answer.AccessToken == null) throw new UnauthorizedAccessException("login error");

            panel.Token = answer.AccessToken;
            await repository.SaveChangesAsync();
            logger.PanelTokenRefreshed(panel.Id);

            await UpdatePanelCertificateKey(panel);
        }
    }

    public async Task DeletePanelAsync(long panelId)
    {
        var panel = await repository.GetSingleAsync(p => p.Id == panelId && p.UserId == userContextService.UserId)
                    ?? throw new NotFoundException("panel not found");

        panel.IsDelete = true;
        
        await repository.UpdateEntity(panel);
        await repository.SaveChangesAsync();

        logger.PanelDeleted(panelId, userContextService.UserId);
    }

    public async Task<PanelOverviewDto> UpdatePanelAsync(PanelUpdateDto update, long id)
    {
        var panel = await repository.GetSingleAsync(p =>
                        p.Id == id && p.UserId == userContextService.UserId && !p.IsDelete)
                    ?? throw new NotFoundException($"Panel with ID {id} not found or not accessible.");

        if (panel.CertificateKey != null && panel.CertificateKey != update.CertificateKey)
            throw new BadRequestException(
                "The panel you want to edit conflicts with the existing information. Please use the Add Panel section.");

        var dataChanged = false;

        if (update.Url != null && update.Url != panel.Url)
        {
            dataChanged = true;
            panel.Url = update.Url;
        }

        if (update.UserName != null && update.UserName != panel.UserName)
        {
            dataChanged = true;
            panel.UserName = update.UserName;
        }

        if (update.Password != null && update.Password != panel.Password)
        {
            dataChanged = true;
            panel.Password = update.Password;
        }

        if (dataChanged)
        {
            var loginReq = new MarzbanLoginRequest
            {
                Path = panel.Url,
                Username = panel.UserName,
                Password = update.Password ?? panel.Password,
                GrantType = "password",
            };

            var loginRes = await client.LoginAsync(loginReq) ??
                           throw new NotFoundException("Marzban login error during panel update.");
            if (loginRes.AccessToken == null)
                throw new UnauthorizedAccessException("Failed to get Marzban access token during panel update.");

            panel.Token = loginRes.AccessToken;
            await UpdatePanelCertificateKey(panel);
        }

        if (update.Name != null && update.Name != panel.Name)
            panel.Name = update.Name;

        await repository.UpdateEntity(panel);
        await repository.SaveChangesAsync();

        logger.PanelUpdated(panel.Id, userContextService.UserId);
        return mapper.Map<PanelOverviewDto>(panel);
    }

    public async Task UpdateMarzbanInboundsAsync(long panelId)
    {
        var panel = await repository.GetByIdAsync(panelId) ?? throw new NotFoundException("Panel not found");

        try
        {
            var baseReq = new MarzbanUpdateCoreConfigRequest { Path = panel.Url, Token = panel.Token };
            var current = await client.GetCoreConfigAsync(baseReq);

            var newInboundJson = @"
            {
                'listen': '0.0.0.0',
                'protocol': 'vless',
                'settings': {
                    'clients': [],
                    'decryption': 'none'
                },
                'streamSettings': {
                    'network': 'tcp',
                    'security': 'none',
                    'tcpSettings': {
                        'acceptProxyProtocol': false,
                        'header': {
                            'type': 'http',
                            'request': {
                                'version': '1.1',
                                'method': 'GET',
                                'path': ['/'],
                                'headers': { 'Host': ['www.google.com'] }
                            },
                            'response': {
                                'version': '1.1',
                                'status': '200',
                                'reason': 'OK',
                                'headers': {}
                            }
                        }
                    }
                },
                'sniffing': { 'enabled': false }
            }";

            var newInbound = JObject.Parse(newInboundJson);
            newInbound["tag"] = "NodeShop";
            newInbound["port"] = panel.InboundPort;
            newInbound["hosts"] = new JArray();

            if (current["inbounds"] is JArray inbounds)
                inbounds.Add(newInbound);

            await client.UpdateCoreConfigAsync(baseReq, current);

            logger.PanelInboundsUpdated(panel.Id, panel.InboundPort, retry: false);
        }
        catch
        {
            // Refresh token then retry
            var loginModel = new MarzbanLoginRequest
                { Path = panel.Url, Username = panel.UserName, Password = panel.Password };
            var loginRes = await client.LoginAsync(loginModel) ?? throw new NotFoundException("Login error on retry.");
            if (string.IsNullOrEmpty(loginRes.AccessToken))
                throw new UnauthorizedAccessException("Failed to refresh Marzban token.");

            var panelEntity = await repository.GetByIdAsync(panel.Id);
            if (panelEntity != null)
            {
                panelEntity.Token = loginRes.AccessToken;
                await repository.SaveChangesAsync();
            }

            var baseReq2 = new MarzbanUpdateCoreConfigRequest { Path = panel.Url, Token = loginRes.AccessToken };
            var current2 = await client.GetCoreConfigAsync(baseReq2);

            var newInboundJson2 = @"
            {
                'listen': '0.0.0.0', 'protocol': 'vless',
                'settings': { 'clients': [], 'decryption': 'none' },
                'streamSettings': { 'network': 'tcp', 'security': 'none', 'tcpSettings': { 'acceptProxyProtocol': false, 'header': { 'type': 'http' } } },
                'sniffing': { 'enabled': false }
            }";

            var newInbound2 = JObject.Parse(newInboundJson2);
            newInbound2["tag"] = "NodeShop";
            newInbound2["port"] = panel.InboundPort;
            newInbound2["hosts"] = new JArray();

            if (current2["inbounds"] is JArray inbounds2)
                inbounds2.Add(newInbound2);

            await client.UpdateCoreConfigAsync(baseReq2, current2);

            logger.PanelInboundsUpdated(panel.Id, panel.InboundPort, retry: true);
        }
    }
}