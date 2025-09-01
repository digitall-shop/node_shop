using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Builder;

public class Ui
{
    public static async Task BuildReactAppAsync(string clientAppPath, IConfiguration configuration, ILogger logger,
        string installArgs = "")
    {
        var buildSettings = configuration.GetSection("Build_Settings");
        var basePath = buildSettings["BasePath"] ?? "/EasyUi/";
        var outDir = buildSettings["OutDir"] ?? "dist";
        var assetsDir = buildSettings["AssetsDir"] ?? "statics";

        var viteBasePath = Environment.GetEnvironmentVariable("VITE_BASE_PATH");

        var envFilePath = Path.Combine(clientAppPath, ".env.production");
        var envDevFilePath = Path.Combine(clientAppPath, ".env.development");

        await File.WriteAllTextAsync(envFilePath, $"VITE_API_BASE_URL={viteBasePath}\n");
        await File.WriteAllTextAsync(envDevFilePath, $"VITE_API_BASE_URL={viteBasePath}\n");

        var envVars = new Dictionary<string, string>
        {
            { "VITE_API_BASE_URL", viteBasePath },
            { "NODE_ENV", "development" },
        };

        logger.LogInformation("📦 Building React app with base path: {BasePath}", basePath);

        string buildArgs = $"-- --outDir {outDir} --assetsDir {assetsDir}";

        await RunCommandAsync("npm", $"install {installArgs}", clientAppPath, logger, envVars);
        await RunCommandAsync("npm", $"run build {buildArgs}", clientAppPath, logger, envVars);

        var distPath = Path.Combine(clientAppPath, outDir);
        if (Directory.Exists(distPath))
        {
            logger.LogInformation("✅ Build output exists at: {DistPath}", distPath);
            var files = Directory.GetFiles(distPath, "*", SearchOption.AllDirectories);
            logger.LogInformation("📂 Found {FileCount} files in the build output", files.Length);
        }
        else
        {
            logger.LogWarning("⚠️ Build output directory not found at {DistPath}", distPath);
        }
    }

    private static async Task RunCommandAsync(
        string command,
        string arguments,
        string workingDirectory,
        ILogger logger,
        Dictionary<string, string> environmentVariables = null)
    {
        bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        string shell = isWindows ? "cmd.exe" : "/bin/bash";
        string shellArgs = isWindows ? $"/c {command} {arguments}" : $"-c \"{command} {arguments}\"";

        string fullPath = Path.GetFullPath(workingDirectory);

        var startInfo = new ProcessStartInfo
        {
            FileName = shell,
            Arguments = shellArgs,
            WorkingDirectory = fullPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (environmentVariables != null)
        {
            foreach (var kvp in environmentVariables)
            {
                startInfo.EnvironmentVariables[kvp.Key] = kvp.Value;
                logger.LogDebug("🔧 Setting environment variable: {Key}={Value}", kvp.Key, kvp.Value);
            }
        }

        using var process = new Process();
        process.StartInfo = startInfo;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation("▶️ Running command: {Command} {Args}", command, arguments);
        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        stopwatch.Stop();

        logger.LogInformation("✅ Command finished in {Seconds:F2} seconds", stopwatch.Elapsed.TotalSeconds);

        if (process.ExitCode != 0)
        {
            logger.LogError("❌ Command failed:\n{StdErr}", stderr);
            logger.LogError("📄 Command output:\n{StdOut}", stdout);
            // throw new Exception($"Command '{command} {arguments}' failed with exit code {process.ExitCode}");
        }

        if (!string.IsNullOrWhiteSpace(stdout))
        {
            logger.LogDebug("📄 Command output: {Output}", stdout.Substring(0, Math.Min(500, stdout.Length)));
        }
    }
}