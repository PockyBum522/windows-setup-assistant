﻿using WindowsSetupAssistant.Interfaces;

namespace WindowsSetupAssistant.Models.ISelectables.Installers;

/// <summary>
/// Contains data for an archive in the \Resources\Portable Applications\ folder
/// Mostly what to show for the display name and where to install it if the user selects it
/// </summary>
public class ArchiveInstaller : ISelectable
{
    /// <inheritdoc />
    public string DisplayName { get; set; } = "";

    /// <summary>
    /// Path to the folder containing the portable application
    /// </summary>
    public string ArchiveFilename { get; set; } = "";
    
    /// <summary>
    /// Path to copy the portable application to
    /// </summary>
    public string DestinationPath { get; set; } = "";

    /// <inheritdoc />
    public bool IsSelected { get; set; }
}