# Windows Post-Setup Assistant
GUI to easily create and save automated configuration/app install profiles that can quickly be applied to fresh installations of Windows. 

Everything you need to set up your workstation the way you want it (automatically) after installing Windows!

# Prerequisites
* A Windows 10 Installation that is not configured with your preferred settings or applications.

* Internet on said machine

# Objectives 
<ul>

## Profile Creation
* Profiles should be able to install applications and configure windows/app settings in a predefined order

* Any application installation should be able to be marked as optional, which will prompt the user at runtime asking if they want to install it

* Any setting change should be able to be marked as optional, which will prompt the user at runtime asking if they want to make that change

* All prompts should happen as early as possible after running the main batch file so that for the majority of the time necessary, user interaction is not required

## Automatic Profile Execution
You should be able to run the main batch file as Admin and:

* Select a profile that you created previously using this application
* Answer prompts for anything you've marked optional all at the beginning

* Let it run (It will handle rebooting as necessary.)

* When it's finished, with no interaction on your part after the initial prompts, you should have:
    * Windows completely up-to-date, if desired
    * Any applications you want, installed
    * Your workstation almost completely configured:
        * Windows settings changed to match your chosen settings
        * Application settings changed to match your chosen settings
</ul>

# Future Objectives
* Windows 11 Support

# Usage:
<ul>

## Adding a new module:

* Add a new class under WindowsPostSetupAssistant.Core.Modules
* Implement IModule
* For ModuleGuid, generate a new GUID (Without {} ) and paste it in. You should end up with: public Guid ModuleGuid => new Guid("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");
* Set up ValidateArguments to point to a bool method that returns true if Arguments is valid, false otherwise
* Execute should point to a method that does what the module needs to do, using Arguments to do it, if applicable

## Terminology:

* A card is a UI element that contains one or more IConfigurationAction(s)
* IModule is what gets looked up by an action. IModule is what actually applies the configuration action on the computer.
</ul>