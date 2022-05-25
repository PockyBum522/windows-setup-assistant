using System;
using WindowsPostSetupAssistant.Main.Interfaces;

namespace WindowsPostSetupAssistant.Main.CommandLine;

public class CommandLineInterface : ICommandLineInterface
{
    public string[] GetCommandLineArgs()
    {
        return Environment.GetCommandLineArgs();
    }
}