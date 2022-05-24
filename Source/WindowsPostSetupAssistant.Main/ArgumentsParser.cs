using System;

namespace WindowsPostSetupAssistant.Main;

public class ArgumentsParser
{
    public bool ArgumentPresent(string argumentName)
    {
        // Check each argument passed from command line
        foreach (var argument in Environment.GetCommandLineArgs())
        {
            var argumentMatches = argument.ToLower().Contains(argumentName.ToLower());
            
            if (argumentMatches)
            {
                return true;
            }
        }

        return false;
    }
    
    public string GetArgumentValue(string argumentName)
    {
        var numberOfArguments = Environment.GetCommandLineArgs().Length;
        
        for (var i = 0; i < numberOfArguments; i++)
        {
            var argument = Environment.GetCommandLineArgs()[i];

            // If it doesn't match, continue
            if (!argument.ToLower().Contains(argumentName.ToLower())) continue;
            
            // Otherwise, continue if there's nothing to the right of this argument:
            if (i + 1 > numberOfArguments) continue;
            
            // Otherwise:
            var nextArgument = Environment.GetCommandLineArgs()[i + 1];

            // If next argument is another argument, return empty string
            if (nextArgument.Contains('/')) return "";

            // Otherwise:
            return nextArgument;
        }

        return "";
    }
}