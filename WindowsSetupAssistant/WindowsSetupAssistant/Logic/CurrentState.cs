﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Serilog;
using WindowsSetupAssistant.Models;
using WindowsSetupAssistant.WindowResources;

namespace WindowsSetupAssistant.Logic;

public class CurrentState
{
    private readonly ILogger _logger;
    public static string StatePath => @"C:\Users\Public\Documents\CSharpInstallerScriptState.json";
    public static string StagePath => @"C:\Users\Public\Documents\CSharpInstallerScriptStage.json";
    
    public ScriptStageEnum ScriptStage;

    public MainWindowPartialViewModel MainWindowPartialViewModel { get; set; }

    public CurrentState(ILogger logger)
    {
        _logger = logger;
        
        MainWindowPartialViewModel = new();
        
        LoadStateFromDisk();
    }

    public void DeleteSavedChoicesAndStageOnDisk()
    {
        _logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        File.Delete(StatePath);
        File.Delete(StagePath);

        // Delete script from startup
        var startupFolderPath = Environment.ExpandEnvironmentVariables("%AllUsersProfile%") + @"\Start Menu\Programs\Startup";
		        
        var startupBatPath = Path.Join(startupFolderPath, "Re-RunAdminBatFileAutomatically.bat");
		        
        File.Delete(startupBatPath);
		        
        Environment.Exit(0);
    }
    
    public void SaveCurrentState()
    {
        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        using var jsonStateFileWriter = new StreamWriter(StatePath);
        
        using var jsonStateWriter = new JsonTextWriter(jsonStateFileWriter) { Formatting = Formatting.Indented };
        
        serializer.Serialize(jsonStateWriter, MainWindowPartialViewModel);
        
        using var jsonStageFileWriter = new StreamWriter(StagePath);
        
        using var jsonStageWriter = new JsonTextWriter(jsonStageFileWriter){ Formatting = Formatting.Indented };
        
        serializer.Serialize(jsonStageWriter, ScriptStage);

        CreateRebootScriptInStartup();
    }

    public void RebootComputer()
    {
        Console.WriteLine("Exiting script temporarily. will reboot and re-run admin bat file on next startup...");
    
        Process.Start("shutdown", "/r /t 0");
        
        Thread.Sleep(1000);
        
        Environment.Exit(0);
    }
    
    private void CreateRebootScriptInStartup()
    {
        // Delete script from startup
        var startupFolderPath = Environment.ExpandEnvironmentVariables("%AllUsersProfile%") + @"\Start Menu\Programs\Startup";
		        
        var startupBatPath = Path.Join(startupFolderPath, "Re-RunAdminBatFileAutomatically.bat");
	
        // Add batch file to startup that will call this after reboot
        // ReSharper disable once StringLiteralTypo
        var batchFileContents = @$"
echo ""Loading bootloader batch file as admin again.""

echo ""Delaying for internet...""

timeout 60

echo ""Now re-running file as admin""

setlocal DisableDelayedExpansion
set cmdInvoke=1
set winSysFolder=System32
set ""batchPath=%~dpnx0""
rem this works also from cmd shell, other than %~0
for %%k in (%0) do set batchName=%%~nk
set ""vbsGetPrivileges=%temp%\OEgetPriv_batchScriptV01.vbs""
setlocal EnableDelayedExpansion

ECHO.
ECHO **************************************
ECHO Invoking UAC for Privilege Escalation
ECHO **************************************
ECHO.

ECHO Set UAC = CreateObject^(""Shell.Application""^) > ""%temp%\OEgetPriv_batchScriptV01.vbs""
ECHO args = ""ELEV "" >> ""%temp%\OEgetPriv_batchScriptV01.vbs""
ECHO For Each strArg in WScript.Arguments >> ""%temp%\OEgetPriv_batchScriptV01.vbs""
ECHO args = args ^& strArg ^& "" ""  >> ""%temp%\OEgetPriv_batchScriptV01.vbs""
ECHO Next >> ""%temp%\OEgetPriv_batchScriptV01.vbs""
ECHO UAC.ShellExecute ""!batchPath!"", args, """", ""runas"", 1 >> ""%temp%\OEgetPriv_batchScriptV01.vbs""

: Now run the file we just made
powershell start '{ApplicationPaths.ThisApplicationProcessPath}' ""%temp%\OEgetPriv_batchScriptV01.vbs""
";

        File.WriteAllText(startupBatPath, batchFileContents);
    }

    private void LoadStateFromDisk()
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        if (File.Exists(StatePath))
        {
            var jsonStateRaw = File.ReadAllText(StatePath);

            MainWindowPartialViewModel =
                JsonConvert.DeserializeObject<MainWindowPartialViewModel>(jsonStateRaw, settings) ??
                new MainWindowPartialViewModel();

            _logger.Debug("Loaded current state from disk");
        }
        else
        {
            _logger.Warning("No current state file on disk. This is fine as long as this is the first run");
        }

        if (File.Exists(StagePath))
        {
            var jsonStageRaw = File.ReadAllText(StagePath);

            ScriptStage = JsonConvert.DeserializeObject<ScriptStageEnum>(jsonStageRaw, settings);
            
            _logger.Information("Loaded current stage from disk: {Stage}", ScriptStage);
        }
        else
        {
            ScriptStage = ScriptStageEnum.Uninitialized;
            
            _logger.Warning("No current stage file on disk. This is fine as long as this is the first run");
        }
    }
}