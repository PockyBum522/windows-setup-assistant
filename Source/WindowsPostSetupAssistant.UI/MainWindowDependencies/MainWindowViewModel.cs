using System;
using System.Collections.Generic;
using WindowsPostSetupAssistant.Core.Interfaces;
using WindowsPostSetupAssistant.Core.Models;
using WindowsPostSetupAssistant.Core.Models.ConfigurationActions;

namespace WindowsPostSetupAssistant.UI.MainWindowDependencies;

public class MainWindowViewModel
{
    public List<Card> Cards = new();
    
    public MainWindowViewModel()
    {
        InitializeWindowsThemesCard();
        InitializeWindows10TaskbarCard();
    }

    private void InitializeWindowsThemesCard()
    {
        var cardActions = new List<IConfigurationAction>();
        
        cardActions.Add(
            new BasicConfigurationAction() { Description = "Enable dark theme" });

        cardActions.Add(
            new BasicConfigurationAction() { Description = "Enable windows transparency", MarkedAsOptional = true});

        cardActions.Add(
            new TextInputConfigurationAction() { Description = "Set accent color to (hex):" });
        
        cardActions.Add(
            new TextInputConfigurationAction() { Description = "Set start menu color to (hex):" });
        
        cardActions.Add(
            new TextInputConfigurationAction() { Description = "Set wallpaper to color to (path):" });
        
        var windowsThemeSettingsCard = new Card()
        {
            Title = "Windows 10 Theme Options",
            CardActions = cardActions
        };
        
        Cards.Add(windowsThemeSettingsCard);
    }
    
    private void InitializeWindows10TaskbarCard()
    {
        var cardActions = new List<IConfigurationAction>();
        
        cardActions.Add(
            new BasicConfigurationAction() { Description = "Disable news and interests" });

        cardActions.Add(
            new DropdownConfigurationAction()
            {
                Description = "Set search icon to show as:",
                DropdownOptions = {"Hidden", "Show search Icon", "Show search box"}
            });

        var windowsTaskbarSettingsCard = new Card()
        {
            Title = "Windows 10 Taskbar Settings",
            CardActions = cardActions
        };
        
        Cards.Add(windowsTaskbarSettingsCard);
    }
}
