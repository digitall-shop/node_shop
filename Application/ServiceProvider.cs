using System.Reflection;
using Application.Client.CurrencyRate;
using Application.Client.Marzban;
using Application.Client.NodeManager;
using Application.Client.Plisio;
using Application.Factory;
using Application.Services.Workers; // worker namespace
using Application.Mapping;
using Application.Services.Implementations;
using Application.Services.Implementations.Strategy;
using Application.Services.Interfaces;
using Application.Services.Interfaces.Strategy;
using Application.Infrastructure; // restore for IFileService
using Data.Repositories;
using Domain.Contract;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceProvider
{
    public static IServiceCollection ApplicationServiceProvider(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(UserProfiler).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped(typeof(INodeManagerApiClient), typeof(NodeManagerApiClient));
        services.AddScoped(typeof(IAsyncRepository<,>), typeof(BaseRepository<,>));
        services.AddScoped(typeof(IUserService), typeof(UserService));
        services.AddScoped(typeof(IPanelService), typeof(PanelService));
        services.AddScoped(typeof(INodeService), typeof(NodeService));
        services.AddScoped(typeof(IAuthService), typeof(AuthService));
        services.AddScoped(typeof(IInstanceService), typeof(InstanceService));
        services.AddScoped(typeof(INodeProvisioningService), typeof(NodeProvisioningService));
        services.AddScoped(typeof(IPaymentStrategy), typeof(CardToCardStrategy));
        services.AddScoped(typeof(IPaymentStrategy), typeof(PlisioPaymentStrategy));
        services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
        services.AddScoped(typeof(ITransactionService), typeof(TransactionService));
        services.AddScoped(typeof(IFileService), typeof(FileService));
        services.AddScoped(typeof(ICurrencyRateService), typeof(CurrencyRateService));
        services.AddScoped(typeof(IBankAccountService), typeof(BankAccountService));
        services.AddSingleton(typeof(IEncryptionService), typeof(EncryptionService));
        services.AddScoped(typeof(ISupportService),typeof (SupportService));
        services.AddScoped(typeof(IBroadcastService),typeof (BroadcastService));

        services.AddHostedService<NodeAgentProvisioningWorker>();

        services.AddHttpClient<IPlisioClient, PlisioClient>(http => { http.Timeout = TimeSpan.FromSeconds(20); });
        services.AddHttpClient<IMarzbanApiClient, MarzbanApiClient>();
        services.AddScoped<ISafirApiClient, SafirApiClient>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceProvider).Assembly));
        services.AddScoped(typeof(PaymentStrategyFactory));
        return services;
    }
}
