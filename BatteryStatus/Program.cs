using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        /*##############################################################################
        # 	Script Name: BatteryStatus_Script_(x10)_v1
        # 	Author: Lightwolf / Alias: Richy Sedlar
        # 	My Youtube Channel:	https://www.youtube.com/channel/UCq4DnC39P6nrGE26xY-63Xg
        #	YT-Video: https://youtu.be/cbfwXqCVVAo
        #
        #   Modified by: DMOrigin
        #   URL: https://www.gamers-shell.de/

        ################################################################################
        # 	Description:
            This script shows Battery level state on standart LCDs or wide LCDs, as digital Symbols
            Just edit your options, and it you will able to overwatch up to 100 Battery's very easy.*/

        //-----   Generals    -----

        // LCD / Text Panel to show status
        // All LCD with a specific NameTag included in their name, will be show the information. You can edit the NameTag here.
        string LCD_NameTag = "[Battery Status LCD]"; //example: my LCD 31 [Battery Status LCD]

        // NameTags Specific Blocks
        // on default this script search for all Battery's attached, and show there Status, but somethimes you want be able to
        // show only specific Battery's, then set OnlyBatteryWithNameTag to true.
        bool OnlyBatteryWithNameTag = false; //false	= show all found Battery's attached, true = show only Battery's with Specific NameTag in their name

        //	that means that only Battery's with a specific Name or a Word, that you added to your Battery Blockname, will be shown only
        //	you can change the Nametag here.
        string BatteryNameTag = "[Battery-Status]"; // NameTag to show only specific Battery's, example: my Battery 15 [Battery-Status]

        //-----   Display Settings    -----

        // Wide LCD or LCD
        // on default it shows a total amount of max. 50 Battery's. There's a option to show a total amount of 100 Battery's, but this only works for wide LCDs,
        // on 1x1 LCD will be then shown just the half information! if you use Wide LCDs, then set WideLCD to true.
        bool WideLCD = false; // true = 50 Battery's, false = Show on wide LCD's up to 100 Battery's

        // this script change size of the Symbols depending of the amount.
        // it shows Large Symbols for an total amount of 1-10 Battery's on 1x1 LCDs, and a total amount of 1-20 Battery's on wide LCDs, all amount above will be shown
        // in small Symbols, to show always small Symbols set it to true
        bool OnlySmallSigns_Enabled = false; // true = always small symbols, false = depending on amount

        // Self updating System
        // thanks to SpaceEngeneers Update 1.185.1, we are able to use a new system, no need for timer,
        // false = you need an activation
        bool SelfUpdatingSys_Enabled = true; // false = Off, true = after compile script Selfupdating starts


        //if Self updating System enabled you can choose how many times per second the script will be activated
        int SelfUpSys_perSecond = 2; // 1 = 1 sec, 2 = 2 sec etc.

        //-----   Other Settings    -----

        // Shows Title on top of the LCD / Text Panel
        // "BATTERY STATUS" as title on Top, if disabled it leaves it space black
        bool BatteryTitle_Enabled = true; // true = show title, false = no title

        //there are three lines as border, you can deaktivate each of them of you want, true = on, false = off
        bool Underline_1_Enabled = true; // Show Underline below Title
        bool BatSpaceline_Enabled = true; // Show Middle line between Battery Amount and Stored Energy
        bool Underline_2_Enabled = true; // Show Underline Below Battery Amount and Stored Energy

        // Show Amount of Battery's displayed
        bool BatteryAmountEnabled = true; // true = Show Amount, false = off

        // Show total amount of all Stored Energy in Wh, kWh, MWh or GWh, Units depending on amount of Energy
        bool BatteryAllStoredEnergyEnabled = true; // true = Show stored Energy, false = off

        // Show Status of Power Input with an small Flash-Symbol
        // if true, when no Power input = red, Reactor online = Green, Solar Panel online and no reactor online = Cyan
        // Power Input below needed supply = Orange
        bool PowerInput_Enabled = true; // true = Show Power Input supply, Flash-Symbol changes color for different states, false = off

        // LCD Brightness
        // 0-255, 0 = dark, 255 = Bright
        int LCDbright = 255;
        /*
        # 	Kown Issues:
        # 	after the small Update from 20.11.17 there is a Bug...
        # 	-	if you placeing or deleting a SolarPanel, it stops the script, recompile and run it again
        #
        # 	General Thanks to the Community, to all that share their knowledge, that helped me a lot to make this
        # 	scripts working. Thanks for that.
        */


        //Dont touch the script below, to prevent errors
        //################################################################################
        public Program()
        {
            if (SelfUpdatingSys_Enabled)
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
        }

        public void Save()
        {
            // empty
        }

        int SelfUpSysCounttimes = 5;
        int SelfUpSysCounter = 0;

        // Main Script Start
        public void Main(string argument, UpdateType updateSource)
        {

            bool Run_ThisScript = false;
            if (SelfUpdatingSys_Enabled)
            {
                if (SelfUpSysCounter == 0)
                {
                    SelfUpSysCounter = SelfUpSysCounttimes * SelfUpSys_perSecond;
                    Run_ThisScript = true;
                }
                if (!Run_ThisScript)
                {
                    SelfUpSysCounter -= 1;
                }
            }
            else
            {
                Run_ThisScript = true;
            }

            if (Run_ThisScript)
            {

                string echotext = "\nBattery Status script:\nby Lightwolf\n\nSelf Updating every " + SelfUpSys_perSecond + " Seconds\n";

                Echo(echotext);

                string batteryStoredUnit = "kWh";
                bool OnlyNameTag = false;

                //Pixel Tempelates
                string P1 = "";
                string P2 = "";
                string P3 = "";
                string P4 = "";
                string P7 = "";
                string P8 = "";
                string P9 = "";
                string P10 = "";
                string P11 = "";
                string P17 = P10 + P7;
                string P18 = P10 + P8;
                string P28 = P10 + P10 + P7;
                string P38 = P10 + P10 + P10 + P8;
                string PxFull = "";
                string Breakli = "\n";
                string underli_A = "";
                string underli_B = "";
                //Space		########################################
                string li035 = "";
                string li036 = "";

                string li037 = ""; string li038 = ""; string li039 = "";
                string li040 = ""; string li041 = ""; string li042 = ""; string li043 = ""; string li044 = ""; string li045 = ""; string li046 = ""; string li047 = ""; string li048 = ""; string li049 = "";
                string li050 = ""; string li051 = ""; string li052 = ""; string li053 = ""; string li054 = ""; string li055 = ""; string li056 = ""; string li057 = ""; string li058 = ""; string li059 = "";
                string li060 = ""; string li061 = ""; string li062 = ""; string li063 = ""; string li064 = ""; string li065 = ""; string li066 = ""; string li067 = ""; string li068 = ""; string li069 = "";
                string li070 = ""; string li071 = ""; string li072 = ""; string li073 = ""; string li074 = ""; string li075 = ""; string li076 = ""; string li077 = ""; string li078 = ""; string li079 = "";
                string li080 = ""; string li081 = ""; string li082 = ""; string li083 = ""; string li084 = ""; string li085 = ""; string li086 = ""; string li087 = ""; string li088 = "";

                string li089 = ""; string li090 = ""; string li091 = ""; string li092 = ""; string li093 = "";

                string li094 = ""; string li095 = ""; string li096 = ""; string li097 = ""; string li098 = ""; string li099 = ""; string li100 = "";
                string li101 = ""; string li102 = ""; string li103 = ""; string li104 = ""; string li105 = ""; string li106 = ""; string li107 = ""; string li108 = ""; string li109 = "";
                string li110 = ""; string li111 = ""; string li112 = ""; string li113 = ""; string li114 = ""; string li115 = ""; string li116 = ""; string li117 = ""; string li118 = ""; string li119 = "";
                string li120 = ""; string li121 = ""; string li122 = ""; string li123 = ""; string li124 = ""; string li125 = ""; string li126 = ""; string li127 = ""; string li128 = ""; string li129 = "";
                string li130 = ""; string li131 = ""; string li132 = ""; string li133 = ""; string li134 = ""; string li135 = ""; string li136 = ""; string li137 = ""; string li138 = ""; string li139 = "";
                string li140 = ""; string li141 = ""; string li142 = ""; string li143 = ""; string li144 = ""; string li145 = ""; string li146 = ""; string li147 = ""; string li148 = ""; string li149 = "";
                string li150 = ""; string li151 = ""; string li152 = ""; string li153 = ""; string li154 = ""; string li155 = ""; string li156 = ""; string li157 = ""; string li158 = ""; string li159 = "";
                string li160 = ""; string li161 = ""; string li162 = ""; string li163 = ""; string li164 = ""; string li165 = ""; string li166 = ""; string li167 = ""; string li168 = ""; string li169 = "";
                string li170 = ""; string li171 = ""; string li172 = ""; string li173 = ""; string li174 = ""; string li175 = ""; string li176 = ""; string li177 = ""; string li178 = "";

                //Battery Status	########################################

                // Create list with all Batterys attached/added
                var batteryList = new List<IMyBatteryBlock>(); //create new empty list
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteryList); //put all Batterys in this list
                Echo("Batteries: " + batteryList.Count);

                float battery_X_MaxOutput_KW_All = 0f; // create empty Val
                float battery_X_CurrentOutput_KW_All = 0f; // create empty Val
                float battery_X_MaxInput_KW_All = 0f; // create empty Val
                float batt_X_CurrInput_KW_All = 0f; // create empty Val
                float batt_X_MaxStored_KW_All = 0f; // create empty Val
                float batX_CurrStored_KW_All = 0f; // create empty Val
                int battAmountActualloop = 0;
                int battAmountCount = batteryList.Count;
                float batt_X_CurrentStored_float_X = 0f;
                bool OnlySmallSigns = false;

                int WideMaxValue = 10;
                int WideMinValue = 11;
                if (WideLCD)
                {
                    WideMaxValue = 20;
                    WideMinValue = 21;
                }

                if (OnlySmallSigns_Enabled) { OnlySmallSigns = true; }
                else if (battAmountCount > WideMaxValue) { OnlySmallSigns = true; }

                string pz = P2;
                if (OnlySmallSigns)
                {
                    li035 = pz; li036 = pz; li037 = pz; li038 = pz; li039 = pz; li040 = pz; li041 = pz; li042 = pz; li043 = pz; li044 = pz; li045 = pz; li046 = pz; li047 = pz; li048 = pz;
                    li049 = pz; li050 = pz; li051 = pz; li052 = pz; li053 = pz; li054 = pz; li055 = pz; li056 = pz; li057 = pz; li058 = pz; li059 = pz; li060 = pz; li061 = pz;
                    li064 = pz; li065 = pz; li066 = pz; li067 = pz; li068 = pz; li069 = pz; li070 = pz; li071 = pz; li072 = pz; li073 = pz; li074 = pz; li075 = pz; li076 = pz; li077 = pz;
                    li078 = pz; li079 = pz; li080 = pz; li081 = pz; li082 = pz; li083 = pz; li084 = pz; li085 = pz; li086 = pz; li087 = pz; li088 = pz; li089 = pz; li090 = pz;
                    li093 = pz; li094 = pz; li095 = pz; li096 = pz; li097 = pz; li098 = pz; li099 = pz; li100 = pz; li101 = pz; li102 = pz; li103 = pz; li104 = pz; li105 = pz; li106 = pz;
                    li107 = pz; li108 = pz; li109 = pz; li110 = pz; li111 = pz; li112 = pz; li113 = pz; li114 = pz; li115 = pz; li116 = pz; li117 = pz; li118 = pz; li119 = pz;
                    li122 = pz; li123 = pz; li124 = pz; li125 = pz; li126 = pz; li127 = pz; li128 = pz; li129 = pz; li130 = pz; li131 = pz; li132 = pz; li133 = pz; li134 = pz; li135 = pz;
                    li136 = pz; li137 = pz; li138 = pz; li139 = pz; li140 = pz; li141 = pz; li142 = pz; li143 = pz; li144 = pz; li145 = pz; li146 = pz; li147 = pz; li148 = pz;
                    li151 = pz; li152 = pz; li153 = pz; li154 = pz; li155 = pz; li156 = pz; li157 = pz; li158 = pz; li159 = pz; li160 = pz; li161 = pz; li162 = pz; li163 = pz; li164 = pz;
                    li165 = pz; li166 = pz; li167 = pz; li168 = pz; li169 = pz; li170 = pz; li171 = pz; li172 = pz; li173 = pz; li174 = pz; li175 = pz; li176 = pz; li177 = pz;
                }

                //for (int i = 0; i < batteryList.Count; i++)
                foreach (var battery in batteryList)
                {
                    OnlyNameTag = false;
                    if (OnlyBatteryWithNameTag)
                    {
                        if (battery.CustomName.Contains(BatteryNameTag)) { OnlyNameTag = true; }
                    }

                    // replacement for the string operations
                    float batterylist_X_MaxOutput_float_X = battery.MaxOutput * 1000f;
                    float battery_X_MaxInput_float_X = battery.MaxInput * 1000f;
                    float batt_X_MaxStored_float_X = battery.MaxStoredPower * 1000f;
                    float batt_X_CurrInput_float_X = battery.CurrentInput * 1000f;
                    float batListX_CurrOutput_float_X = battery.CurrentOutput * 1000f;
                    batt_X_CurrentStored_float_X = battery.CurrentStoredPower * 1000f;

                    battery_X_MaxOutput_KW_All += batterylist_X_MaxOutput_float_X;
                    battery_X_MaxInput_KW_All += battery_X_MaxInput_float_X;
                    batt_X_MaxStored_KW_All += batt_X_MaxStored_float_X;
                    if (OnlyBatteryWithNameTag)
                    {
                        if (OnlyNameTag)
                        {
                            batt_X_CurrInput_KW_All += batt_X_CurrInput_float_X;
                            batX_CurrStored_KW_All += batt_X_CurrentStored_float_X;
                        }
                    }
                    else
                    {
                        batt_X_CurrInput_KW_All += batt_X_CurrInput_float_X;
                        batX_CurrStored_KW_All += batt_X_CurrentStored_float_X;
                    }
                    battery_X_CurrentOutput_KW_All += batListX_CurrOutput_float_X;

                    //Get Battery ST in percent
                    float BatLevel = batt_X_MaxStored_float_X / 6; //Battery Level 1 percent
                    float BatLevel_1 = BatLevel; // 1 Bar
                    float BatLevel_2 = BatLevel * 2; // 2 Bar
                    float BatLevel_3 = BatLevel * 3; // 3 Bar
                    float BatLevel_4 = BatLevel * 4; // 4 Bar
                    float BatLevel_5 = BatLevel * 5; // 5 Bar
                    float BatLevel_6 = BatLevel * 6; // 6 Bar
                                                        //BatteST Val empty
                    string Py = P17;
                    string BB_Gr = ""; string BB_Cyan = ""; string BB_Ye = ""; string BB_Or = ""; string BB_Re = "";

                    if (OnlySmallSigns) { Py = P11; BB_Gr = ""; BB_Cyan = ""; BB_Ye = ""; BB_Or = ""; BB_Re = ""; }
                    string BBST01 = Py; string BBST02 = Py; string BBST03 = Py; string BBST04 = Py; string BBST05 = Py; string BBST06 = Py; string BBST07 = Py; string BBST08 = Py; string BBST09 = Py;
                    string BBST10 = Py; string BBST11 = Py; string BBST12 = Py; string BBST13 = Py; string BBST14 = Py; string BBST15 = Py; string BBST16 = Py; string BBST17 = Py; string BBST18 = Py; string BBST19 = Py;
                    string BBST20 = Py; string BBST21 = Py; string BBST22 = Py; string BBST23 = Py; string BBST24 = Py; string BBST25 = Py; string BBST26 = Py; string BBST27 = Py; string BBST28 = Py; string BBST29 = Py;
                    string BBST30 = Py; string BBST31 = Py; string BBST32 = Py; string BBST33 = Py; string BBST34 = Py; string BBST35 = Py; string BBST36 = Py; string BBST37 = Py; string BBST38 = Py; string BBST39 = Py;

                    bool batt_IsFunctional = false;
                    bool batt_IsCharging = false;
                    bool batt_OnlyRecharge = false;
                    bool batt_OnlyDischarge = false;
                    bool batt_IsOff = false;

                    if (battery.Enabled)
                    {
                        batt_IsOff = true;
                        if (battery.IsFunctional)
                        {
                            if (battery.IsWorking)
                            {
                                batt_IsFunctional = true;
                                if (battery.IsCharging) { batt_IsCharging = true; }
                                else if (battery.ChargeMode == ChargeMode.Recharge) { batt_OnlyRecharge = true; }
                                else if (battery.ChargeMode == ChargeMode.Discharge) { batt_OnlyDischarge = true; }
                            }
                        }
                    }
                    else if (battery.IsFunctional) { batt_IsFunctional = true; }

                    bool Level_on = false;
                    //if batt Charging
                    if (batt_IsCharging) { Level_on = true; }
                    //if batt OnlyRecharge
                    else if (batt_OnlyRecharge)
                    {
                        if (OnlySmallSigns)
                        {
                            BBST01 = P11; BBST02 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11;
                            BBST07 = ""; BBST08 = ""; BBST09 = ""; BBST10 = ""; BBST11 = ""; BBST12 = ""; BBST13 = "";
                            BBST14 = ""; BBST15 = ""; BBST16 = ""; BBST17 = "";
                            BBST18 = P11; BBST19 = P11; BBST20 = P11; BBST21 = P11; BBST22 = P11; BBST23 = P11; BBST24 = P11;
                        }
                        else
                        {
                            BBST10 = ""; BBST11 = ""; BBST12 = ""; BBST13 = ""; BBST14 = ""; BBST15 = "";
                            BBST16 = ""; BBST17 = ""; BBST18 = ""; BBST19 = ""; BBST20 = ""; BBST21 = "";
                            BBST22 = ""; BBST23 = ""; BBST24 = ""; BBST25 = ""; BBST26 = ""; BBST27 = ""; BBST28 = "";
                        }
                        //if batt OnlyDischarge
                    }
                    else if (batt_OnlyDischarge) { Level_on = true; }
                    //if batt Offline
                    else if (!batt_IsOff) { Level_on = true; }
                    //if batt Not Functional
                    else if (!batt_IsFunctional)
                    {
                        if (OnlySmallSigns)
                        {
                            BBST01 = P11; BBST02 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11;
                            BBST07 = ""; BBST08 = ""; BBST09 = ""; BBST10 = ""; BBST11 = ""; BBST12 = "";
                            BBST13 = ""; BBST14 = ""; BBST15 = ""; BBST16 = ""; BBST17 = "";
                            BBST18 = P11; BBST19 = P11; BBST20 = P11; BBST21 = P11; BBST22 = P11; BBST23 = P11; BBST24 = P11;
                        }
                        else
                        {
                            BBST10 = ""; BBST11 = ""; BBST12 = ""; BBST13 = ""; BBST14 = ""; BBST15 = ""; BBST16 = "";
                            BBST17 = ""; BBST18 = ""; BBST19 = ""; BBST20 = ""; BBST21 = ""; BBST22 = ""; BBST23 = "";
                            BBST24 = ""; BBST25 = ""; BBST26 = ""; BBST27 = ""; BBST28 = "";
                            BBST29 = P17; BBST30 = P17; BBST31 = P17; BBST32 = P17; BBST33 = P17; BBST34 = P17; BBST35 = P17; BBST36 = P17; BBST37 = P17; BBST38 = P17; BBST39 = P17;
                        }
                    }
                    else { Level_on = true; }

                    if (batt_IsFunctional)
                    {
                        if (Level_on)
                        {
                            if (batt_X_CurrentStored_float_X >= BatLevel_6)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = BB_Gr; BBST03 = BB_Gr; BBST04 = P11; BBST05 = P11; BBST06 = BB_Gr; BBST07 = BB_Gr; BBST08 = P11; BBST09 = P11; BBST10 = BB_Gr; BBST11 = BB_Gr; BBST12 = P11;
                                    BBST13 = P11; BBST14 = BB_Gr; BBST15 = BB_Gr; BBST16 = P11; BBST17 = P11; BBST18 = BB_Gr; BBST19 = BB_Gr; BBST20 = P11; BBST21 = P11; BBST22 = BB_Gr; BBST23 = BB_Gr; BBST24 = P11;
                                }
                                else
                                {
                                    BBST01 = BB_Gr; BBST02 = BB_Gr; BBST03 = BB_Gr; BBST04 = BB_Gr;
                                    BBST08 = BB_Gr; BBST09 = BB_Gr; BBST10 = BB_Gr; BBST11 = BB_Gr;
                                    BBST15 = BB_Gr; BBST16 = BB_Gr; BBST17 = BB_Gr; BBST18 = BB_Gr;
                                    BBST22 = BB_Gr; BBST23 = BB_Gr; BBST24 = BB_Gr; BBST25 = BB_Gr;
                                    BBST29 = BB_Gr; BBST30 = BB_Gr; BBST31 = BB_Gr; BBST32 = BB_Gr;
                                    BBST36 = BB_Gr; BBST37 = BB_Gr; BBST38 = BB_Gr; BBST39 = BB_Gr;
                                }
                            }
                            else if (batt_X_CurrentStored_float_X >= BatLevel_5)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = BB_Cyan; BBST07 = BB_Cyan; BBST08 = P11; BBST09 = P11; BBST10 = BB_Cyan; BBST11 = BB_Cyan; BBST12 = P11;
                                    BBST13 = P11; BBST14 = BB_Cyan; BBST15 = BB_Cyan; BBST16 = P11; BBST17 = P11; BBST18 = BB_Cyan; BBST19 = BB_Cyan; BBST20 = P11; BBST21 = P11; BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = P11;
                                }
                                else
                                {
                                    BBST08 = BB_Cyan; BBST09 = BB_Cyan; BBST10 = BB_Cyan; BBST11 = BB_Cyan;
                                    BBST15 = BB_Cyan; BBST16 = BB_Cyan; BBST17 = BB_Cyan; BBST18 = BB_Cyan;
                                    BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = BB_Cyan; BBST25 = BB_Cyan;
                                    BBST29 = BB_Cyan; BBST30 = BB_Cyan; BBST31 = BB_Cyan; BBST32 = BB_Cyan;
                                    BBST36 = BB_Cyan; BBST37 = BB_Cyan; BBST38 = BB_Cyan; BBST39 = BB_Cyan;
                                }
                            }
                            else if (batt_X_CurrentStored_float_X >= BatLevel_4)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11; BBST07 = P11; BBST08 = P11; BBST09 = P11; BBST10 = BB_Cyan; BBST11 = BB_Cyan; BBST12 = P11; BBST13 = P11;
                                    BBST14 = BB_Cyan; BBST15 = BB_Cyan; BBST16 = P11; BBST17 = P11; BBST18 = BB_Cyan; BBST19 = BB_Cyan; BBST20 = P11; BBST21 = P11; BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = P11;
                                }
                                else
                                {
                                    BBST15 = BB_Cyan; BBST16 = BB_Cyan; BBST17 = BB_Cyan; BBST18 = BB_Cyan;
                                    BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = BB_Cyan; BBST25 = BB_Cyan;
                                    BBST29 = BB_Cyan; BBST30 = BB_Cyan; BBST31 = BB_Cyan; BBST32 = BB_Cyan;
                                    BBST36 = BB_Cyan; BBST37 = BB_Cyan; BBST38 = BB_Cyan; BBST39 = BB_Cyan;
                                }
                            }
                            else if (batt_X_CurrentStored_float_X >= BatLevel_3)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11; BBST07 = P11; BBST08 = P11; BBST09 = P11; BBST10 = P11; BBST11 = P11; BBST12 = P11; BBST13 = P11;
                                    BBST14 = BB_Ye; BBST15 = BB_Ye; BBST16 = P11; BBST17 = P11; BBST18 = BB_Cyan; BBST19 = BB_Cyan; BBST20 = P11; BBST21 = P11; BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = P11;
                                }
                                else
                                {
                                    BBST22 = BB_Ye; BBST23 = BB_Ye; BBST24 = BB_Ye; BBST25 = BB_Ye;
                                    BBST29 = BB_Cyan; BBST30 = BB_Cyan; BBST31 = BB_Cyan; BBST32 = BB_Cyan;
                                    BBST36 = BB_Cyan; BBST37 = BB_Cyan; BBST38 = BB_Cyan; BBST39 = BB_Cyan;
                                }
                            }
                            else if (batt_X_CurrentStored_float_X >= BatLevel_2)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11; BBST07 = P11; BBST08 = P11; BBST09 = P11; BBST10 = P11; BBST11 = P11; BBST12 = P11; BBST13 = P11;
                                    BBST14 = P11; BBST15 = P11; BBST16 = P11; BBST17 = P11; BBST18 = BB_Or; BBST19 = BB_Or; BBST20 = P11; BBST21 = P11; BBST22 = BB_Cyan; BBST23 = BB_Cyan; BBST24 = P11;
                                }
                                else
                                {
                                    BBST29 = BB_Or; BBST30 = BB_Or; BBST31 = BB_Or; BBST32 = BB_Or;
                                    BBST36 = BB_Cyan; BBST37 = BB_Cyan; BBST38 = BB_Cyan; BBST39 = BB_Cyan;
                                }
                            }
                            else if (batt_X_CurrentStored_float_X >= BatLevel_1)
                            {
                                if (OnlySmallSigns)
                                {
                                    BBST01 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11; BBST07 = P11; BBST08 = P11; BBST09 = P11; BBST10 = P11; BBST11 = P11; BBST12 = P11;
                                    BBST13 = P11; BBST14 = P11; BBST15 = P11; BBST16 = P11; BBST17 = P11; BBST18 = P11; BBST19 = P11; BBST20 = P11; BBST21 = P11; BBST22 = BB_Re; BBST23 = BB_Re; BBST24 = P11;
                                }
                                else
                                {
                                    BBST36 = BB_Re; BBST37 = BB_Re; BBST38 = BB_Re; BBST39 = BB_Re;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (OnlySmallSigns)
                        {
                            BBST01 = P11; BBST02 = P11; BBST02 = P11; BBST03 = P11; BBST04 = P11; BBST05 = P11; BBST06 = P11;
                            BBST07 = ""; BBST08 = ""; BBST09 = ""; BBST10 = ""; BBST11 = ""; BBST12 = "";
                            BBST13 = ""; BBST14 = ""; BBST15 = ""; BBST16 = ""; BBST17 = "";
                            BBST18 = P11; BBST19 = P11; BBST20 = P11; BBST21 = P11; BBST22 = P11; BBST23 = P11; BBST24 = P11;
                        }
                        else
                        {
                            BBST10 = ""; BBST11 = ""; BBST12 = ""; BBST13 = ""; BBST14 = ""; BBST15 = ""; BBST16 = "";
                            BBST17 = ""; BBST18 = ""; BBST19 = ""; BBST20 = ""; BBST21 = ""; BBST22 = ""; BBST23 = "";
                            BBST24 = ""; BBST25 = ""; BBST26 = ""; BBST27 = ""; BBST28 = "";
                            BBST29 = P17; BBST30 = P17; BBST31 = P17; BBST32 = P17; BBST33 = P17; BBST34 = P17; BBST35 = P17; BBST36 = P17; BBST37 = P17; BBST38 = P17; BBST39 = P17;
                        }
                    }

                    // Fuse batt Sign together	########################################
                    string BC_037 = ""; string BG_037 = ""; string BR_037 = ""; string Bo_037 = "";
                    string BC_038 = ""; string BG_038 = ""; string BR_038 = ""; string Bo_038 = "";
                    string BC_039 = ""; string BG_039 = ""; string BR_039 = ""; string Bo_039 = "";
                    string BC_040 = ""; string BG_040 = ""; string BR_040 = ""; string Bo_040 = "";
                    string BC_041 = ""; string BG_041 = ""; string BR_041 = ""; string Bo_041 = "";
                    string BC_042 = ""; string BG_042 = ""; string BR_042 = ""; string Bo_042 = "";
                    string BC_043 = ""; string BG_043 = ""; string BR_043 = ""; string Bo_043 = "";

                    string BC_044 = "" + BBST01 + ""; string BG_044 = "" + BBST01 + ""; string BR_044 = "" + BBST01 + ""; string Bo_044 = "" + BBST01 + "";
                    string BC_045 = "" + BBST02 + ""; string BG_045 = "" + BBST02 + ""; string BR_045 = "" + BBST02 + ""; string Bo_045 = "" + BBST02 + "";
                    string BC_046 = "" + BBST03 + ""; string BG_046 = "" + BBST03 + ""; string BR_046 = "" + BBST03 + ""; string Bo_046 = "" + BBST03 + "";
                    string BC_047 = "" + BBST04 + ""; string BG_047 = "" + BBST04 + ""; string BR_047 = "" + BBST04 + ""; string Bo_047 = "" + BBST04 + "";
                    string BC_048 = "" + BBST05 + ""; string BG_048 = "" + BBST05 + ""; string BR_048 = "" + BBST05 + ""; string Bo_048 = "" + BBST05 + "";
                    string BC_049 = "" + BBST06 + ""; string BG_049 = "" + BBST06 + ""; string BR_049 = "" + BBST06 + ""; string Bo_049 = "" + BBST06 + "";
                    string BC_050 = "" + BBST07 + ""; string BG_050 = "" + BBST07 + ""; string BR_050 = "" + BBST07 + ""; string Bo_050 = "" + BBST07 + "";
                    string BC_051 = "" + BBST08 + ""; string BG_051 = "" + BBST08 + ""; string BR_051 = "" + BBST08 + ""; string Bo_051 = "" + BBST08 + "";
                    string BC_052 = "" + BBST09 + ""; string BG_052 = "" + BBST09 + ""; string BR_052 = "" + BBST09 + ""; string Bo_052 = "" + BBST09 + "";
                    string BC_053 = "" + BBST10 + ""; string BG_053 = "" + BBST10 + ""; string BR_053 = "" + BBST10 + ""; string Bo_053 = "" + BBST10 + "";
                    string BC_054 = "" + BBST11 + ""; string BG_054 = "" + BBST11 + ""; string BR_054 = "" + BBST11 + ""; string Bo_054 = "" + BBST11 + "";
                    string BC_055 = "" + BBST12 + ""; string BG_055 = "" + BBST12 + ""; string BR_055 = "" + BBST12 + ""; string Bo_055 = "" + BBST12 + "";
                    string BC_056 = "" + BBST13 + ""; string BG_056 = "" + BBST13 + ""; string BR_056 = "" + BBST13 + ""; string Bo_056 = "" + BBST13 + "";
                    string BC_057 = "" + BBST14 + ""; string BG_057 = "" + BBST14 + ""; string BR_057 = "" + BBST14 + ""; string Bo_057 = "" + BBST14 + "";
                    string BC_058 = "" + BBST15 + ""; string BG_058 = "" + BBST15 + ""; string BR_058 = "" + BBST15 + ""; string Bo_058 = "" + BBST15 + "";
                    string BC_059 = "" + BBST16 + ""; string BG_059 = "" + BBST16 + ""; string BR_059 = "" + BBST16 + ""; string Bo_059 = "" + BBST16 + "";
                    string BC_060 = "" + BBST17 + ""; string BG_060 = "" + BBST17 + ""; string BR_060 = "" + BBST17 + ""; string Bo_060 = "" + BBST17 + "";
                    string BC_061 = "" + BBST18 + ""; string BG_061 = "" + BBST18 + ""; string BR_061 = "" + BBST18 + ""; string Bo_061 = "" + BBST18 + "";
                    string BC_062 = "" + BBST19 + ""; string BG_062 = "" + BBST19 + ""; string BR_062 = "" + BBST19 + ""; string Bo_062 = "" + BBST19 + "";
                    string BC_063 = "" + BBST20 + ""; string BG_063 = "" + BBST20 + ""; string BR_063 = "" + BBST20 + ""; string Bo_063 = "" + BBST20 + "";
                    string BC_064 = "" + BBST21 + ""; string BG_064 = "" + BBST21 + ""; string BR_064 = "" + BBST21 + ""; string Bo_064 = "" + BBST21 + "";
                    string BC_065 = "" + BBST22 + ""; string BG_065 = "" + BBST22 + ""; string BR_065 = "" + BBST22 + ""; string Bo_065 = "" + BBST22 + "";
                    string BC_066 = "" + BBST23 + ""; string BG_066 = "" + BBST23 + ""; string BR_066 = "" + BBST23 + ""; string Bo_066 = "" + BBST23 + "";
                    string BC_067 = "" + BBST24 + ""; string BG_067 = "" + BBST24 + ""; string BR_067 = "" + BBST24 + ""; string Bo_067 = "" + BBST24 + "";
                    string BC_068 = "" + BBST25 + ""; string BG_068 = "" + BBST25 + ""; string BR_068 = "" + BBST25 + ""; string Bo_068 = "" + BBST25 + "";
                    string BC_069 = "" + BBST26 + ""; string BG_069 = "" + BBST26 + ""; string BR_069 = "" + BBST26 + ""; string Bo_069 = "" + BBST26 + "";
                    string BC_070 = "" + BBST27 + ""; string BG_070 = "" + BBST27 + ""; string BR_070 = "" + BBST27 + ""; string Bo_070 = "" + BBST27 + "";
                    string BC_071 = "" + BBST28 + ""; string BG_071 = "" + BBST28 + ""; string BR_071 = "" + BBST28 + ""; string Bo_071 = "" + BBST28 + "";
                    string BC_072 = "" + BBST29 + ""; string BG_072 = "" + BBST29 + ""; string BR_072 = "" + BBST29 + ""; string Bo_072 = "" + BBST29 + "";
                    string BC_073 = "" + BBST30 + ""; string BG_073 = "" + BBST30 + ""; string BR_073 = "" + BBST30 + ""; string Bo_073 = "" + BBST30 + "";
                    string BC_074 = "" + BBST31 + ""; string BG_074 = "" + BBST31 + ""; string BR_074 = "" + BBST31 + ""; string Bo_074 = "" + BBST31 + "";
                    string BC_075 = "" + BBST32 + ""; string BG_075 = "" + BBST32 + ""; string BR_075 = "" + BBST32 + ""; string Bo_075 = "" + BBST32 + "";
                    string BC_076 = "" + BBST33 + ""; string BG_076 = "" + BBST33 + ""; string BR_076 = "" + BBST33 + ""; string Bo_076 = "" + BBST33 + "";
                    string BC_077 = "" + BBST34 + ""; string BG_077 = "" + BBST34 + ""; string BR_077 = "" + BBST34 + ""; string Bo_077 = "" + BBST34 + "";
                    string BC_078 = "" + BBST35 + ""; string BG_078 = "" + BBST35 + ""; string BR_078 = "" + BBST35 + ""; string Bo_078 = "" + BBST35 + "";
                    string BC_079 = "" + BBST36 + ""; string BG_079 = "" + BBST36 + ""; string BR_079 = "" + BBST36 + ""; string Bo_079 = "" + BBST36 + "";
                    string BC_080 = "" + BBST37 + ""; string BG_080 = "" + BBST37 + ""; string BR_080 = "" + BBST37 + ""; string Bo_080 = "" + BBST37 + "";
                    string BC_081 = "" + BBST38 + ""; string BG_081 = "" + BBST38 + ""; string BR_081 = "" + BBST38 + ""; string Bo_081 = "" + BBST38 + "";
                    string BC_082 = "" + BBST39 + ""; string BG_082 = "" + BBST39 + ""; string BR_082 = "" + BBST39 + ""; string Bo_082 = "" + BBST39 + "";
                    string BC_083 = ""; string BG_083 = ""; string BR_083 = ""; string Bo_083 = "";
                    string BC_084 = ""; string BG_084 = ""; string BR_084 = ""; string Bo_084 = "";
                    string BC_085 = ""; string BG_085 = ""; string BR_085 = ""; string Bo_085 = "";
                    string BC_086 = ""; string BG_086 = ""; string BR_086 = ""; string Bo_086 = "";
                    string BC_087 = ""; string BG_087 = ""; string BR_087 = ""; string Bo_087 = "";
                    string BC_088 = ""; string BG_088 = ""; string BR_088 = ""; string Bo_088 = "";
                    string pC = ""; string pG = ""; string pR = ""; string pO = "";
                    if (OnlySmallSigns)
                    {
                        BC_037 = ""; BG_037 = ""; BR_037 = ""; Bo_037 = "";
                        BC_038 = ""; BG_038 = ""; BR_038 = ""; Bo_038 = "";
                        BC_039 = pC + BBST01 + pC; BG_039 = pG + BBST01 + pG; BR_039 = pR + BBST01 + pR; Bo_039 = pO + BBST01 + pO;
                        BC_040 = pC + BBST02 + pC; BG_040 = pG + BBST02 + pG; BR_040 = pR + BBST02 + pR; Bo_040 = pO + BBST02 + pO;
                        BC_041 = pC + BBST03 + pC; BG_041 = pG + BBST03 + pG; BR_041 = pR + BBST03 + pR; Bo_041 = pO + BBST03 + pO;
                        BC_042 = pC + BBST04 + pC; BG_042 = pG + BBST04 + pG; BR_042 = pR + BBST04 + pR; Bo_042 = pO + BBST04 + pO;
                        BC_043 = pC + BBST05 + pC; BG_043 = pG + BBST05 + pG; BR_043 = pR + BBST05 + pR; Bo_043 = pO + BBST05 + pO;
                        BC_044 = pC + BBST06 + pC; BG_044 = pG + BBST06 + pG; BR_044 = pR + BBST06 + pR; Bo_044 = pO + BBST06 + pO;
                        BC_045 = pC + BBST07 + pC; BG_045 = pG + BBST07 + pG; BR_045 = pR + BBST07 + pR; Bo_045 = pO + BBST07 + pO;
                        BC_046 = pC + BBST08 + pC; BG_046 = pG + BBST08 + pG; BR_046 = pR + BBST08 + pR; Bo_046 = pO + BBST08 + pO;
                        BC_047 = pC + BBST09 + pC; BG_047 = pG + BBST09 + pG; BR_047 = pR + BBST09 + pR; Bo_047 = pO + BBST09 + pO;
                        BC_048 = pC + BBST10 + pC; BG_048 = pG + BBST10 + pG; BR_048 = pR + BBST10 + pR; Bo_048 = pO + BBST10 + pO;
                        BC_049 = pC + BBST11 + pC; BG_049 = pG + BBST11 + pG; BR_049 = pR + BBST11 + pR; Bo_049 = pO + BBST11 + pO;
                        BC_050 = pC + BBST12 + pC; BG_050 = pG + BBST12 + pG; BR_050 = pR + BBST12 + pR; Bo_050 = pO + BBST12 + pO;
                        BC_051 = pC + BBST13 + pC; BG_051 = pG + BBST13 + pG; BR_051 = pR + BBST13 + pR; Bo_051 = pO + BBST13 + pO;
                        BC_052 = pC + BBST14 + pC; BG_052 = pG + BBST14 + pG; BR_052 = pR + BBST14 + pR; Bo_052 = pO + BBST14 + pO;
                        BC_053 = pC + BBST15 + pC; BG_053 = pG + BBST15 + pG; BR_053 = pR + BBST15 + pR; Bo_053 = pO + BBST15 + pO;
                        BC_054 = pC + BBST16 + pC; BG_054 = pG + BBST16 + pG; BR_054 = pR + BBST16 + pR; Bo_054 = pO + BBST16 + pO;
                        BC_055 = pC + BBST17 + pC; BG_055 = pG + BBST17 + pG; BR_055 = pR + BBST17 + pR; Bo_055 = pO + BBST17 + pO;
                        BC_056 = pC + BBST18 + pC; BG_056 = pG + BBST18 + pG; BR_056 = pR + BBST18 + pR; Bo_056 = pO + BBST18 + pO;
                        BC_057 = pC + BBST19 + pC; BG_057 = pG + BBST19 + pG; BR_057 = pR + BBST19 + pR; Bo_057 = pO + BBST19 + pO;
                        BC_058 = pC + BBST20 + pC; BG_058 = pG + BBST20 + pG; BR_058 = pR + BBST20 + pR; Bo_058 = pO + BBST20 + pO;
                        BC_059 = pC + BBST21 + pC; BG_059 = pG + BBST21 + pG; BR_059 = pR + BBST21 + pR; Bo_059 = pO + BBST21 + pO;
                        BC_060 = pC + BBST22 + pC; BG_060 = pG + BBST22 + pG; BR_060 = pR + BBST22 + pR; Bo_060 = pO + BBST22 + pO;
                        BC_061 = pC + BBST23 + pC; BG_061 = pG + BBST23 + pG; BR_061 = pR + BBST23 + pR; Bo_061 = pO + BBST23 + pO;
                        BC_062 = pC + BBST24 + pC; BG_062 = pG + BBST24 + pG; BR_062 = pR + BBST24 + pR; Bo_062 = pO + BBST24 + pO;
                        BC_063 = ""; BG_063 = ""; BR_063 = ""; Bo_063 = "";
                    }
                    string liX037 = ""; string liX038 = ""; string liX039 = "";
                    string liX040 = ""; string liX041 = ""; string liX042 = ""; string liX043 = ""; string liX044 = ""; string liX045 = ""; string liX046 = ""; string liX047 = ""; string liX048 = ""; string liX049 = "";
                    string liX050 = ""; string liX051 = ""; string liX052 = ""; string liX053 = ""; string liX054 = ""; string liX055 = ""; string liX056 = ""; string liX057 = ""; string liX058 = ""; string liX059 = "";
                    string liX060 = ""; string liX061 = ""; string liX062 = ""; string liX063 = ""; string liX064 = ""; string liX065 = ""; string liX066 = ""; string liX067 = ""; string liX068 = ""; string liX069 = "";
                    string liX070 = ""; string liX071 = ""; string liX072 = ""; string liX073 = ""; string liX074 = ""; string liX075 = ""; string liX076 = ""; string liX077 = ""; string liX078 = ""; string liX079 = "";
                    string liX080 = ""; string liX081 = ""; string liX082 = ""; string liX083 = ""; string liX084 = ""; string liX085 = ""; string liX086 = ""; string liX087 = ""; string liX088 = "";

                    string Px = "";
                    bool Cyan_on = false;
                    bool CheckToShow = false;

                    if (OnlyBatteryWithNameTag)
                    {
                        if (OnlyNameTag)
                        {
                            CheckToShow = true;
                        }
                    }
                    else
                    {
                        CheckToShow = true;
                    }

                    if (CheckToShow)
                    {
                        //if batt Charging
                        if (batt_IsCharging)
                        {
                            Cyan_on = true;
                        }
                        else if (batt_OnlyRecharge)
                        {
                            //if batt OnlyRecharge
                            if (OnlySmallSigns) { Px = P4; }
                            else { Px = P7; }
                            liX037 += Px + Bo_037; liX038 += Px + Bo_038; liX039 += Px + Bo_039; liX040 += Px + Bo_040; liX041 += Px + Bo_041; liX042 += Px + Bo_042; liX043 += Px + Bo_043;
                            liX044 += Px + Bo_044; liX045 += Px + Bo_045; liX046 += Px + Bo_046; liX047 += Px + Bo_047; liX048 += Px + Bo_048; liX049 += Px + Bo_049; liX050 += Px + Bo_050;
                            liX051 += Px + Bo_051; liX052 += Px + Bo_052; liX053 += Px + Bo_053; liX054 += Px + Bo_054; liX055 += Px + Bo_055; liX056 += Px + Bo_056; liX057 += Px + Bo_057;
                            liX058 += Px + Bo_058; liX059 += Px + Bo_059; liX060 += Px + Bo_060; liX061 += Px + Bo_061; liX062 += Px + Bo_062; liX063 += Px + Bo_063;
                            if (battAmountCount < WideMinValue)
                            {
                                liX064 += Px + Bo_064; liX065 += Px + Bo_065; liX066 += Px + Bo_066; liX067 += Px + Bo_067; liX068 += Px + Bo_068; liX069 += Px + Bo_069;
                                liX070 += Px + Bo_070; liX071 += Px + Bo_071; liX072 += Px + Bo_072; liX073 += Px + Bo_073; liX074 += Px + Bo_074; liX075 += Px + Bo_075; liX076 += Px + Bo_076; liX077 += Px + Bo_077; liX078 += Px + Bo_078; liX079 += Px + Bo_079;
                                liX080 += Px + Bo_080; liX081 += Px + Bo_081; liX082 += Px + Bo_082; liX083 += Px + Bo_083; liX084 += Px + Bo_084; liX085 += Px + Bo_085; liX086 += Px + Bo_086; liX087 += Px + Bo_087; liX088 += Px + Bo_088;
                            }
                        }
                        else if (batt_OnlyDischarge)
                        {
                            //if batt OnlyDischarge
                            if (OnlySmallSigns) { Px = P4; }
                            else { Px = P7; }
                            liX037 += Px + BG_037; liX038 += Px + BG_038; liX039 += Px + BG_039; liX040 += Px + BG_040; liX041 += Px + BG_041; liX042 += Px + BG_042; liX043 += Px + BG_043;
                            liX044 += Px + BG_044; liX045 += Px + BG_045; liX046 += Px + BG_046; liX047 += Px + BG_047; liX048 += Px + BG_048; liX049 += Px + BG_049; liX050 += Px + BG_050;
                            liX051 += Px + BG_051; liX052 += Px + BG_052; liX053 += Px + BG_053; liX054 += Px + BG_054; liX055 += Px + BG_055; liX056 += Px + BG_056; liX057 += Px + BG_057;
                            liX058 += Px + BG_058; liX059 += Px + BG_059; liX060 += Px + BG_060; liX061 += Px + BG_061; liX062 += Px + BG_062; liX063 += Px + BG_063;
                            if (battAmountCount < WideMinValue)
                            {
                                liX064 += Px + BG_064; liX065 += Px + BG_065; liX066 += Px + BG_066; liX067 += Px + BG_067; liX068 += Px + BG_068; liX069 += Px + BG_069;
                                liX070 += Px + BG_070; liX071 += Px + BG_071; liX072 += Px + BG_072; liX073 += Px + BG_073; liX074 += Px + BG_074; liX075 += Px + BG_075; liX076 += Px + BG_076; liX077 += Px + BG_077; liX078 += Px + BG_078; liX079 += Px + BG_079;
                                liX080 += Px + BG_080; liX081 += Px + BG_081; liX082 += Px + BG_082; liX083 += Px + BG_083; liX084 += Px + BG_084; liX085 += Px + BG_085; liX086 += Px + BG_086; liX087 += Px + BG_087; liX088 += Px + BG_088;
                            }
                        }
                        else if (!batt_IsFunctional)
                        {
                            //if batt Not Functional
                            if (OnlySmallSigns) { Px = P4; }
                            else { Px = P7; }
                            liX037 += Px + BR_037; liX038 += Px + BR_038; liX039 += Px + BR_039; liX040 += Px + BR_040; liX041 += Px + BR_041; liX042 += Px + BR_042; liX043 += Px + BR_043;
                            liX044 += Px + BR_044; liX045 += Px + BR_045; liX046 += Px + BR_046; liX047 += Px + BR_047; liX048 += Px + BR_048; liX049 += Px + BR_049; liX050 += Px + BR_050;
                            liX051 += Px + BR_051; liX052 += Px + BR_052; liX053 += Px + BR_053; liX054 += Px + BR_054; liX055 += Px + BR_055; liX056 += Px + BR_056; liX057 += Px + BR_057;
                            liX058 += Px + BR_058; liX059 += Px + BR_059; liX060 += Px + BR_060; liX061 += Px + BR_061; liX062 += Px + BR_062; liX063 += Px + BR_063;
                            if (battAmountCount < WideMinValue)
                            {
                                liX064 += Px + BR_064; liX065 += Px + BR_065; liX066 += Px + BR_066; liX067 += Px + BR_067; liX068 += Px + BR_068; liX069 += Px + BR_069;
                                liX070 += Px + BR_070; liX071 += Px + BR_071; liX072 += Px + BR_072; liX073 += Px + BR_073; liX074 += Px + BR_074; liX075 += Px + BR_075; liX076 += Px + BR_076; liX077 += Px + BR_077; liX078 += Px + BR_078; liX079 += Px + BR_079;
                                liX080 += Px + BR_080; liX081 += Px + BR_081; liX082 += Px + BR_082; liX083 += Px + BR_083; liX084 += Px + BR_084; liX085 += Px + BR_085; liX086 += Px + BR_086; liX087 += Px + BR_087; liX088 += Px + BR_088;
                            }
                        }
                        else if (!batt_IsOff)
                        {
                            //if batt OffliX
                            if (OnlySmallSigns) { Px = P4; }
                            else { Px = P7; }
                            liX037 += Px + BR_037; liX038 += Px + BR_038; liX039 += Px + BR_039; liX040 += Px + BR_040; liX041 += Px + BR_041; liX042 += Px + BR_042; liX043 += Px + BR_043;
                            liX044 += Px + BR_044; liX045 += Px + BR_045; liX046 += Px + BR_046; liX047 += Px + BR_047; liX048 += Px + BR_048; liX049 += Px + BR_049; liX050 += Px + BR_050;
                            liX051 += Px + BR_051; liX052 += Px + BR_052; liX053 += Px + BR_053; liX054 += Px + BR_054; liX055 += Px + BR_055; liX056 += Px + BR_056; liX057 += Px + BR_057;
                            liX058 += Px + BR_058; liX059 += Px + BR_059; liX060 += Px + BR_060; liX061 += Px + BR_061; liX062 += Px + BR_062; liX063 += Px + BR_063;
                            if (battAmountCount < WideMinValue)
                            {
                                liX064 += Px + BR_064; liX065 += Px + BR_065; liX066 += Px + BR_066; liX067 += Px + BR_067; liX068 += Px + BR_068; liX069 += Px + BR_069;
                                liX070 += Px + BR_070; liX071 += Px + BR_071; liX072 += Px + BR_072; liX073 += Px + BR_073; liX074 += Px + BR_074; liX075 += Px + BR_075; liX076 += Px + BR_076; liX077 += Px + BR_077; liX078 += Px + BR_078; liX079 += Px + BR_079;
                                liX080 += Px + BR_080; liX081 += Px + BR_081; liX082 += Px + BR_082; liX083 += Px + BR_083; liX084 += Px + BR_084; liX085 += Px + BR_085; liX086 += Px + BR_086; liX087 += Px + BR_087; liX088 += Px + BR_088;
                            }
                        }
                        else
                        {
                            Cyan_on = true;
                        }

                        if (Cyan_on)
                        {
                            if (OnlySmallSigns) { Px = P4; }
                            else { Px = P7; }

                            liX037 += Px + BC_037; liX038 += Px + BC_038; liX039 += Px + BC_039; liX040 += Px + BC_040; liX041 += Px + BC_041; liX042 += Px + BC_042; liX043 += Px + BC_043;
                            liX044 += Px + BC_044; liX045 += Px + BC_045; liX046 += Px + BC_046; liX047 += Px + BC_047; liX048 += Px + BC_048; liX049 += Px + BC_049; liX050 += Px + BC_050;
                            liX051 += Px + BC_051; liX052 += Px + BC_052; liX053 += Px + BC_053; liX054 += Px + BC_054; liX055 += Px + BC_055; liX056 += Px + BC_056; liX057 += Px + BC_057;
                            liX058 += Px + BC_058; liX059 += Px + BC_059; liX060 += Px + BC_060; liX061 += Px + BC_061; liX062 += Px + BC_062; liX063 += Px + BC_063;
                            if (battAmountCount < WideMinValue)
                            {
                                liX064 += Px + BC_064; liX065 += Px + BC_065; liX066 += Px + BC_066; liX067 += Px + BC_067; liX068 += Px + BC_068; liX069 += Px + BC_069;
                                liX070 += Px + BC_070; liX071 += Px + BC_071; liX072 += Px + BC_072; liX073 += Px + BC_073; liX074 += Px + BC_074; liX075 += Px + BC_075; liX076 += Px + BC_076; liX077 += Px + BC_077; liX078 += Px + BC_078; liX079 += Px + BC_079;
                                liX080 += Px + BC_080; liX081 += Px + BC_081; liX082 += Px + BC_082; liX083 += Px + BC_083; liX084 += Px + BC_084; liX085 += Px + BC_085; liX086 += Px + BC_086; liX087 += Px + BC_087; liX088 += Px + BC_088;
                            }
                        }
                        battAmountActualloop += 1;
                    }
                    int Am50_10To20 = 0;
                    int Am50_20To40 = 0;
                    int Am50_30To60 = 0;
                    int Am50_40To80 = 0;
                    int Am50_50To100 = 0;
                    int Am10_5To10 = 0;

                    if (WideLCD)
                    {
                        Am50_10To20 = 21;
                        Am50_20To40 = 41;
                        Am50_30To60 = 61;
                        Am50_40To80 = 81;
                        Am50_50To100 = 80;

                        Am10_5To10 = 11;
                    }
                    else
                    {
                        Am50_10To20 = 11;
                        Am50_20To40 = 21;
                        Am50_30To60 = 31;
                        Am50_40To80 = 41;
                        Am50_50To100 = 40;

                        Am10_5To10 = 6;
                    }

                    if (OnlySmallSigns)
                    {
                        // if batt Amount 19-50
                        if (battAmountActualloop < Am50_10To20)
                        {
                            //0-10
                            li035 += liX037; li036 += liX038; li037 += liX039; li038 += liX040; li039 += liX041; li040 += liX042; li041 += liX043; li042 += liX044; li043 += liX045; li044 += liX046;
                            li045 += liX047; li046 += liX048; li047 += liX049; li048 += liX050; li049 += liX051; li050 += liX052; li051 += liX053; li052 += liX054; li053 += liX055; li054 += liX056;
                            li055 += liX057; li056 += liX058; li057 += liX059; li058 += liX060; li059 += liX061; li060 += liX062; li061 += liX063;
                            li062 = PxFull;
                            li063 = PxFull;
                        }
                        else if (battAmountActualloop < Am50_20To40)
                        {
                            //11-20
                            li064 += liX037; li065 += liX038; li066 += liX039; li067 += liX040; li068 += liX041; li069 += liX042; li070 += liX043; li071 += liX044; li072 += liX045; li073 += liX046;
                            li074 += liX047; li075 += liX048; li076 += liX049; li077 += liX050; li078 += liX051; li079 += liX052; li080 += liX053; li081 += liX054; li082 += liX055; li083 += liX056;
                            li084 += liX057; li085 += liX058; li086 += liX059; li087 += liX060; li088 += liX061; li089 += liX062; li090 += liX063;
                            li091 = PxFull;
                            li092 = PxFull;
                        }
                        else if (battAmountActualloop < Am50_30To60)
                        {
                            //21-30
                            li093 += liX037; li094 += liX038; li095 += liX039; li096 += liX040; li097 += liX041; li098 += liX042; li099 += liX043; li100 += liX044; li101 += liX045; li102 += liX046;
                            li103 += liX047; li104 += liX048; li105 += liX049; li106 += liX050; li107 += liX051; li108 += liX052; li109 += liX053; li110 += liX054; li111 += liX055; li112 += liX056;
                            li113 += liX057; li114 += liX058; li115 += liX059; li116 += liX060; li117 += liX061; li118 += liX062; li119 += liX063;
                            li120 = PxFull;
                            li121 = PxFull;
                        }
                        else if (battAmountActualloop < Am50_40To80)
                        {
                            //31-40
                            li122 += liX037; li123 += liX038; li124 += liX039; li125 += liX040; li126 += liX041; li127 += liX042; li128 += liX043; li129 += liX044; li130 += liX045; li131 += liX046;
                            li132 += liX047; li133 += liX048; li134 += liX049; li135 += liX050; li136 += liX051; li137 += liX052; li138 += liX053; li139 += liX054; li140 += liX055; li141 += liX056;
                            li142 += liX057; li143 += liX058; li144 += liX059; li145 += liX060; li146 += liX061; li147 += liX062; li148 += liX063;
                            li149 = PxFull;
                            li150 = PxFull;
                        }
                        else if (battAmountActualloop > Am50_50To100)
                        {
                            //41-50
                            li151 += liX037; li152 += liX038; li153 += liX039; li154 += liX040; li155 += liX041; li156 += liX042; li157 += liX043; li158 += liX044; li159 += liX045; li160 += liX046;
                            li161 += liX047; li162 += liX048; li163 += liX049; li164 += liX050; li165 += liX051; li166 += liX052; li167 += liX053; li168 += liX054; li169 += liX055; li170 += liX056;
                            li171 += liX057; li172 += liX058; li173 += liX059; li174 += liX060; li175 += liX061; li176 += liX062; li177 += liX063;
                        }
                    }
                    else
                    {
                        // if batt Amount 1-10
                        if (battAmountActualloop < Am10_5To10)
                        {
                            li035 = PxFull;
                            li036 = PxFull;

                            li037 += liX037; li038 += liX038; li039 += liX039;
                            li040 += liX040; li041 += liX041; li042 += liX042; li043 += liX043; li044 += liX044; li045 += liX045; li046 += liX046; li047 += liX047; li048 += liX048; li049 += liX049;
                            li050 += liX050; li051 += liX051; li052 += liX052; li053 += liX053; li054 += liX054; li055 += liX055; li056 += liX056; li057 += liX057; li058 += liX058; li059 += liX059;
                            li060 += liX060; li061 += liX061; li062 += liX062; li063 += liX063; li064 += liX064; li065 += liX065; li066 += liX066; li067 += liX067; li068 += liX068; li069 += liX069;
                            li070 += liX070; li071 += liX071; li072 += liX072; li073 += liX073; li074 += liX074; li075 += liX075; li076 += liX076; li077 += liX077; li078 += liX078; li079 += liX079;
                            li080 += liX080; li081 += liX081; li082 += liX082; li083 += liX083; li084 += liX084; li085 += liX085; li086 += liX086; li087 += liX087; li088 += liX088;

                            li089 = PxFull;
                            li090 = PxFull;
                            li091 = PxFull;
                            li092 = PxFull;
                            li093 = PxFull;
                        }
                        else
                        {
                            //6-10
                            li094 += liX037; li095 += liX038; li096 += liX039;
                            li097 += liX040; li098 += liX041; li099 += liX042; li100 += liX043; li101 += liX044; li102 += liX045; li103 += liX046; li104 += liX047; li105 += liX048; li106 += liX049;
                            li107 += liX050; li108 += liX051; li109 += liX052; li110 += liX053; li111 += liX054; li112 += liX055; li113 += liX056; li114 += liX057; li115 += liX058; li116 += liX059;
                            li117 += liX060; li118 += liX061; li119 += liX062; li120 += liX063; li121 += liX064; li122 += liX065; li123 += liX066; li124 += liX067; li125 += liX068; li126 += liX069;
                            li127 += liX070; li128 += liX071; li129 += liX072; li130 += liX073; li131 += liX074; li132 += liX075; li133 += liX076; li134 += liX077; li135 += liX078; li136 += liX079;
                            li137 += liX080; li138 += liX081; li139 += liX082; li140 += liX083; li141 += liX084; li142 += liX085; li143 += liX086; li144 += liX087; li145 += liX088;
                        }
                    }
                }//End Battery Loop

                //Power Generator Status	########################################
                string PG_ST_018 = P28; string AmX018 = P11;
                string PG_ST_019 = P28; string AmX019 = P11;
                string PG_ST_020 = P28; string AmX020 = P11;
                string PG_ST_021 = P28; string AmX021 = P11;
                string PG_ST_022 = P28; string AmX022 = P11;
                string PG_ST_023 = P28; string AmX023 = P11;
                string PG_ST_024 = P28; string AmX024 = P11;
                string PG_ST_025 = P28; string AmX025 = P11;
                string PG_ST_026 = P28; string AmX026 = P11;
                string PG_ST_027 = P28; string AmX027 = P11;
                string PG_ST_028 = P28; string AmX028 = P11;
                string PG_ST_029 = P28; string AmX029 = P11;
                string PG_ST_030 = P28; string AmX030 = P11;

                string PG_Off_019 = ""; string PG_On_019 = ""; string PG_G_019 = ""; string PG_L_019 = "";
                string PG_Off_020 = ""; string PG_On_020 = ""; string PG_G_020 = ""; string PG_L_020 = "";
                string PG_Off_021 = ""; string PG_On_021 = ""; string PG_G_021 = ""; string PG_L_021 = "";
                string PG_Off_022 = ""; string PG_On_022 = ""; string PG_G_022 = ""; string PG_L_022 = "";
                string PG_Off_023 = ""; string PG_On_023 = ""; string PG_G_023 = ""; string PG_L_023 = "";
                string PG_Off_024 = ""; string PG_On_024 = ""; string PG_G_024 = ""; string PG_L_024 = "";
                string PG_Off_025 = ""; string PG_On_025 = ""; string PG_G_025 = ""; string PG_L_025 = "";
                string PG_Off_026 = ""; string PG_On_026 = ""; string PG_G_026 = ""; string PG_L_026 = "";
                string PG_Off_027 = ""; string PG_On_027 = ""; string PG_G_027 = ""; string PG_L_027 = "";
                string PG_Off_028 = ""; string PG_On_028 = ""; string PG_G_028 = ""; string PG_L_028 = "";
                string PG_Off_029 = ""; string PG_On_029 = ""; string PG_G_029 = ""; string PG_L_029 = "";

                int PowGenAmount = 0;
                float GenSol_CurrentOutput = 0f;
                float Solar_CurrentPower_MW = 0f;

                if (PowerInput_Enabled)
                {
                    var power_producer = new List<IMyPowerProducer>();
                    GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(power_producer);
                    foreach (var pp in power_producer)
                    {
                        if (pp.Enabled && pp.IsFunctional)
                        {
                            var isBattery = pp as IMyBatteryBlock;
                            if (isBattery == null)
                            {
                                GenSol_CurrentOutput += pp.CurrentOutput;

                                var isSolar = pp as IMySolarPanel;
                                if (isSolar != null)
                                    Solar_CurrentPower_MW += pp.CurrentOutput;
                                else
                                    PowGenAmount += 1; // Power Generator Counter (Excluding Solar Panels)
                            }
                        }
                    }

                    GenSol_CurrentOutput *= 1000; // mW to kW

                    if (PowGenAmount > 0)
                    {   //if Power Gen found
                        PG_ST_018 = "";
                        PG_ST_019 = "" + PG_G_019 + "";
                        PG_ST_020 = "" + PG_G_020 + "";
                        PG_ST_021 = "" + PG_G_021 + "";
                        PG_ST_022 = "" + PG_G_022 + "";
                        PG_ST_023 = "" + PG_G_023 + "";
                        PG_ST_024 = "" + PG_G_024 + "";
                        PG_ST_025 = "" + PG_G_025 + "";
                        PG_ST_026 = "" + PG_G_026 + "";
                        PG_ST_027 = "" + PG_G_027 + "";
                        PG_ST_028 = "" + PG_G_028 + "";
                        PG_ST_029 = "" + PG_G_029 + "";
                        PG_ST_030 = "";
                    }
                    else if (Solar_CurrentPower_MW > 0)
                    {   //if no Reactor, but Solar panel
                        PG_ST_018 = "";
                        PG_ST_019 = "" + PG_On_019 + "";
                        PG_ST_020 = "" + PG_On_020 + "";
                        PG_ST_021 = "" + PG_On_021 + "";
                        PG_ST_022 = "" + PG_On_022 + "";
                        PG_ST_023 = "" + PG_On_023 + "";
                        PG_ST_024 = "" + PG_On_024 + "";
                        PG_ST_025 = "" + PG_On_025 + "";
                        PG_ST_026 = "" + PG_On_026 + "";
                        PG_ST_027 = "" + PG_On_027 + "";
                        PG_ST_028 = "" + PG_On_028 + "";
                        PG_ST_029 = "" + PG_On_029 + "";
                        PG_ST_030 = "";
                    }
                    else
                    {
                        //if no Reactor + Solar found
                        PG_ST_018 = "";
                        PG_ST_019 = "" + PG_Off_019 + "";
                        PG_ST_020 = "" + PG_Off_020 + "";
                        PG_ST_021 = "" + PG_Off_021 + "";
                        PG_ST_022 = "" + PG_Off_022 + "";
                        PG_ST_023 = "" + PG_Off_023 + "";
                        PG_ST_024 = "" + PG_Off_024 + "";
                        PG_ST_025 = "" + PG_Off_025 + "";
                        PG_ST_026 = "" + PG_Off_026 + "";
                        PG_ST_027 = "" + PG_Off_027 + "";
                        PG_ST_028 = "" + PG_Off_028 + "";
                        PG_ST_029 = "" + PG_Off_029 + "";
                        PG_ST_030 = "";
                    }
                    if (GenSol_CurrentOutput < batt_X_CurrInput_KW_All)
                    {   //if needed input is lower then actual input
                        PG_ST_018 = "";
                        PG_ST_019 = "" + PG_L_019 + "";
                        PG_ST_020 = "" + PG_L_020 + "";
                        PG_ST_021 = "" + PG_L_021 + "";
                        PG_ST_022 = "" + PG_L_022 + "";
                        PG_ST_023 = "" + PG_L_023 + "";
                        PG_ST_024 = "" + PG_L_024 + "";
                        PG_ST_025 = "" + PG_L_025 + "";
                        PG_ST_026 = "" + PG_L_026 + "";
                        PG_ST_027 = "" + PG_L_027 + "";
                        PG_ST_028 = "" + PG_L_028 + "";
                        PG_ST_029 = "" + PG_L_029 + "";
                        PG_ST_030 = "";
                    }
                }
                string AmN1018 = P11; string AmN2018 = P11; string AmN3018 = P11; string BatSpLi018 = P2;
                string AmN1019 = P11; string AmN2019 = P11; string AmN3019 = P11; string BatSpLi019 = P2;
                string AmN1020 = P11; string AmN2020 = P11; string AmN3020 = P11; string BatSpLi020 = P2;
                string AmN1021 = P11; string AmN2021 = P11; string AmN3021 = P11; string BatSpLi021 = P2;
                string AmN1022 = P11; string AmN2022 = P11; string AmN3022 = P11; string BatSpLi022 = P2;
                string AmN1023 = P11; string AmN2023 = P11; string AmN3023 = P11; string BatSpLi023 = P2;
                string AmN1024 = P11; string AmN2024 = P11; string AmN3024 = P11; string BatSpLi024 = P2;
                string AmN1025 = P11; string AmN2025 = P11; string AmN3025 = P11; string BatSpLi025 = P2;
                string AmN1026 = P11; string AmN2026 = P11; string AmN3026 = P11; string BatSpLi026 = P2;
                string AmN1027 = P11; string AmN2027 = P11; string AmN3027 = P11; string BatSpLi027 = P2;
                string AmN1028 = P11; string AmN2028 = P11; string AmN3028 = P11; string BatSpLi028 = P2;
                string AmN1029 = P11; string AmN2029 = P11; string AmN3029 = P11; string BatSpLi029 = P2;
                string AmN1030 = P11; string AmN2030 = P11; string AmN3030 = P11; string BatSpLi030 = P2;

                string MWN1018 = P11; string MWN2018 = P11; string MWN3018 = P11;
                string MWN1019 = P11; string MWN2019 = P11; string MWN3019 = P11;
                string MWN1020 = P11; string MWN2020 = P11; string MWN3020 = P11;
                string MWN1021 = P11; string MWN2021 = P11; string MWN3021 = P11;
                string MWN1022 = P11; string MWN2022 = P11; string MWN3022 = P11;
                string MWN1023 = P11; string MWN2023 = P11; string MWN3023 = P11;
                string MWN1024 = P11; string MWN2024 = P11; string MWN3024 = P11;
                string MWN1025 = P11; string MWN2025 = P11; string MWN3025 = P11;
                string MWN1026 = P11; string MWN2026 = P11; string MWN3026 = P11;
                string MWN1027 = P11; string MWN2027 = P11; string MWN3027 = P11;
                string MWN1028 = P11; string MWN2028 = P11; string MWN3028 = P11;
                string MWN1029 = P11; string MWN2029 = P11; string MWN3029 = P11;
                string MWN1030 = P11; string MWN2030 = P11; string MWN3030 = P11;

                int input = 0;
                int Am_NX = 0;
                int Am_NXX = 0;
                int Am_NXXX = 0;
                int MW_NX = 0;
                int MW_NXX = 0;
                int MW_NXXX = 0;

                if (BatteryAmountEnabled)
                {
                    if (!PowerInput_Enabled)
                    {
                        PG_ST_018 = "";
                        PG_ST_019 = "" + P8 + "";
                        PG_ST_020 = "" + "" + "";
                        PG_ST_021 = "" + P8 + "";
                        PG_ST_022 = "" + P8 + "";
                        PG_ST_023 = "" + P8 + "";
                        PG_ST_024 = "" + P8 + "";
                        PG_ST_025 = "" + P8 + "";
                        PG_ST_026 = "" + P8 + "";
                        PG_ST_027 = "" + P8 + "";
                        PG_ST_028 = "" + P8 + "";
                        PG_ST_029 = "" + P8 + "";
                        PG_ST_030 = "";
                    }
                    AmX018 = "";
                    AmX019 = "";
                    AmX020 = "";
                    AmX021 = "";
                    AmX022 = "";
                    AmX023 = "";
                    AmX024 = "";
                    AmX025 = "";
                    AmX026 = "";
                    AmX027 = "";
                    AmX028 = "";
                    AmX029 = "";
                    AmX030 = "";
                }
                //Convert Val-Number seperate Num, for Amount + Stored Energy
                int CounterNumConvert = 2;
                int CheckerNumConvert = CounterNumConvert;

                if (batX_CurrStored_KW_All > 999999) { batX_CurrStored_KW_All = batX_CurrStored_KW_All / 1000000; batteryStoredUnit = "GWh"; }
                else if (batX_CurrStored_KW_All > 999) { batX_CurrStored_KW_All = batX_CurrStored_KW_All / 1000; batteryStoredUnit = "MWh"; }
                else if (batX_CurrStored_KW_All < 1) { batX_CurrStored_KW_All = batX_CurrStored_KW_All * 1000; batteryStoredUnit = "Wh"; }

                int numVal = Convert.ToInt32(batX_CurrStored_KW_All);

                for (int i = 0; i < CounterNumConvert; i++)
                {   // Here set your Number to split
                    if (CheckerNumConvert == 2) { input = battAmountActualloop; }
                    else if (CheckerNumConvert == 1) { input = numVal; }

                    // Create Variables to hold Data
                    int input_No100 = 0;
                    int input_No10 = 0;
                    int input_No1 = 0;
                    int inputNo10_Cache = 0;
                    int inputNo1_Cache = 0;
                    if (input > 99)
                    {   // Input > then 99 (100 bis 999+)
                        if (input > 899) { input_No100 = 9; inputNo10_Cache = input - 900; }    // 900-999
                        else if (input > 799) { input_No100 = 8; inputNo10_Cache = input - 800; }   // 800-899
                        else if (input > 699) { input_No100 = 7; inputNo10_Cache = input - 700; }   // 700-799
                        else if (input > 599) { input_No100 = 6; inputNo10_Cache = input - 600; }   // 600-699
                        else if (input > 499) { input_No100 = 5; inputNo10_Cache = input - 500; }   // 500-599
                        else if (input > 399) { input_No100 = 4; inputNo10_Cache = input - 400; }   // 400-499
                        else if (input > 299) { input_No100 = 3; inputNo10_Cache = input - 300; }   // 300-399
                        else if (input > 199) { input_No100 = 2; inputNo10_Cache = input - 200; }   // 200-299
                        else if (input > 99) { input_No100 = 1; inputNo10_Cache = input - 100; }    // 100-199
                        else if (input < 100) { input_No100 = 0; inputNo10_Cache = input; }     // 1-99
                        else if (input > 999) { input_No100 = 9; inputNo10_Cache = input; }     // 1000 + more
                                                                                                // inputNo10_Cache > then 9 < 100
                        if (inputNo10_Cache > 89) { input_No10 = 9; inputNo1_Cache = inputNo10_Cache - 90; }    // 90-99
                        else if (inputNo10_Cache > 79) { input_No10 = 8; inputNo1_Cache = inputNo10_Cache - 80; }   // 80-89
                        else if (inputNo10_Cache > 69) { input_No10 = 7; inputNo1_Cache = inputNo10_Cache - 70; }   // 70-79
                        else if (inputNo10_Cache > 59) { input_No10 = 6; inputNo1_Cache = inputNo10_Cache - 60; }   // 60-69
                        else if (inputNo10_Cache > 49) { input_No10 = 5; inputNo1_Cache = inputNo10_Cache - 50; }   // 50-59
                        else if (inputNo10_Cache > 39) { input_No10 = 4; inputNo1_Cache = inputNo10_Cache - 40; } // 40-49
                        else if (inputNo10_Cache > 29) { input_No10 = 3; inputNo1_Cache = inputNo10_Cache - 30; }   // 30-39
                        else if (inputNo10_Cache > 19) { input_No10 = 2; inputNo1_Cache = inputNo10_Cache - 20; }   // 20-29
                        else if (inputNo10_Cache > 9) { input_No10 = 1; inputNo1_Cache = inputNo10_Cache - 10; }    // 10-19
                        else if (inputNo10_Cache < 10) { input_No10 = 0; inputNo1_Cache = inputNo10_Cache; }        // 1-9
                        else if (inputNo10_Cache > 999) { input_No10 = 9; inputNo1_Cache = inputNo10_Cache; }       // 1000 + more
                                                                                                                    // get last Number
                        if (inputNo1_Cache == 1) { input_No1 = 1; }
                        else if (inputNo1_Cache == 2) { input_No1 = 2; }
                        else if (inputNo1_Cache == 3) { input_No1 = 3; }
                        else if (inputNo1_Cache == 4) { input_No1 = 4; }
                        else if (inputNo1_Cache == 5) { input_No1 = 5; }
                        else if (inputNo1_Cache == 6) { input_No1 = 6; }
                        else if (inputNo1_Cache == 7) { input_No1 = 7; }
                        else if (inputNo1_Cache == 8) { input_No1 = 8; }
                        else if (inputNo1_Cache == 9) { input_No1 = 9; }
                        else if (inputNo1_Cache == 0) { input_No1 = 0; }
                        else { input_No1 = 9; }
                    }
                    else if (input > 9)
                    {   // Input > then 9 < 100
                        if (input > 89) { input_No10 = 9; inputNo1_Cache = input - 90; }    // 90-99
                        else if (input > 79) { input_No10 = 8; inputNo1_Cache = input - 80; }   // 80-89
                        else if (input > 69) { input_No10 = 7; inputNo1_Cache = input - 70; }   // 70-79
                        else if (input > 59) { input_No10 = 6; inputNo1_Cache = input - 60; }   // 60-69
                        else if (input > 49) { input_No10 = 5; inputNo1_Cache = input - 50; }   // 50-59
                        else if (input > 39) { input_No10 = 4; inputNo1_Cache = input - 40; }   // 40-49
                        else if (input > 29) { input_No10 = 3; inputNo1_Cache = input - 30; }   // 30-39
                        else if (input > 19) { input_No10 = 2; inputNo1_Cache = input - 20; }   // 20-29
                        else { input_No10 = 1; inputNo1_Cache = input - 10; }   // 11-19
                                                                                // get last Number
                        if (inputNo1_Cache == 1) { input_No1 = 1; }
                        else if (inputNo1_Cache == 2) { input_No1 = 2; }
                        else if (inputNo1_Cache == 3) { input_No1 = 3; }
                        else if (inputNo1_Cache == 4) { input_No1 = 4; }
                        else if (inputNo1_Cache == 5) { input_No1 = 5; }
                        else if (inputNo1_Cache == 6) { input_No1 = 6; }
                        else if (inputNo1_Cache == 7) { input_No1 = 7; }
                        else if (inputNo1_Cache == 8) { input_No1 = 8; }
                        else if (inputNo1_Cache == 9) { input_No1 = 9; }
                        else { input_No1 = 0; }
                    }
                    else
                    {   // Input < then 10
                        if (input == 1) { input_No1 = 1; }
                        else if (input == 2) { input_No1 = 2; }
                        else if (input == 3) { input_No1 = 3; }
                        else if (input == 4) { input_No1 = 4; }
                        else if (input == 5) { input_No1 = 5; }
                        else if (input == 6) { input_No1 = 6; }
                        else if (input == 7) { input_No1 = 7; }
                        else if (input == 8) { input_No1 = 8; }
                        else if (input == 9) { input_No1 = 9; }
                        else { input_No1 = 0; }
                    }
                    if (CheckerNumConvert == 2) { Am_NX = input_No1; Am_NXX = input_No10; Am_NXXX = input_No100; }
                    else if (CheckerNumConvert == 1) { MW_NX = input_No1; MW_NXX = input_No10; MW_NXXX = input_No100; }
                    CheckerNumConvert = CheckerNumConvert - 1;

                    // Returns:
                    //Am_NX
                    //Am_NXX
                    //Am_NXXX
                    //MW_NX
                    //MW_NXX
                    //MW_NXXX
                }
                string Chk_Num_018 = P11; string Chk_Num_019 = P11; string Chk_Num_020 = P11; string Chk_Num_021 = P11; string Chk_Num_022 = P11; string Chk_Num_023 = P11; string Chk_Num_024 = P11;
                string Chk_Num_025 = P11; string Chk_Num_026 = P11; string Chk_Num_027 = P11; string Chk_Num_028 = P11; string Chk_Num_029 = P11; string Chk_Num_030 = P11;

                int Chk_Num_X = 0;
                int Chk_StateCounter = 6;
                int Chk_State = Chk_StateCounter;

                for (int i = 0; i < Chk_StateCounter; i++)
                {
                    if (Chk_State == 6) { Chk_Num_X = Am_NXXX; }
                    else if (Chk_State == 5) { Chk_Num_X = Am_NXX; }
                    else if (Chk_State == 4) { Chk_Num_X = Am_NX; }
                    else if (Chk_State == 3) { Chk_Num_X = MW_NXXX; }
                    else if (Chk_State == 2) { Chk_Num_X = MW_NXX; }
                    else if (Chk_State == 1) { Chk_Num_X = MW_NX; }


                    if (Chk_Num_X == 9)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 8)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 7)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 6)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 5)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 4)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 3)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 2)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 1)
                    {
                        Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                        Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                    }
                    else if (Chk_Num_X == 0)
                    {
                        bool Chk_Num_ShwPx = false;
                        if (Chk_State == 6) { Chk_Num_ShwPx = true; }
                        else if (Chk_State == 5)
                        {
                            if (battAmountActualloop < 10) { Chk_Num_ShwPx = true; }
                        }
                        else if (Chk_State == 3) { Chk_Num_ShwPx = true; }
                        else if (Chk_State == 2)
                        {
                            if (batX_CurrStored_KW_All < 10) { Chk_Num_ShwPx = true; }
                        }
                        if (Chk_Num_ShwPx)
                        {
                            Chk_Num_018 = P11; Chk_Num_019 = P11; Chk_Num_020 = P11; Chk_Num_021 = P11; Chk_Num_022 = P11; Chk_Num_023 = P11; Chk_Num_024 = P11;
                            Chk_Num_025 = P11; Chk_Num_026 = P11; Chk_Num_027 = P11; Chk_Num_028 = P11; Chk_Num_029 = P11; Chk_Num_030 = P11;
                        }
                        else
                        {
                            Chk_Num_018 = ""; Chk_Num_019 = ""; Chk_Num_020 = ""; Chk_Num_021 = ""; Chk_Num_022 = ""; Chk_Num_023 = "";
                            Chk_Num_024 = ""; Chk_Num_025 = ""; Chk_Num_026 = ""; Chk_Num_027 = ""; Chk_Num_028 = ""; Chk_Num_029 = ""; Chk_Num_030 = "";
                        }
                    }

                    if (Chk_State == 6)
                    {
                        if (BatteryAmountEnabled)
                        {
                            AmN3018 = Chk_Num_018; AmN3019 = Chk_Num_019; AmN3020 = Chk_Num_020; AmN3021 = Chk_Num_021; AmN3022 = Chk_Num_022; AmN3023 = Chk_Num_023; AmN3024 = Chk_Num_024;
                            AmN3025 = Chk_Num_025; AmN3026 = Chk_Num_026; AmN3027 = Chk_Num_027; AmN3028 = Chk_Num_028; AmN3029 = Chk_Num_029; AmN3030 = Chk_Num_030;
                        }
                    }
                    else if (Chk_State == 5)
                    {
                        if (BatteryAmountEnabled)
                        {
                            AmN2018 = Chk_Num_018; AmN2019 = Chk_Num_019; AmN2020 = Chk_Num_020; AmN2021 = Chk_Num_021; AmN2022 = Chk_Num_022; AmN2023 = Chk_Num_023; AmN2024 = Chk_Num_024;
                            AmN2025 = Chk_Num_025; AmN2026 = Chk_Num_026; AmN2027 = Chk_Num_027; AmN2028 = Chk_Num_028; AmN2029 = Chk_Num_029; AmN2030 = Chk_Num_030;
                        }
                    }
                    else if (Chk_State == 4)
                    {
                        if (BatteryAmountEnabled)
                        {
                            AmN1018 = Chk_Num_018; AmN1019 = Chk_Num_019; AmN1020 = Chk_Num_020; AmN1021 = Chk_Num_021; AmN1022 = Chk_Num_022; AmN1023 = Chk_Num_023; AmN1024 = Chk_Num_024;
                            AmN1025 = Chk_Num_025; AmN1026 = Chk_Num_026; AmN1027 = Chk_Num_027; AmN1028 = Chk_Num_028; AmN1029 = Chk_Num_029; AmN1030 = Chk_Num_030;
                        }
                    }
                    else if (Chk_State == 3)
                    {
                        if (BatteryAllStoredEnergyEnabled)
                        {
                            MWN3018 = Chk_Num_018; MWN3019 = Chk_Num_019; MWN3020 = Chk_Num_020; MWN3021 = Chk_Num_021; MWN3022 = Chk_Num_022; MWN3023 = Chk_Num_023; MWN3024 = Chk_Num_024;
                            MWN3025 = Chk_Num_025; MWN3026 = Chk_Num_026; MWN3027 = Chk_Num_027; MWN3028 = Chk_Num_028; MWN3029 = Chk_Num_029; MWN3030 = Chk_Num_030;
                        }
                    }
                    else if (Chk_State == 2)
                    {
                        if (BatteryAllStoredEnergyEnabled)
                        {
                            MWN2018 = Chk_Num_018; MWN2019 = Chk_Num_019; MWN2020 = Chk_Num_020; MWN2021 = Chk_Num_021; MWN2022 = Chk_Num_022; MWN2023 = Chk_Num_023; MWN2024 = Chk_Num_024;
                            MWN2025 = Chk_Num_025; MWN2026 = Chk_Num_026; MWN2027 = Chk_Num_027; MWN2028 = Chk_Num_028; MWN2029 = Chk_Num_029; MWN2030 = Chk_Num_030;
                        }
                    }
                    else if (Chk_State == 1)
                    {
                        if (BatteryAllStoredEnergyEnabled)
                        {
                            MWN1018 = Chk_Num_018; MWN1019 = Chk_Num_019; MWN1020 = Chk_Num_020; MWN1021 = Chk_Num_021; MWN1022 = Chk_Num_022; MWN1023 = Chk_Num_023; MWN1024 = Chk_Num_024;
                            MWN1025 = Chk_Num_025; MWN1026 = Chk_Num_026; MWN1027 = Chk_Num_027; MWN1028 = Chk_Num_028; MWN1029 = Chk_Num_029; MWN1030 = Chk_Num_030;
                        }
                    }
                    Chk_State -= 1;
                }
                string Unit_ST01 = P38;
                string Unit_ST02 = P38;
                string Unit_ST03 = P38;
                string Unit_ST04 = P38;
                string Unit_ST05 = P38;
                string Unit_ST06 = P38;
                string Unit_ST07 = P38;
                string Unit_ST08 = P38;
                string Unit_ST09 = P38;
                string Unit_ST10 = P38;
                string Unit_ST11 = P38;
                string Unit_ST12 = P38;
                string Unit_ST13 = P38;

                bool BatUnit_Wh = false;
                bool BatUnit_kWh = false;
                bool BatUnit_MWh = false;
                bool BatUnit_GWh = false;

                if (batteryStoredUnit == "GWh") { BatUnit_GWh = true; }
                else if (batteryStoredUnit == "MWh") { BatUnit_MWh = true; }
                else if (batteryStoredUnit == "Wh") { BatUnit_Wh = true; }
                else { BatUnit_kWh = true; }

                if (BatteryAllStoredEnergyEnabled)
                {
                    if (BatUnit_Wh)
                    {
                        Unit_ST01 = "";
                        Unit_ST02 = "";
                        Unit_ST03 = "";
                        Unit_ST04 = "";
                        Unit_ST05 = "";
                        Unit_ST06 = "";
                        Unit_ST07 = "";
                        Unit_ST08 = "";
                        Unit_ST09 = "";
                        Unit_ST10 = "";
                        Unit_ST11 = "";
                        Unit_ST12 = "";
                        Unit_ST13 = "";
                    }
                    else if (BatUnit_MWh)
                    {
                        Unit_ST01 = "";
                        Unit_ST02 = "";
                        Unit_ST03 = "";
                        Unit_ST04 = "";
                        Unit_ST05 = "";
                        Unit_ST06 = "";
                        Unit_ST07 = "";
                        Unit_ST08 = "";
                        Unit_ST09 = "";
                        Unit_ST10 = "";
                        Unit_ST11 = "";
                        Unit_ST12 = "";
                        Unit_ST13 = "";
                    }
                    else if (BatUnit_GWh)
                    {
                        Unit_ST01 = "";
                        Unit_ST02 = "";
                        Unit_ST03 = "";
                        Unit_ST04 = "";
                        Unit_ST05 = "";
                        Unit_ST06 = "";
                        Unit_ST07 = "";
                        Unit_ST08 = "";
                        Unit_ST09 = "";
                        Unit_ST10 = "";
                        Unit_ST11 = "";
                        Unit_ST12 = "";
                        Unit_ST13 = "";
                    }
                    else if (BatUnit_kWh)
                    {
                        Unit_ST01 = "";
                        Unit_ST02 = "";
                        Unit_ST03 = "";
                        Unit_ST04 = "";
                        Unit_ST05 = "";
                        Unit_ST06 = "";
                        Unit_ST07 = "";
                        Unit_ST08 = "";
                        Unit_ST09 = "";
                        Unit_ST10 = "";
                        Unit_ST11 = "";
                        Unit_ST12 = "";
                        Unit_ST13 = "";
                    }
                }
                //Start lis 1-3	################################
                string li001 = PxFull;
                string li002 = PxFull;
                string li003 = PxFull;
                //Battery Title 4-13	############################
                string li004 = P9 + "" + P8;
                string li005 = P9 + "" + P8;
                string li006 = P9 + "" + P8;
                string li007 = P9 + "" + P8;
                string li008 = P9 + "" + P8;
                string li009 = P9 + "" + P8;
                string li010 = P9 + "" + P8;
                string li011 = P9 + "" + P8;
                string li012 = P9 + "" + P8;
                string li013 = P9 + "" + P8;
                if (!BatteryTitle_Enabled)
                {
                    li004 = PxFull; li005 = PxFull; li006 = PxFull; li007 = PxFull; li008 = PxFull; li009 = PxFull; li010 = PxFull; li011 = PxFull; li012 = PxFull; li013 = PxFull;
                }
                //Space		########################################
                string li014 = PxFull;
                //Underli 1	####################################
                string li015 = underli_A;
                string li016 = underli_B;
                if (!Underline_1_Enabled) { li015 = PxFull; li016 = PxFull; }
                if (BatSpaceline_Enabled)
                {
                    BatSpLi018 = ""; BatSpLi019 = ""; BatSpLi020 = ""; BatSpLi021 = ""; BatSpLi022 = ""; BatSpLi023 = ""; BatSpLi024 = "";
                    BatSpLi025 = ""; BatSpLi026 = ""; BatSpLi027 = ""; BatSpLi028 = ""; BatSpLi029 = ""; BatSpLi030 = "";
                }

                //Space		########################################
                string li017 = PxFull;
                //lis Battery Amount + MW		####################
                string li018 = P9 + PG_ST_018 + AmX018 + AmN3018 + AmN2018 + P1 + AmN1018 + P9 + BatSpLi018 + P10 + MWN3018 + P1 + MWN2018 + P1 + MWN1018 + P3 + Unit_ST01;
                string li019 = P9 + PG_ST_019 + AmX019 + AmN3019 + AmN2019 + P1 + AmN1019 + P9 + BatSpLi019 + P10 + MWN3019 + P1 + MWN2019 + P1 + MWN1019 + P3 + Unit_ST02;
                string li020 = P9 + PG_ST_020 + AmX020 + AmN3020 + AmN2020 + P1 + AmN1020 + P9 + BatSpLi020 + P10 + MWN3020 + P1 + MWN2020 + P1 + MWN1020 + P3 + Unit_ST03;
                string li021 = P9 + PG_ST_021 + AmX021 + AmN3021 + AmN2021 + P1 + AmN1021 + P9 + BatSpLi021 + P10 + MWN3021 + P1 + MWN2021 + P1 + MWN1021 + P3 + Unit_ST04;
                string li022 = P9 + PG_ST_022 + AmX022 + AmN3022 + AmN2022 + P1 + AmN1022 + P9 + BatSpLi022 + P10 + MWN3022 + P1 + MWN2022 + P1 + MWN1022 + P3 + Unit_ST05;
                string li023 = P9 + PG_ST_023 + AmX023 + AmN3023 + AmN2023 + P1 + AmN1023 + P9 + BatSpLi023 + P10 + MWN3023 + P1 + MWN2023 + P1 + MWN1023 + P3 + Unit_ST06;
                string li024 = P9 + PG_ST_024 + AmX024 + AmN3024 + AmN2024 + P1 + AmN1024 + P9 + BatSpLi024 + P10 + MWN3024 + P1 + MWN2024 + P1 + MWN1024 + P3 + Unit_ST07;
                string li025 = P9 + PG_ST_025 + AmX025 + AmN3025 + AmN2025 + P1 + AmN1025 + P9 + BatSpLi025 + P10 + MWN3025 + P1 + MWN2025 + P1 + MWN1025 + P3 + Unit_ST08;
                string li026 = P9 + PG_ST_026 + AmX026 + AmN3026 + AmN2026 + P1 + AmN1026 + P9 + BatSpLi026 + P10 + MWN3026 + P1 + MWN2026 + P1 + MWN1026 + P3 + Unit_ST09;
                string li027 = P9 + PG_ST_027 + AmX027 + AmN3027 + AmN2027 + P1 + AmN1027 + P9 + BatSpLi027 + P10 + MWN3027 + P1 + MWN2027 + P1 + MWN1027 + P3 + Unit_ST10;
                string li028 = P9 + PG_ST_028 + AmX028 + AmN3028 + AmN2028 + P1 + AmN1028 + P9 + BatSpLi028 + P10 + MWN3028 + P1 + MWN2028 + P1 + MWN1028 + P3 + Unit_ST11;
                string li029 = P9 + PG_ST_029 + AmX029 + AmN3029 + AmN2029 + P1 + AmN1029 + P9 + BatSpLi029 + P10 + MWN3029 + P1 + MWN2029 + P1 + MWN1029 + P3 + Unit_ST12;
                string li030 = P9 + PG_ST_030 + AmX030 + AmN3030 + AmN2030 + P1 + AmN1030 + P9 + BatSpLi030 + P10 + MWN3030 + P1 + MWN2030 + P1 + MWN1030 + P3 + Unit_ST13;
                //Space		########################################
                string li031 = PxFull;
                string li032 = PxFull;
                //Underli 2	####################################
                string li033 = underli_A;
                string li034 = underli_B;
                if (!Underline_2_Enabled) { li033 = PxFull; li034 = PxFull; }
                //Bound all lis together
                string str_Boundli_001_To_010 = li001 + Breakli + li002 + Breakli + li003 + Breakli + li004 + Breakli + li005 + Breakli + li006 + Breakli + li007 + Breakli + li008 + Breakli + li009 + Breakli + li010 + Breakli;
                string str_Boundli_011_To_020 = li011 + Breakli + li012 + Breakli + li013 + Breakli + li014 + Breakli + li015 + Breakli + li016 + Breakli + li017 + Breakli + li018 + Breakli + li019 + Breakli + li020 + Breakli;
                string str_Boundli_021_To_030 = li021 + Breakli + li022 + Breakli + li023 + Breakli + li024 + Breakli + li025 + Breakli + li026 + Breakli + li027 + Breakli + li028 + Breakli + li029 + Breakli + li030 + Breakli;
                string str_Boundli_031_To_040 = li031 + Breakli + li032 + Breakli + li033 + Breakli + li034 + Breakli + li035 + Breakli + li036 + Breakli + li037 + Breakli + li038 + Breakli + li039 + Breakli + li040 + Breakli;
                string str_Boundli_041_To_050 = li041 + Breakli + li042 + Breakli + li043 + Breakli + li044 + Breakli + li045 + Breakli + li046 + Breakli + li047 + Breakli + li048 + Breakli + li049 + Breakli + li050 + Breakli;
                string str_Boundli_051_To_060 = li051 + Breakli + li052 + Breakli + li053 + Breakli + li054 + Breakli + li055 + Breakli + li056 + Breakli + li057 + Breakli + li058 + Breakli + li059 + Breakli + li060 + Breakli;
                string str_Boundli_061_To_070 = li061 + Breakli + li062 + Breakli + li063 + Breakli + li064 + Breakli + li065 + Breakli + li066 + Breakli + li067 + Breakli + li068 + Breakli + li069 + Breakli + li070 + Breakli;
                string str_Boundli_071_To_080 = li071 + Breakli + li072 + Breakli + li073 + Breakli + li074 + Breakli + li075 + Breakli + li076 + Breakli + li077 + Breakli + li078 + Breakli + li079 + Breakli + li080 + Breakli;
                string str_Boundli_081_To_090 = li081 + Breakli + li082 + Breakli + li083 + Breakli + li084 + Breakli + li085 + Breakli + li086 + Breakli + li087 + Breakli + li088 + Breakli + li089 + Breakli + li090 + Breakli;
                string str_Boundli_091_To_100 = li091 + Breakli + li092 + Breakli + li093 + Breakli + li094 + Breakli + li095 + Breakli + li096 + Breakli + li097 + Breakli + li098 + Breakli + li099 + Breakli + li100 + Breakli;
                string str_Boundli_101_To_110 = li101 + Breakli + li102 + Breakli + li103 + Breakli + li104 + Breakli + li105 + Breakli + li106 + Breakli + li107 + Breakli + li108 + Breakli + li109 + Breakli + li110 + Breakli;
                string str_Boundli_111_To_120 = li111 + Breakli + li112 + Breakli + li113 + Breakli + li114 + Breakli + li115 + Breakli + li116 + Breakli + li117 + Breakli + li118 + Breakli + li119 + Breakli + li120 + Breakli;
                string str_Boundli_121_To_130 = li121 + Breakli + li122 + Breakli + li123 + Breakli + li124 + Breakli + li125 + Breakli + li126 + Breakli + li127 + Breakli + li128 + Breakli + li129 + Breakli + li130 + Breakli;
                string str_Boundli_131_To_140 = li131 + Breakli + li132 + Breakli + li133 + Breakli + li134 + Breakli + li135 + Breakli + li136 + Breakli + li137 + Breakli + li138 + Breakli + li139 + Breakli + li140 + Breakli;
                string str_Boundli_141_To_150 = li141 + Breakli + li142 + Breakli + li143 + Breakli + li144 + Breakli + li145 + Breakli + li146 + Breakli + li147 + Breakli + li148 + Breakli + li149 + Breakli + li150 + Breakli;
                string str_Boundli_151_To_160 = li151 + Breakli + li152 + Breakli + li153 + Breakli + li154 + Breakli + li155 + Breakli + li156 + Breakli + li157 + Breakli + li158 + Breakli + li159 + Breakli + li160 + Breakli;
                string str_Boundli_161_To_170 = li161 + Breakli + li162 + Breakli + li163 + Breakli + li164 + Breakli + li165 + Breakli + li166 + Breakli + li167 + Breakli + li168 + Breakli + li169 + Breakli + li170 + Breakli;
                string str_Boundli_171_To_178 = li171 + Breakli + li172 + Breakli + li173 + Breakli + li174 + Breakli + li175 + Breakli + li176 + Breakli + li177 + Breakli + li178;
                string str_AllBoundlis_001_To_178 = str_Boundli_001_To_010 + str_Boundli_011_To_020 + str_Boundli_021_To_030 + str_Boundli_031_To_040 + str_Boundli_041_To_050 + str_Boundli_051_To_060 + str_Boundli_061_To_070 + str_Boundli_071_To_080 + str_Boundli_081_To_090 + str_Boundli_091_To_100 + str_Boundli_101_To_110 + str_Boundli_111_To_120 + str_Boundli_121_To_130 + str_Boundli_131_To_140 + str_Boundli_141_To_150 + str_Boundli_151_To_160 + str_Boundli_161_To_170 + str_Boundli_171_To_178;

                // find all LCD with NameTag
                var BBS_TextPanels = new List<IMyTerminalBlock>();
                GridTerminalSystem.SearchBlocksOfName(LCD_NameTag, BBS_TextPanels);

                // this loop send Message to show to all found Lcds
                foreach(var TerminalBlock in BBS_TextPanels)
                {
                    var BSS_TextPanel = TerminalBlock as IMyTextPanel;

                    BSS_TextPanel.SetValue("FontColor", new Color(LCDbright, LCDbright, LCDbright)); // White
                    BSS_TextPanel.SetValue("FontSize", 0.10f);    // set Font size of your LCD
                    BSS_TextPanel.SetValue("Font", (long)1147350002);
                    BSS_TextPanel.ShowPublicTextOnScreen();
                    //BatteryStatus_Lcd_X.WritePublicText("", false);
                    BSS_TextPanel.WritePublicText("" + str_AllBoundlis_001_To_178, false);
                }
            }
        } // End of Main script
    }
}