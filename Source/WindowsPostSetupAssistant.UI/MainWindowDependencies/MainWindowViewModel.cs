using System;
using System.Collections.Generic;
using WindowsPostSetupAssistant.Core.Interfaces;
using WindowsPostSetupAssistant.Core.UIModels;
using WindowsPostSetupAssistant.Core.UIModels.ConfigurationActions;

namespace WindowsPostSetupAssistant.UI.MainWindowDependencies;

public class MainWindowViewModel
{
    public List<Card> Cards = new();
    
    public MainWindowViewModel()
    {
        // InitializeWindowsThemesCard();
        
        // InitializeWindows10TaskbarCard();
        
        InitializeInstallersCard();
    }

    // private void InitializeWindowsThemesCard()
    // {
    //     var cardActions = new List<IConfigurationAction>();
    //     
    //     cardActions.Add(
    //         new BasicConfigurationAction() { Description = "Enable dark theme" });
    //
    //     cardActions.Add(
    //         new BasicConfigurationAction() { Description = "Enable windows transparency", MarkedAsOptional = true});
    //
    //     cardActions.Add(
    //         new TextInputConfigurationAction() { Description = "Set accent color to (hex):" });
    //     
    //     cardActions.Add(
    //         new TextInputConfigurationAction() { Description = "Set start menu color to (hex):" });
    //     
    //     cardActions.Add(
    //         new TextInputConfigurationAction() { Description = "Set wallpaper to color to (path):" });
    //     
    //     var windowsThemeSettingsCard = new Card()
    //     {
    //         Title = "Windows 10 Theme Options",
    //         CardActions = cardActions
    //     };
    //     
    //     Cards.Add(windowsThemeSettingsCard);
    // }

    // private void InitializeWindows10TaskbarCard()
    // {
    //     var cardActions = new List<IConfigurationAction>();
    //     
    //     cardActions.Add(
    //         new BasicConfigurationAction() { Description = "Disable news and interests" });
    //
    //     cardActions.Add(
    //         new DropdownConfigurationAction
    //         (
    //             description: "Set search icon to show as",
    //             dropdownOptions: new List<string> {"Hidden", "Show search Icon", "Show search box"},
    //             associatedModuleGuid: new Guid("686622A6-2A7D-4616-B59C-2DE31651A58D")
    //         ));
    //
    //     var windowsTaskbarSettingsCard = new Card()
    //     {
    //         Title = "Windows 10 Taskbar Settings",
    //         CardActions = cardActions
    //     };
    //     
    //     Cards.Add(windowsTaskbarSettingsCard);
    // }
    
    private void InitializeInstallersCard()
    {
        var cardActions = new List<IConfigurationAction>();

        cardActions.Add(
            new DropdownConfigurationAction
            (
                description: "Install Windows Subsystem for Linux",
                dropdownOptions: new List<string> { "Ubuntu", "Debian" },
                associatedModuleGuid: new Guid("6869A2A6-2A7D-4616-B59C-2DE31651A58D")
            ));

        cardActions.Add(
            new BasicConfigurationAction(
                description: "Install Chrome",
                argumentsForModule: "googlechrome",
                associatedModuleGuid: new Guid("62369EAC-8C58-43A0-82FF-7468B875F0D8")
            ));
        
        cardActions.Add(
            new BasicConfigurationAction(
                description: "Install Notepad++",
                argumentsForModule: "notepadplusplus",
                associatedModuleGuid: new Guid("62369EAC-8C58-43A0-82FF-7468B875F0D8")
            ));

        var installersTestCard = new Card()
        {
            Title = "Install",
            CardActions = cardActions
        };
        
        Cards.Add(installersTestCard);
    }
}
