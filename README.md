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
setting up. So, set only that parameters that you want to change.

### Possible configurations

#### Section "battery"
- **OnlyWithNameTag** *[true|false]*: Set this to true if you want that this script shows
only the status of the specific batteries.
- **NameTag** *[string]*: The name that you want to use to mark that specific batteries
that you want to visit. This string musst be part of the block name.

#### Section "lcd"
- **NameTag** *[string]*: The name that you use to mark text panels that will display
the status of all your batteries. This string musst be part of the block name.
- **Widscreen** *[true|false]*: Set this to true if your text panels are widescreens.
- **Brightness** *[0-255]*: Setup the brightness of your text panels.

#### Section "display"
- **OnlySmallSigns** *[true|false]*: Set this to true if you want to use only small signs.
- **ShowBatteryTitle** *[true|false]*: If true the title will be shown.
- **ShowUnderlineBelowTitle** *[true|false]*: If true a line under the title will be generated.
- **ShowUnderlineBelowStatus** *[true|false]*: If true a line between the amount of batteries, 
stored power and the signs.
- **ShowSeperatorInStatus** *[true|false]*: If true a vertical line will be shown between the
amount of batteries and stored power.
- **ShowBatteryAmount** *[true|false]*: If true the amount of batteries will be shown.
- **ShowTotalStoredPower** *[true|false]*: If rue the amount of stored power of all batteries
will be shown.

#### Section "system"
- **UpdatingEnabled** *[true|false]*: If this is set to true the script will be updates automaticaly.
- **UpdateInterval** *[1...n]*: Value is in seconds. Set this value to specify the time between updates.

## Remarks

Script Name: BatteryStatus_Script_(x10)_v1																	 
Author: Lightwolf / Alias: Richy Sedlar																		 
My Youtube Channel:	https://www.youtube.com/channel/UCq4DnC39P6nrGE26xY-63Xg										 
YT-Video: https://youtu.be/cbfwXqCVVAo																							 

This script shows Battery level state on standart LCDs or wide LCDs, as digital Symbols 
Just edit your options, and it you will able to overwatch up to 100 Battery's very easy.*/
