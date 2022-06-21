using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using WindowsPostSetupAssistant.Core;
using WindowsPostSetupAssistant.Main.CommandLine;
using WindowsPostSetupAssistant.UI.MainWindowDependencies;

namespace WindowsPostSetupAssistant.Main;

public static class Program
{
    private static string ExecuteProfileArgument => "executeProfile";
    private static string ChooseProfileArgument => "chooseProfile";
    
    private static readonly Logger Logger;
    private static string[] _args;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    static Program()
    {
        Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Application", "SerilogTestContext")
            //.MinimumLevel.Information()
            .MinimumLevel.Debug()
            .WriteTo.File(ApplicationPaths.LogPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
    }
    
    [STAThread]
    public static void Main(string[] args)
    {
        _args = args;
        
#if DEBUG
        // Only restart for admin elevation if in release mode
#else
        if (!CheckIfRunningAsAdministrator())
        {
            Logger.Information("Application is not running as administrator, restarting...");
            RerunThisApplicationAsAdministrator();   
        }
        
        Logger.Information("Application running as administrator"); 
#endif
        
        var argumentsParser = new ArgumentsParser(new CommandLineInterface());

        // Check if both are present and warn user they are mutually exclusive
        if (argumentsParser.ArgumentPresent(ExecuteProfileArgument) &&
            argumentsParser.ArgumentPresent(ChooseProfileArgument))
        {
            Console.WriteLine();
            Console.WriteLine("ERROR: Mutually exclusive arguments present");
            Console.WriteLine();
            Console.WriteLine($"Both /{ExecuteProfileArgument} and /{ChooseProfileArgument}");
            Console.WriteLine("Were found. They are mutually exclusive and cannot be run together.");
            Console.WriteLine();
            Console.WriteLine("Please modify arguments.");
            Console.WriteLine("Program will now exit.");

            Console.ReadLine();

            Environment.Exit(0);
        }
        
        // If neither profile activation option is present, user is just running the GUI
        if (!argumentsParser.ArgumentPresent(ExecuteProfileArgument) &&
            !argumentsParser.ArgumentPresent(ChooseProfileArgument))
        {
            HideConsoleWindow();
            
            LaunchGui();
        }

        if (argumentsParser.ArgumentPresent(ChooseProfileArgument))
        {
            Console.WriteLine();
            Console.WriteLine("======== RUNNING CHOOSE PROFILE HERE ========");
            Console.WriteLine();
            
            Console.ReadLine();
        }
        
        if (argumentsParser.ArgumentPresent(ExecuteProfileArgument))
        {
            Console.WriteLine();
            Console.WriteLine("======== EXECUTE SELECTED PROFILE HERE ========");
            Console.WriteLine($"Selected profile passed is: {argumentsParser.GetArgumentValue(ExecuteProfileArgument)}");
            Console.WriteLine();
            
            Console.ReadLine();
        }
    }

    private static void HideConsoleWindow()
    {
        const int SW_HIDE = 0;
        
        var handle = GetConsoleWindow();

        ShowWindow(handle, SW_HIDE);
    }
    
    [STAThread]
    private static void RerunThisApplicationAsAdministrator()
    {
        var executableFullPath = Assembly.GetExecutingAssembly().Location
            .Replace(".dll", ".exe");

        Logger.Information("Running from: {ExeName}", executableFullPath ?? "");
        
        var thisAppNewProcess = new Process();
        
        thisAppNewProcess.StartInfo.FileName = executableFullPath;
        thisAppNewProcess.StartInfo.Arguments = string.Join(" ", _args);
        thisAppNewProcess.StartInfo.Verb = "runas";
        thisAppNewProcess.StartInfo.UseShellExecute = true;
        
        Logger.Information("About to restart: {ApplicationPath}", executableFullPath);
        thisAppNewProcess.Start();
        
        Logger.Information("Exiting old process");
        Environment.Exit(0);
    }
    
    [STAThread]
    private static bool CheckIfRunningAsAdministrator()
    {
        var id = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(id);
        
        var runningAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

        return runningAsAdmin;
    }
    
    [STAThread]
    private static void LaunchGui()
    {
        var uiMainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel(Logger)
        };

        uiMainWindow.ShowDialog();
    }
}