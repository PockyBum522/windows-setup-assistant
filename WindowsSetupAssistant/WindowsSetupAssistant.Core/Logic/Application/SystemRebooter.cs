using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace WindowsSetupAssistant.Core.Logic.Application;

/// <summary>
/// Methods for rebooting the system
/// </summary>
public class SystemRebooter
{
    /// <summary>
    /// Reboots the system and exits program, so run it last
    /// </summary>
    public void RebootComputerAndExit()
    {
        Console.WriteLine("Exiting script temporarily. will reboot and re-run admin bat file on next startup...");
    
        Process.Start("powershell", "-C shutdown /r /t 60");
        
        while (Process.GetProcessesByName("powershell").Length < 1)
        {
            Thread.Sleep(1000);    
        }
        
        Thread.Sleep(1000);    
        
        Environment.Exit(0);
    }

    /// <summary>
    /// Prompts the user, asking if they are ready to reboot the PC
    /// </summary>
    public void PromptToRebootComputerAndExit()
    {
        var message = @"ONLY CLICK YES IF YOU HAVE CLICKED THROUGH ALL VISIBLE INSTALLERS AND THEY HAVE FINISHED!!!

(This is only if you selected to install applications with a non-silent installer, of course.)

Are you ready to reboot the computer?";
        
        if (MessageBox.Show(message, "WARNING: Okay to Reboot?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
        {
            // User clicked no
            Environment.Exit(0);
        }
        else
        {
            // User clicked yes
            Console.WriteLine("Exiting script, will reboot and re-run admin bat file on next startup if present...");
    
            Process.Start("powershell", "-C shutdown /r /t 5");
        
            while (Process.GetProcessesByName("powershell").Length < 1)
            {
                Thread.Sleep(1000);    
            }
        
            Environment.Exit(0);
        }
    }
}