using Serilog;

namespace WindowsSetupAssistant;

public static class DependencyInitializer
{
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