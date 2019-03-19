# Battery Status Script

## Space Engineers

This is a script for the game Space Engineers. You can put this script into a 
programable block. Just do it by copy paste. See the instructions for more 
informations.

## Notice

This is a modified version of the original Battery Status Script in version
1.1 [Vanilla] from Lightwolf Adventures. Currently the last update was of
his script was at 8 Jan. 2018. So, there are some issues with this script.
Therefore I have modified this script to bring it into the current game
version.

You can find the original script here: https://steamcommunity.com/sharedfiles/filedetails/?id=1238124032

## Instructions

I use the MDK from https://github.com/malware-dev/MDK-SE. The code is written
inside the community version of Visual Studio. Therefore you can find the
script under the BatteryStatus folder. Open the file Program.cs to view the
hole code.

If you want a copy'n paste version open file Release_Version in the root
directory. Select the hole content, copy it to your clipboard, switch to
Space Engineers and paste the code into your programable block.

## Configuration System

I hate to modify the script himself. It makes updates cumbersome. Because you need
to resetup your settings until every script update. Therefore I added a new feature
that allows you to configure this script outside the script.

Now it is possible to place a configuration inside the Custom Data field of that
programmable block which runs this script. Use the INI File format to store your
settings in that field.

You don't need to setup anything. If a setting is not set, a default value will be
used. So, set only that parameters that you want to change.

### Possible configurations

#### Section "battery"
- **OnlyWithNameTag** *[true|false]*: Set this to true if you want that this script shows
only the status of the specific batteries. Default is false.
- **NameTag** *[string]*: The name that you want to use to mark that specific batteries
that you want to visit. This string musst be part of the block name. Default is
"[Battery-Status]".
- **OnlyLocalGrid** *[true|false]*: Set this to true if you want to check only the local 
grid and not all batteries of all connected grids. Default is false.

#### Section "lcd"
- **NameTag** *[string]*: The name that you use to mark text panels that will display
the status of all your batteries. This string musst be part of the block name. Default
is "[Battery Status LCD]".
- **Widscreen** *[true|false]*: Set this to true if your text panels are widescreens.
Default is false.
- **Brightness** *[0-255]*: Setup the brightness of your text panels. Default value is 255.
- **OnlyLocalGrid** *[true|false]*: Use only displays that are part of the local grid. 
if you want to use lcd's of other grid set this to false. Default is true.

#### Section "display"
- **OnlySmallSigns** *[true|false]*: Set this to true if you want to use only small signs.
Default is false.
- **ShowBatteryTitle** *[true|false]*: If true the title will be shown. Default is true.
- **ShowUnderlineBelowTitle** *[true|false]*: If true a line under the title will be generated.
Default is true.
- **ShowUnderlineBelowStatus** *[true|false]*: If true a line between the amount of batteries, 
stored power and the signs. Default is true.
- **ShowSeperatorInStatus** *[true|false]*: If true a vertical line will be shown between the
amount of batteries and stored power. Default is true.
- **ShowBatteryAmount** *[true|false]*: If true the amount of batteries will be shown. Default
is true.
- **ShowTotalStoredPower** *[true|false]*: If rue the amount of stored power of all batteries
will be shown. Default is true.

#### Section "system"
- **UpdatingEnabled** *[true|false]*: If this is set to true the script will be updates automaticaly.
Default is true.
- **UpdateInterval** *[1...n]*: Value is in seconds. Set this value to specify the time between updates.
Default is 2.

### Configuration examples

The following examples shows you a basic configuration that is used in the most cases:
```
[lcd]
NameTag=[BatteryStatus]

[battery]
NameTag=[Status]
OnlyWithNameTag=true
```

The second example shows you a configuration with a widescreen panel and where only the grid of the programmable block will be checked.
```
[lcd]
NameTag=[BatteryStatus]
Widescreen=true

[system]
CheckOnlyLocalGrid=true
```

## Arguments

Now we need to talk about arguments. In Space Engineers it's possible to run a script with arguments. Arguments
are simple strings that passed to the script. There are multiple ways to pass arguments. The fastes way is to
use the field "Argument" of the programmable block himself. Type in your argument and press "Run". The following
Arguments are supported by this script.

- **restoreDefault**: Execute this script to restore all settings to it's default value. This is a destructive
 argument. All your previous settings will be overwritten by it's default value. Make sure you are know what 
 you are doing. Normaly there is no need to do this. The script will write the default values automaticaly once.
 If no data is set inside the CustomData field.
- **readConfig**: Use this argument after you has changed the settings. The settings will not be read automaticaly.
 This happens only once after the block is booting up. So, if you change some settings you need to tell the script
 that.

## Remarks

Script Name: BatteryStatus_Script_(x10)_v1																	 
Author: Lightwolf / Alias: Richy Sedlar																		 
My Youtube Channel:	https://www.youtube.com/channel/UCq4DnC39P6nrGE26xY-63Xg										 
YT-Video: https://youtu.be/cbfwXqCVVAo																							 

This script shows Battery level state on standart LCDs or wide LCDs, as digital Symbols 
Just edit your options, and it you will able to overwatch up to 100 Battery's very easy.*/
