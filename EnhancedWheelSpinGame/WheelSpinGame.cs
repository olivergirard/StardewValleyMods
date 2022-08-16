using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using System.Threading;
using System;
using StardewValley;
using xTile.Dimensions;

namespace EnhancedWheelSpinGame
{
    internal class WheelSpinGame
    {
        private static IMonitor Monitor;
        private static string colorPicked;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        //TODO change colors (array) to hold all colors
        //that picked color sets a class variable colorPicked to a color string blue, pink, green, or orange
        //run newwheel method based off of that

        public static bool NewWheel()
        {
            try
            {
                








                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(NewWheel)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool NewAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer farmer)
        {
            try
            {









                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(NewWheel)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
