using WindowsPostSetupAssistant.Main.Interfaces;

namespace WindowsPostSetupAssistant.Main.CommandLine;

public class ArgumentsParser
{
    private readonly ICommandLineInterface _commandLineInterface;

    public ArgumentsParser(ICommandLineInterface commandLineInterface)
    {
        _commandLineInterface = commandLineInterface;
    }
    
    public bool ArgumentPresent(string argumentName)
    {
        // Check each argument passed from command line
        foreach (var argument in _commandLineInterface.GetCommandLineArgs())
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
        var numberOfArguments = _commandLineInterface.GetCommandLineArgs().Length;
        
        for (var i = 0; i < numberOfArguments; i++)
        {
            var argument = _commandLineInterface.GetCommandLineArgs()[i];

            // If it doesn't match, continue
            if (!argument.ToLower().Contains(argumentName.ToLower())) continue;
            
            // Otherwise, continue if there's nothing to the right of this argument:
            if (i + 1 >= numberOfArguments) continue;
            
            // Otherwise:
            var nextArgument = _commandLineInterface.GetCommandLineArgs()[i + 1];

            // If next argument is another argument, return empty string
            if (nextArgument.Contains('/')) return "";

            // Otherwise:
            return nextArgument;
        }

        return "";
    }
}