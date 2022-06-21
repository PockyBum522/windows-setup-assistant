namespace WindowsPostSetupAssistant.Core;

/// <summary>
/// Stores central paths related to the application itself, such as settings storage paths and logging paths
/// </summary>
public static class ApplicationPaths
{
    /// <summary>
    /// Full path to the directory the app is running from, used for building log and settings directories
    /// </summary>
    public static string GetAppRunFromDirectory()
    {
        try
        {
            return Path.GetDirectoryName(Environment.ProcessPath) ?? "ERROR_GETTING_APP_PATH";
        }
        catch (IOException ex)
        {
            throw new Exception($"Can't get app root directory{Environment.NewLine}{ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Full path to the app's local settings INI file
    /// </summary>
    public static string PathSettingsApplicationLocalIniFile => throw new NotImplementedException();
    //     Path.Combine(
    //         ApplicationData.AppName, 
    //         "settings.ini");

    /// <summary>
    /// Full path to base folder for logs (the folder, not the log files themselves)
    /// </summary>
    public static string LogAppBasePath =>
        Path.Combine(
            GetAppRunFromDirectory(),
            "Logs",
            ApplicationData.AppName,
            Environment.UserName);
    
    /// <summary>
    /// Full path to a generic log filename, for Serilog
    /// </summary>
    public static string LogPath => 
        Path.Combine(
            LogAppBasePath,
            $"{ApplicationData.AppName}.log");
}