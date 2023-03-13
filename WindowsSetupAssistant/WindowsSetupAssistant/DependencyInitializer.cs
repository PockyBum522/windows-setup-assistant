using Serilog;

namespace WindowsSetupAssistant;

/// <summary>
/// Poor man's dependency injection
/// </summary>
public static class DependencyInitializer
{
    /// <summary>
    /// Set up the Serilog logger
    /// </summary>
    /// <returns>Serilog Logger</returns>
    public static ILogger GetConfiguredLogger()
    {
        return new LoggerConfiguration()
            .Enrich.WithProperty("Application", "SerilogTestContext")
            .MinimumLevel.Debug()
            .WriteTo.File(ApplicationPaths.LogPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();
    }
}