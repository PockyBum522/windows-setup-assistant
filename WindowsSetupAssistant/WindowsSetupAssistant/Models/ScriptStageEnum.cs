namespace WindowsSetupAssistant.Models;

public enum ScriptStageEnum
{
    Uninitialized,
    WindowsHasBeenUpdatedOnce,
    WindowsHasBeenUpdatedTwice,
    WindowsHasBeenUpdatedFully,
    DomainHasBeenJoined
}