using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using System.Threading;
using System;
using StardewValley;
using xTile.Dimensions;
using System.Collections.Generic;
using System.Reflection;
using StardewValley.Characters;
using System.Linq;

namespace EnhancedWheelSpinGame {

    internal class WheelSpinGame
    {
        private static IMonitor Monitor;
        private static int optionPicked;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        //TODO

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

        public bool WheelDialogue(Location tileLocation, Farmer player)
        {
            try
            {
                int tileIndex = Game1.currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings");

                if ((tileIndex == 308) || (tileIndex == 309))
                {
                    Response[] colors = new Response[5]
                        {
                        new Response("Orange", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1645")),
                        new Response("Green", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1647")),
                        new Response("Blue", "Blue"),
                        new Response("Pink", "Pink"),
                        new Response("I", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1650"))
                        };

                    if ((player.IsLocalPlayer) && (Game1.dayOfMonth.Equals(16)) && (Game1.currentSeason.Equals("Fall")))
                    {
                        Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1652")), colors, "wheelBet");

                        optionPicked = DetermineOptionPicked(colors);
                    }
                }

                return false;

            } catch (Exception)
            {
                return true;
            }
        }

        public int DetermineOptionPicked(Response[] colors) {

            int option;

            /* 1 = orange, 2 = green, 3 = blue, 4 = pink, 5 = I */
            
            if (Game1.currentLocation.answerDialogue(colors[0]))
            {
                option = 1;
            } 
            else if (Game1.currentLocation.answerDialogue(colors[1]))
            {
                option = 2;
            }
            else if (Game1.currentLocation.answerDialogue(colors[2]))
            {
                option = 3;
            }
            else if (Game1.currentLocation.answerDialogue(colors[3]))
            {
                option = 4;
            }
            else
            {
                option = 5;
            }

            return option;
        }



    }
}
