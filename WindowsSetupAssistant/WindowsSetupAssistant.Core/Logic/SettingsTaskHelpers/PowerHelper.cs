using System.Diagnostics;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

/// <summary>
/// Methods for changing Windows and operating-system-related settings
/// </summary>
public class PowerHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public PowerHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Set monitor off, standby, and hibernate timeouts. If no parameters are specified, defaults to a very long
    /// timeout for everything on AC
    /// </summary>
    /// <param name="monitorOffAfterXMinutesOnAc">When on AC power, how long to wait (in minutes)to turn off the screen</param>
    /// <param name="monitorOffAfterXMinutesOnBattery">When on battery power, how long to wait (in minutes)to turn off the screen</param>
    /// <param name="standbyTimeOutMinutesOnAc">When on AC power, how long to wait (in minutes)to sleep the system</param>
    /// <param name="standbyTimeOutMinutesOnBattery">When on battery power, how long to wait (in minutes)to sleep the system</param>
    /// <param name="hibernateTimeOutMinutesOnAc">When on AC power, how long to wait (in minutes)to hibernate</param>
    public void SetPowerSettingsTo(
        int monitorOffAfterXMinutesOnAc = 250, 
        int monitorOffAfterXMinutesOnBattery = 5,
        
        int standbyTimeOutMinutesOnAc = 300,
        int standbyTimeOutMinutesOnBattery = 10,
        
        int hibernateTimeOutMinutesOnAc = 0)
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -monitor-timeout-ac {monitorOffAfterXMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -monitor-timeout-dc {monitorOffAfterXMinutesOnBattery}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = "-x -disk-timeout-ac 0",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -standby-timeout-ac {standbyTimeOutMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
        
        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -standby-timeout-dc {standbyTimeOutMinutesOnBattery}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();

        processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -hibernate-timeout-ac {hibernateTimeOutMinutesOnAc}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
    }
}