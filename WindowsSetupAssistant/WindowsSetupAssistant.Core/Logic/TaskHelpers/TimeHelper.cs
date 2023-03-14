using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GuerrillaNtp;
using Serilog;

namespace WindowsSetupAssistant.Core.Logic.TaskHelpers;

/// <summary>
/// Methods for working with Windows time/data and the system time
/// </summary>
public class TimeHelper
{
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="logger">Injected ILogger to use</param>
    public TimeHelper(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sets system time zone
    /// </summary>
    /// <param name="timeZoneId">The timezone ID as needed by tzutil.exe For instance, "Eastern Standard Time"</param>
    public void SetSystemTimeZone(string timeZoneId)
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "tzutil.exe",
            Arguments = "/s \"" + timeZoneId + "\"",
            UseShellExecute = false,
            CreateNoWindow = true
        });

        if (process == null) return;
        
        process.WaitForExit();
                
        TimeZoneInfo.ClearCachedData();
    }

    /// <summary>
    /// Syncs system time via NTP
    /// </summary>
    public void SyncSystemTime()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        var utcNtpTime = GetNtpTimeUtc();
        
        var systemTime = new SYSTEMTIME
        {
            wYear = (short)utcNtpTime.Year, // must be short
            wMonth = (short)utcNtpTime.Month,
            wDay = (short)utcNtpTime.Day,
            wHour = (short)utcNtpTime.Hour,
            wMinute = (short)utcNtpTime.Minute,
            wSecond = (short)utcNtpTime.Second
        };

        _logger.Information("From NTP got: {TimeObject}", utcNtpTime);

        SetSystemTime(ref systemTime);

        _logger.Information("System time synced!");
    }

    private DateTimeOffset GetNtpTimeUtc()
    {
        var  client = NtpClient.Default;
        var clock = client.Query();
        
        var utcTime = clock.UtcNow;

        return utcTime;
    }
    
    /// <summary>
    /// Struct for passing time to kernel32.dll using SetSystemTime()
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming because this is external type
    public struct SYSTEMTIME {
        /// <summary>
        /// Year
        /// </summary>
        public short wYear;
        
        /// <summary>
        /// Month
        /// </summary>
        public short wMonth;
        
        /// <summary>
        /// Day of week, not needed to set system time
        /// </summary>
        public short wDayOfWeek;
        
        /// <summary>
        /// Day of month
        /// </summary>
        public short wDay;
        
        /// <summary>
        /// Hour of day, 0 - 23
        /// </summary>
        public short wHour;
        
        /// <summary>
        /// Minute of hour
        /// </summary>
        public short wMinute;
        
        /// <summary>
        /// Second of minute
        /// </summary>
        public short wSecond;
        
        /// <summary>
        /// Millisecond of second
        /// </summary>
        public short wMilliseconds;
    }
    
    /// <summary>
    /// Sets the system time
    /// </summary>
    /// <param name="st">Passed SYSTEMTIME struct with time to set</param>
    /// <returns>I am unsure if this is true on success? Look it up.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetSystemTime(ref SYSTEMTIME st);
}