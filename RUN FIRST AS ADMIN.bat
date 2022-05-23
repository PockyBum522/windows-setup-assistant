@echo off

IF %ERRORLEVEL% EQU 0 (
   
   echo ---------------------------------------------------
   echo NOTICE: TEMPORARY MEASURES FOR RUNNING APPLICATION
   echo ---------------------------------------------------
   echo.
   echo In order to ensure that no user interaction is needed, we need to do a few things:
   echo 1. UAC Needs to be set to "Never Notify"
   echo 2. Automatic logon to your user account needs to be set so that reboots may take place with no interaction
   echo.
   echo Once the application has finished, you will be prompted to ask if 
   echo you want to set UAC and automatic logon back to their default settings
   echo.
   echo Once you have read this, press any key to continue and 
   echo this script will assist you with setting these settings...
   echo.

   pause

) ELSE (

   echo -------------------------------------------------------------
   echo ERROR: YOU ARE NOT RUNNING THIS WITH ADMINISTRATOR PRIVILEGES.
   echo -------------------------------------------------------------
   echo. 
   echo If you're seeing this, it means you don't have admin privileges!
   echo You will need to restart this program with Administrator privileges by right-clicking and selecting "Run As Administrator"
   echo. 
   echo Press any key to exit. Make sure to Run As Administrator next time!
   echo. 

   pause

   EXIT /B 1   
)

echo.
echo Installing Chocolatey
echo.

powershell -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"

echo.
echo Setting Chocolatey confirmation prompts to not show
echo.

choco feature enable -n=allowGlobalConfirmation

echo.
echo If Chocolatey was previously installed, checking for updates
echo.

choco upgrade chocolatey

echo.
echo Installing latest Dot NET SDK
echo.

choco upgrade dotnet-sdk

:: Bugfix that takes care of certain observed instances where 
:: dot net dependencies were potentially not installed properly
choco install dotnet-sdk --force

echo.
echo Installing powershell core
echo.

:: Ensures we can run any necessary powershell commands with the latest version if necessary
choco upgrade powershell-core --install-arguments='"ADD_EXPLORER_CONTEXT_MENU_OPENPOWERSHELL=1 ADD_FILE_CONTEXT_MENU_RUNPOWERSHELL=1 USE_MU=1 ENABLE_MU=1"'

echo.
echo Refreshing environment variables for this shell instance
echo.

::echo "RefreshEnv.cmd only works from cmd.exe, please install the Chocolatey Profile to take advantage of refreshenv from PowerShell"
call :RefreshEnvironmentVariables

echo.
echo Now that all dependencies are installed, running main application
echo.

:: %~dp0 is shorthand for full path script is currently running from
:: Run Application Here

echo. ------------------------------------------------------
echo. APPLICATION HAS FINISHED CONFIGURATION OF THIS MACHINE
echo. ------------------------------------------------------
echo.
echo. Your workstation is now configured per your chosen settings profile.
echo.
echo. You can press any key to exit.
echo.

pause

EXIT /B 1   

:: -------------------------------------------------------
:: BELOW HERE IS ALL CODE FROM CHOCOLATEY'S RefreshEnv.cmd
:: -------------------------------------------------------

:: Set one environment variable from registry key
:SetFromReg
    "%WinDir%\System32\Reg" QUERY "%~1" /v "%~2" > "%TEMP%\_envset.tmp" 2>NUL
    for /f "usebackq skip=2 tokens=2,*" %%A IN ("%TEMP%\_envset.tmp") do (
        echo/set "%~3=%%B"
    )
    goto :EOF

:: Get a list of environment variables from registry
:GetRegEnv
    "%WinDir%\System32\Reg" QUERY "%~1" > "%TEMP%\_envget.tmp"
    for /f "usebackq skip=2" %%A IN ("%TEMP%\_envget.tmp") do (
        if /I not "%%~A"=="Path" (
            call :SetFromReg "%~1" "%%~A" "%%~A"
        )
    )
    goto :EOF

:RefreshEnvironmentVariables
    echo/@echo off >"%TEMP%\_env.cmd"

    :: Slowly generating final file
    call :GetRegEnv "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" >> "%TEMP%\_env.cmd"
    call :GetRegEnv "HKCU\Environment">>"%TEMP%\_env.cmd" >> "%TEMP%\_env.cmd"

    :: Special handling for PATH - mix both User and System
    call :SetFromReg "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" Path Path_HKLM >> "%TEMP%\_env.cmd"
    call :SetFromReg "HKCU\Environment" Path Path_HKCU >> "%TEMP%\_env.cmd"

    :: Caution: do not insert space-chars before >> redirection sign
    echo/set "Path=%%Path_HKLM%%;%%Path_HKCU%%" >> "%TEMP%\_env.cmd"

    :: Cleanup
    del /f /q "%TEMP%\_envset.tmp" 2>nul
    del /f /q "%TEMP%\_envget.tmp" 2>nul

    :: capture user / architecture
    SET "OriginalUserName=%USERNAME%"
    SET "OriginalArchitecture=%PROCESSOR_ARCHITECTURE%"

    :: Set these variables
    call "%TEMP%\_env.cmd"

    :: Cleanup
    del /f /q "%TEMP%\_env.cmd" 2>nul

    :: reset user / architecture
    SET "USERNAME=%OriginalUserName%"
    SET "PROCESSOR_ARCHITECTURE=%OriginalArchitecture%"

    echo | set /p dummy="Finished refreshing environtment variables."
    echo.