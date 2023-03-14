# Index

* [What it does](https://github.com/PockyBum522/windows-setup-assistant#windows-post-setup-assistant)
* [Features and Roadmap](https://github.com/PockyBum522/windows-setup-assistant#features-and-roadmap)
* [Prerequisites](https://github.com/PockyBum522/windows-setup-assistant#prerequisites)
* [Usage](https://github.com/PockyBum522/windows-setup-assistant#usage)
* [Objectives](https://github.com/PockyBum522/windows-setup-assistant#objectives)
* [Custom Configuration](https://github.com/PockyBum522/windows-setup-assistant#custom-configuration)
* [Detailed Breakdown of What's Going On in the Batch File Bootstrapper](https://github.com/PockyBum522/windows-setup-assistant#detailed-breakdown-of-whats-going-on-in-the-batch-file-bootstrapper)
* [Detailed Breakdown of What's Going On in the Main Application](https://github.com/PockyBum522/windows-setup-assistant#detailed-breakdown-of-whats-going-on-in-the-main-application)
* [Helping With Development](https://github.com/PockyBum522/windows-setup-assistant#helping-with-development)
* [Why didn't you just...?](https://github.com/PockyBum522/windows-setup-assistant#why-didnt-you-just)

# Windows Setup Assistant

[![.NET Core](https://github.com/PockyBum522/windows-setup-assistant/actions/workflows/dotnet.yml/badge.svg)](https://github.com/PockyBum522/windows-setup-assistant/actions/workflows/dotnet.yml)

[![Build history](https://buildstats.info/appveyor/chart/PockyBum522/windows-setup-assistant)](https://ci.appveyor.com/project/PockyBum522/windows-setup-assistant/history)

[![GitHub stars](https://img.shields.io/github/stars/PockyBum522/windows-setup-assistant.svg?style=social&label=Star&maxAge=2592000)](https://GitHub.com/PockyBum522/windows-setup-assistant/stargazers/)

### What it does

Takes the pain out of new Windows 10 installations, by installing new software and configuring settings for you, all unattended.

You should be able to:
1. Install Windows 10 on a computer
2. Connect to the internet
3. Run the bootstrapper batch file
4. Spend about 5 minutes waiting for the initial requirement applications and libraries to load
5. Answer a few questions about how you want your system configured, or load a profile you made previously with all that information
6. Walk away

In a few hours, depending on your internet speed and system speed, you should reurn to find that:

* Your new Windows install has all settings set how you like them
* Your Windows install is completely up to date (If you selected the "Update Windows" option)
* Any applications you selected for install are now ready
* Any necessary reboots are handled automatically, with the program resuming on next boot.
* Your computer has been renamed to your chosen hostname

Through the use of AutoLogon64 from Microsoft SysInternals, you can make the whole process happen completely unattended.


# Features and Roadmap

* Basic application installation - Complete!
* Unattended Application Installation/Settings Apply/Windows Update with reboots - Complete! 
* Profile load/save - Complete!
* GUI - Complete!
* windows Settings - Some! 

We are aiming to add more settings as the project progresses. We are still in the early stages of setting this up and making something that should be useful to everyone. Your help and pull requests are gratefully welcome!

# Prerequisites

* A Windows 10 Installation that is not configured with your preferred settings or applications.

* Internet on said machine


# Usage:

For the end user, there are a few things you should know:

First off, if you just want to try it, download the repo (Likely the easiest way is as a zip) and then just double click "RUN ME FIRST (BOOTSTRAPPER).bat"

This will set up a few things necessary for unattended install, like disabling UAC prompts (You can re-enable them after the application is finished configuring your computer) and setting up Automatic Logon to your user with Microsoft's AutoLogon.

After the batch file runs, it should build the main application with .NET 7 SDK, which will be installed automatically. 

Once you see the main window:

<a href="img/documentation/getting-started/MainWindow.png">
    <img src="img/documentation/getting-started/MainWindow.png" 
        alt="Picture of the Main Window of the application, where almost all of the settings are set and applications can be selected for install" 
        width=300 />
</a>

<br/>
You are ready to go. Pick what applications you'd like to have installed, and what settings you would like to have applied, and click "Save Profile" if you'd like to re-use this selection later. 

When finished, click "Start Execution" and then you can sit back and relax or walk away. Everything from here on out will be taken care of without user interaction.

If you have chosen to install standard .exe or .msi installers, they will be saved for the last part of the process.


# Objectives 

### Short-Term

* Better progress indication. Right now a lot of things happen invisibly. Not great.

* A better organization system for system settings, including the ability to have windows-setup-assistant crawl folders underneath it for .reg files which will then show up as selectable in the Main Window.

* Application settings able to be set, not just Windows settings

* Better logging and failure recovery

* Considering: Integration of AutoHotKey scripts for both program installation/installation automation as well as changing settings

### Long-Term

* Windows 11 Support


# Custom Configuration

Users can add their own application installs that will show up in the "Install Applications" list. 

Simply browse to \WindowsSetupAssistant\Resources\Configuration\ and edit the "Available Installs.json" file.

There are four types of installations you can add here:

* ChocolateyInstaller
* ExecutableInstaller
* ArchiveInstaller
* PortableApplicationInstaller

I would suggest copying and pasting an existing entry of the type that you want to add, and then editing the values to suit your needs.

### ChocolateyInstaller:

Properties:

* ChocolateyId

This should be set to whatever comes after "choco install" on [Chocolatey Packages](https://community.chocolatey.org/packages) so for "Adobe AIR runtime, for example, you see: choco install adobeair"

So set ChocolateyId to just adobeair

* Arguments 

If you need to pass arguments to the "choco install" command that gets run, put them in here.

* Parameters

Same thing as arguments

* DisplayName 

This is what shows up in the list of applications that can be selected in the main window.

* IsSelected

Used for tracking if the user has selected the entry for install. You can set this to true if you want the application to always launch with this entry selected, but most likely, profiles will help you accomplish what you're trying to do much more easily. 

### ExecutableInstaller:

Properties:

* FileName

This should be the full filename of the exe or msi to start. It should be located in \windows-setup-assistant\WindowsSetupAssistant\Resources\Installers\Installer Executables\

* Arguments 

If you need to pass arguments to the start process command that runs the installer, pass them here.

* AutoHotkeyMacro

Unused at this time.

* DisplayName 

This is what shows up in the list of applications that can be selected in the main window.

* IsSelected

Used for tracking if the user has selected the entry for install. You can set this to true if you want the application to always launch with this entry selected, but most likely, profiles will help you accomplish what you're trying to do much more easily. 

### ArchiveInstaller:

Properties:

* ArchiveFilename

This should be the full filename of the .7z or .zip to extract. It should be located in \windows-setup-assistant\WindowsSetupAssistant\Resources\Installers\Installer Archives\

* DestinationPath 

Where you would like to have the archive extracted to. This folder will be created automatically. \ characters must be escaped. A valid path would look like this:

"DestinationPath": "C:\\PortableApplications\\Arduino IDE\\",

* DisplayName 

This is what shows up in the list of applications that can be selected in the main window.

* IsSelected

Used for tracking if the user has selected the entry for install. You can set this to true if you want the application to always launch with this entry selected, but most likely, profiles will help you accomplish what you're trying to do much more easily. 

### PortableApplicationInstaller:

Properties:

* FolderName

This should be the full folder name of the folder to copy. It should be located in \windows-setup-assistant\WindowsSetupAssistant\Resources\Installers\Portable Applications\ 

* DestinationPath 

Where you would like to have the folder copied to. \ characters must be escaped. A valid path would look like this:

"DestinationPath": "C:\\PortableApplications\\Arduino IDE\\",

* DisplayName 

This is what shows up in the list of applications that can be selected in the main window.

* IsSelected

Used for tracking if the user has selected the entry for install. You can set this to true if you want the application to always launch with this entry selected, but most likely, profiles will help you accomplish what you're trying to do much more easily. 

* DesktopShortcutExePath 

Unused for now, this will later allow you to specify an .exe to make a shortcut to that will show up on your desktop.

* StartMenuShortcutExePath

Unused for now, this will later allow you to specify an .exe to make a shortcut to that will show up on your desktop.


# Detailed Breakdown of What's Going On in the Batch File Bootstrapper

When you double click on "RUN ME FIRST (BOOTSTRAPPER).bat" a few things happen:

* The batch file creates a .lockfile in C:\Users\Public\Documents\ so that the second half of the script (the second half runs as the user) waits until after the first half (first half runs as admin because Chocolatey needs it) has finished to allow the second half (non-admin) part to proceed.

* The batch file checks if you have an internet connection, exits if you don't.

* The batch file checks if you ran it as admin, exits if you DID. 

(This is because while it's easy to elevate the process later, it's suprisingly hard to figure out what user originally ran the batch file if it's started as administrator by them when they run it.)

* The batch file then notifies the user they should disable UAC prompts and set up AutoLogin. 

This is for unattended capability, both of these things can be re-enabled/disabled once the application finishes running and the computer reboots for the last time. 

* The batch file then installs Chocolatey, which we'll be using to install things

* The batch file installs the latest .NET SDK so we can build this application

* The batch file then installs powershell core and Notepad++ just to give some basic utilities to make troubleshooting easier.

* The admin half (first half) of the batch file now deletes the .lockfile and closes. 

Once the second half sees that the lockfile is gone, it knows everything it needs has been installed and it can proceed. 

* The first half does some basic nuget cleaning, restores the packages in the project, and then builds and runs the application.

# Detailed Breakdown of What's Going On in the Main Application

* When you make your selections and hit "Start Execution" the application immediately saves all of the slections to a JSON file stored in C:\Users\Public\Documents\ so that it can act on them later.

* It also creates a batch file in the Public Startup folder to re-run the application on subsequent reboots. 

This is deleted once the application has run through its complete process.

* If you selected to update windows, a CLI windows update handler will be installed, and windows will be updated as far as it will allow without a reboot. 

* The application then marks that it has gotten through the first stage by writing to another file in C:\Users\Public\Documents\

* When the computer is done rebooting to apply the windows updates, the batch file launches because it's in the public startup folder, and re-runs the application.

* Upon re-launch, the application checks the settings it saved at the beginning, and what stage of the process it's on. 

It does any necessary work, and then if necessary, updates the stage and reboots the computer. The first few reboots are just to install windows updates, reboot, then see if there are any more updates and install them.

* Once it's done updating windows, it will start installing selected applications. It handles all the selected 
ChocolateyInstallers, ArchiveInstallers, and PortableApplicationInstallers first, since those don't require user interaction. Once those are finished, it runs all ExecutableInstallers.

It will then load a warning dialog telling you to proceed through any ExecutableInstallers that may be on the screen, then once you are finished, press yes to reboot the computer.

This is the last reboot, and at this point the process is finished. The application will clean up the files it was using to save the current stage of the process it was in, the user options that were selected at the beginning, and the bat file in Public Startup.

# Helping With Development

So you read through all that? I'm impressed. Maybe a little frightened. I'm hesitant to put a lot of effort into this section at such an early stage of development, as things will likely change quickly.

However! Know that help and pull requests are GREATLY appreciated and I will review them in a timely manner.

For now, there's several things that would be helpful until I get a better idea of the structure of the code:

* Windows Settings

If you want a setting added and can get a .reg file or C# code to do what you want, send it over! I'll add it. Soon, I hope to have it so that you can just drop them in \windows-setup-assistant\WindowsSetupAssistant\Resources\Configuration\Registry Files\ and they will be indexed and become a selectable option, allowing for better customization by the end users in this area.

* New Installs

If you add to the "Available Installers.json" with something that you think other people will find useful, make a pull request! I probably won't approve all of them, but if you tell me why you think it should be included, I likely will!

For security reasons, I don't accept ArchiveInstallers or PortableAppsInstallers unless you can tell me where to download the thing on a well-known 3rd party website, and then give me the configuration for it. I will still likely scrutinize the heck out of it.

* Crash/Bug Reports

Log files are located in C:\Users\Public\Documents\Logs\

Please add an issue in github with the log and expected behavior. Logs shouldn't contain sensitive data (Possibly your username.) and if they do, let me know, so I can tell it to stop logging it.

* Code Review

I am self taught, and in addition to that, I don't know what I don't know.

If you see a better way to do something, or structure things, or anything, please talk to me about it. File an issue and use the "question" tag. I would love to have a conversation with you!


# Why didn't you just...?

Use group policy?

Use ninite? (I love ninite. Great product. I used the paid version for years.)

Use X, Y, or Z?

Mostly because I wanted to make something that's useful and usable by everyone! I wanted it to be simple to configure, powerful enough to be useful, and help real people save time. Since this application just pops up a window asking you what you'd like to install or configure, that seems pretty simple and usable to me! 

Up to this point, I have done lots of research and have never been able to find something that will let you do Windows settings, application installs, and custom configuration of those things all in one place. I aim to fix that.
