using System;
using System.IO;

namespace WindowsSetupAssistant.Core.Logic.Application;

/// <summary>
/// Contains methods for creating the startup script in public startup that reloads this application on reboot so it can continue
/// </summary>
public class StartupScriptWriter
{
    /// <summary>
    /// Creates the startup script in public startup that reloads this application on reboot so it can continue
    /// </summary>
    public void CreateRebootScriptInStartup()
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
}