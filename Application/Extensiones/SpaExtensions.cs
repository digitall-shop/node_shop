using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Extensiones;

public static class SpaExtensions
{
    /// <summary>
    /// Maps and configures a React SPA under the given path.
    /// </summary>
    /// <param name="app">WebApplication instance</param>
    /// <param name="dashboardEndpoint">Route prefix, e.g. "/dashboard"</param>
    /// <param name="spaSourcePath">Physical path to the React source (for dev proxy)</param>
    /// <param name="spaBuildPath">Physical path to the built files (for prod static files)</param>
    public static WebApplication MapEasyUiSpa(
        this WebApplication app,
        string dashboardEndpoint,
        string spaSourcePath,
        string spaBuildPath)
    {
        try
        {
            app.MapWhen(
                ctx => ctx.Request.Path.StartsWithSegments(dashboardEndpoint),
                spaApp =>
                {
                    // Strip the /dashboard prefix so files are looked up under "/"
                    spaApp.UsePathBase(dashboardEndpoint);

                    // In Production serve the built assets
                        spaApp.UseSpaStaticFiles();

                    // SPA middleware: proxy in dev, fallback in prod
                    spaApp.UseSpa(spa =>
                    {
                        spa.Options.SourcePath = spaSourcePath;
                        spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                        {
                            FileProvider = new PhysicalFileProvider(spaBuildPath),
                            RequestPath = string.Empty,
                            OnPrepareResponse = ctx =>
                                ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000"
                        };
                    });
                });
        }
        catch (Exception e)
        {
            Console.WriteLine("error occured for ui");
        }

        return app;
    }
}