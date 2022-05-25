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
    }

    private void InitializeWindowsThemesCard()
    {
        var cardActions = new List<IConfigurationAction>();
        
        cardActions.Add(
            new BasicConfigurationAction() { Description = "Enable dark theme" });

        cardActions.Add(
            new BasicConfigurationAction() { Description = "Enable windows transparency" });

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
}
