using Api;
using Application;
using Application.Builder;
using Application.Extensiones;
using Application.Logging.Options;
using Application.Options;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Telegram.Extensions;

try
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    Log.Information("Starting up NodeShop API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<TelegramTopicLoggerOptions>(
        builder.Configuration.GetSection("Loggings:TelegramTopics"));
    
    builder.Services.Configure<LowBalanceAlertOptions>(
        builder.Configuration.GetSection("LowBalanceAlert"));
    
    builder.Services.Configure<PlisioOptions>(builder.Configuration.GetSection("Plisio"));

    builder.Services.AddHttpClient<Application.Client.Plisio.IPlisioClient, Application.Client.Plisio.PlisioClient>(http =>
    {
        http.Timeout = TimeSpan.FromSeconds(20);
    });

    builder.Host.UseSerilog((context, services, loggerCfg) =>
    {
        loggerCfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console();

        var topicOptions = services.GetRequiredService<IOptions<TelegramTopicLoggerOptions>>().Value;

        if (!string.IsNullOrWhiteSpace(topicOptions.BotToken) && topicOptions.ChatId != 0)
        {
            loggerCfg.WriteTo.Logger(lc => lc
                .Filter.ByIncludingOnly(Matching.WithProperty(topicOptions.TopicPropertyName ?? "Topic"))
                .WriteToTelegramTopics(topicOptions)
            );
        }
        else
        {
            Log.Warning("Telegram Log Sink is not configured. BotToken/ChatId missing.");
        }
    });

    // ---- Services ----
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddOpenApiDocument(options =>
    {
        options.Title = "NodeShop API";
        options.Version = "v1";
        options.Description = "Simple and Secure API for NodeShop";

        options.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            Description = "Enter JWT token like: Bearer {your token}"
        });

        options.OperationProcessors.Add(
            new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
    });

    builder.Services.ApplicationServiceProvider(builder.Configuration);
    builder.Services.ApiServiceProvider(builder.Configuration, builder.Environment);

    builder.Services.AddAuthorization();
    builder.Services.AddTelegramBotHostedService(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseStaticFiles();
    var spaSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "EasyUi");
    var spaBuildPath = Path.Combine(spaSourcePath, "dist");

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<NodeShopContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Database migration completed.");
    }

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    app.MapOpenApi();
    app.MapScalarApiReference();
    await Ui.BuildReactAppAsync(spaSourcePath, builder.Configuration, logger);

    app.UseRouting();
    app.UseCors("ShopApiCors");
    app.UseAuthentication();
    app.UseBlockedUserGuard();
    app.UseAuthorization();
    app.MapControllers();

    var easyUiEndpoint = builder.Configuration.GetValue<string>("EasyUiEndPoint") ?? "/EasyUi";

    app.MapEasyUiSpa(easyUiEndpoint, spaSourcePath, spaBuildPath)
        .MigrateDatabase<NodeShopContext>((context, seeder) =>
        {
            /* seed initial data if needed */
        });

    app.MigrateDatabase<NodeShopContext>((context, seeder) => { });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}