using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        var firstAction = new BasicConfigurationAction()
        {
            
        };
        
        cardActions.Add();
        
        Cards.Add(
            new Card()
            {
                Title = "",
                CardActions = 
            }
        );
    }
}