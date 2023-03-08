using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GuerrillaNtp;
using Serilog;

namespace WindowsSetupAssistant.Logic.TaskHelpers;

public class TimeHelper
{
    private readonly ILogger _logger;

    public TimeHelper(ILogger logger)
    {
        _logger = logger;
    }

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

    public void SyncSystemTime()
    {
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

        SetSystemTime(ref systemTime);
    }

    private DateTimeOffset GetNtpTimeUtc()
    {
        var  client = NtpClient.Default;
        var clock = client.Query();
        
        var utcTime = clock.UtcNow;

        return utcTime;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }
    
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetSystemTime(ref SYSTEMTIME st);
}