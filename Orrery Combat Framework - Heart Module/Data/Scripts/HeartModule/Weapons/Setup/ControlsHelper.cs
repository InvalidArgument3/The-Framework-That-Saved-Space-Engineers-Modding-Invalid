﻿using Sandbox.Game.Localization;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;
using YourName.ModName.Data.Scripts.HeartModule.Weapons.Setup.Adding;

namespace Heart_Module.Data.Scripts.HeartModule.Weapons.Setup
{
    internal static class ControlsHelper
    {
        const string IdPrefix = "ModularHeartMod_"; // highly recommended to tag your properties/actions like this to avoid colliding with other mods'

        public static IMyTerminalControlOnOffSwitch CreateToggle(string id, string displayName, string toolTip, Func<IMyTerminalBlock, bool> getter, Action<IMyTerminalBlock, bool> setter)
        {
            var ShootToggle = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyConveyorSorter>(IdPrefix + id);
            ShootToggle.Title = MyStringId.GetOrCompute(displayName);
            ShootToggle.Tooltip = MyStringId.GetOrCompute(toolTip);
            ShootToggle.SupportsMultipleBlocks = true; // wether this control should be visible when multiple blocks are selected (as long as they all have this control).
                                                       // callbacks to determine if the control should be visible or not-grayed-out(Enabled) depending on whatever custom condition you want, given a block instance.
                                                       // optional, they both default to true.
            ShootToggle.Visible = CustomVisibleCondition;
            //c.Enabled = CustomVisibleCondition;
            ShootToggle.OnText = MySpaceTexts.SwitchText_On;
            ShootToggle.OffText = MySpaceTexts.SwitchText_Off;
            //c.OffText = MyStringId.GetOrCompute("Off");
            // setters and getters should both be assigned on all controls that have them, to avoid errors in mods or PB scripts getting exceptions from them.
            ShootToggle.Getter = getter;  // Getting the value
            ShootToggle.Setter = setter; // Setting the value

            MyAPIGateway.TerminalControls.AddControl<IMyConveyorSorter>(ShootToggle);

            return ShootToggle;
        }

        public static IMyTerminalControlSlider CreateSlider(string id, string displayName, string toolTip, float min, float max, Func<IMyTerminalBlock, float> getter, Action<IMyTerminalBlock, float> setter, Action<IMyTerminalBlock, StringBuilder> writer)
        {
            var slider = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyConveyorSorter>(IdPrefix + id);
            slider.Title = MyStringId.GetOrCompute(displayName);
            slider.Tooltip = MyStringId.GetOrCompute(toolTip);
            slider.SetLimits(min, max); // Set the minimum and maximum values for the slider
            slider.Getter = getter; // Replace with your property
            slider.Setter = setter; // Replace with your property
            slider.Writer = writer; // Replace with your property
            slider.Visible = CustomVisibleCondition;
            slider.Enabled = (b) => true; // or your custom condition
            slider.SupportsMultipleBlocks = true;

            MyAPIGateway.TerminalControls.AddControl<IMyConveyorSorter>(slider);
            return slider;
        }

        static bool CustomVisibleCondition(IMyTerminalBlock b)
        {
            // only visible for the blocks having this gamelogic comp
            return b?.GameLogic?.GetAs<SorterWeaponLogic>() != null;
        }
    }
}
