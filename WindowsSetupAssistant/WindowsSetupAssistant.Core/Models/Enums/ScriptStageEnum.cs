namespace WindowsSetupAssistant.Core.Models.Enums;

/// <summary>
/// Enum for what stage of the overall Windows setup process this application is in
/// </summary>
public enum ScriptStageEnum
{
    /// <summary>
    /// Uninitialized, also starting default
    /// </summary>
    Uninitialized,
    
    /// <summary>
    /// First run
    /// </summary>
    FirstRun,
    
    /// <summary>
    /// After we have run windows update once
    /// </summary>
    WindowsHasBeenUpdatedOnce,
        
    /// <summary>
    /// After we have run windows update twice
    /// </summary>
    WindowsHasBeenUpdatedTwice,
        
    /// <summary>
    /// After we have run windows update three times, this is also the stage where we finally install applications
    /// </summary>
    WindowsHasBeenUpdatedFully
}