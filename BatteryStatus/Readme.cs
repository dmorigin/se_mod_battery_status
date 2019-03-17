/*##############################################################################
# 	Script Name: BatteryStatus_Script_(x10)_v1
# 	Author: Lightwolf / Alias: Richy Sedlar
# 	My Youtube Channel:	https://www.youtube.com/channel/UCq4DnC39P6nrGE26xY-63Xg
#	YT-Video: https://youtu.be/cbfwXqCVVAo
#
#   Modified by: DMOrigin
#   URL: https://www.gamers-shell.de/
#   Source: https://github.com/dmorigin/se_mod_battery_status

################################################################################
# 	Description:
    This script shows Battery level state on standart LCDs or wide LCDs, as digital Symbols
    Just edit your options, and it you will able to overwatch up to 100 Battery's very easy.*/

/*
 * Configuration Parameter
 * 
 * Section "battery"
 * - OnlyWithNameTag [true|false]: Set this to true if you want that this script shows
 *  only the status of the specific batteries.
 * - NameTag [string]: The name that you want to use to mark that specific batteries
 * that you want to visit. This string musst be part of the block name.
 * - OnlyLocalGrid [true|false]: Set this to true if you want to check only the local grid
 * and not all batteries of all connected grids. Default is false.
 * 
 * Section "lcd"
 * - NameTag [string]: The name that you use to mark text panels that will display
 * the status of all your batteries. This string musst be part of the block name.
 * - Widscreen [true|false]: Set this to true if your text panels are widescreens.
 * - Brightness [0-255]: Setup the brightness of your text panels.
 * - OnlyLocalGrid [true|false]: Use only displays that are part of the local grid.
 * if you want to use lcd's of other grid set this to false. Default is true.
 * 
 * Section "display"
 * - OnlySmallSigns [true|false]: Set this to true if you want to use only small signs.
 * - ShowBatteryTitle [true|false]: If true the title will be shown.
 * - ShowUnderlineBelowTitle [true|false]: If true a line under the title will be generated.
 * - ShowUnderlineBelowStatus [true|false]: If true a line between the amount of batteries, 
 * stored power and the signs.
 * - ShowSeperatorInStatus [true|false]: If true a vertical line will be shown between the
 * amount of batteries and stored power.
 * - ShowBatteryAmount [true|false]: If true the amount of batteries will be shown.
 * - ShowTotalStoredPower [true|false]: If rue the amount of stored power of all batteries
 * will be shown.
 * 
 * Section "system"
 * - UpdatingEnabled [true|false]: If this is set to true the script will be updates automaticaly.
 * - UpdateInterval [1...n]: Value is in seconds. Set this value to specify the time between updates.
 */