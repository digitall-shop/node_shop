using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Handler;


namespace Telegram.Extensions;
public static class TelegramBotServiceCollectionExtensions
{
    /// <summary>
    /// Adds the Telegram Bot hosted service and its dependencies to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    public static IServiceCollection AddTelegramBotHostedService(this IServiceCollection services, IConfiguration configuration)
    {
        // Retrieve the bot token from configuration
        var botToken = configuration["TelegramBot:Token"] ??
                       throw new ArgumentNullException("TelegramBot:Token is not configured in appsettings.json or environment variables.");

        // Register TelegramBotClient as a Singleton
        services.AddSingleton<TelegramBotClient>(provider => new TelegramBotClient(botToken));

        // Register TelegramBotService as a Hosted Service (Singleton)
        // This service will run in the background of your API application.
        services.AddHostedService<TelegramBotService>();

        return services;
    }
}