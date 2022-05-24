using System;
using WindowsPostSetupAssistant.UI;
using WindowsPostSetupAssistant.UI.MainWindowDependencies;

namespace WindowsPostSetupAssistant.Main;

public static class Program
{
    private static string ExecuteProfileArgument => "executeProfile";
    private static string ChooseProfileArgument => "chooseProfile";

    [STAThread]
    public static void Main()
    {
        var argumentsParser = new ArgumentsParser();

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

            Environment.Exit(0);
        }
        
        // If neither profile activation option is present, user is just running the GUI
        if (!argumentsParser.ArgumentPresent(ExecuteProfileArgument) &&
            !argumentsParser.ArgumentPresent(ChooseProfileArgument))
        {
            LaunchGui();
        }

        if (argumentsParser.ArgumentPresent(ChooseProfileArgument))
        {
            Console.WriteLine();
            Console.WriteLine("======== RUNNING CHOOSE PROFILE HERE ========");
            Console.WriteLine();
        }
        
        if (argumentsParser.ArgumentPresent(ExecuteProfileArgument))
        {
            Console.WriteLine();
            Console.WriteLine("======== EXECUTE SELECTED PROFILE HERE ========");
            Console.WriteLine($"Selected profile passed is: {argumentsParser.GetArgumentValue(ExecuteProfileArgument)}");
            Console.WriteLine();
        }
    }

    [STAThread]
    private static void LaunchGui()
    {
        var uiMainWindow = new MainWindow();

        uiMainWindow.ShowDialog();
    }
}