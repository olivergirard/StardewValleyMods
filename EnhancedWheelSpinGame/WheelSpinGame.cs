using StardewModdingAPI;
using System;
using StardewValley;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using StardewValley.Menus;
using System.Threading;

namespace EnhancedWheelSpinGame
{

    public class WheelSpinGame
    {
        private static IMonitor Monitor;
        private static int optionPicked;
        private static int wager;
        private static double arrowRotation;
        private static SparklingText resultText;
        private static bool doneSpinning = false;
        private static int timerBeforeStart = 1000;
        private static double arrowRotationDeceleration = -0.00062831853071795862;
        private static double arrowRotationVelocity = getArrowRotationVelocity();

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static double getArrowRotationVelocity()
        {
            double arrowRotationVelocity = Math.PI / 16.0;
            arrowRotationVelocity += (double)Game1.random.Next(0, 15) * Math.PI / 256.0;
            if (Game1.random.NextDouble() < 0.5)
            {
                arrowRotationVelocity += Math.PI / 64.0;
            }

            return arrowRotationVelocity;
        }

        public static void update(GameTime time)
        {
            if (timerBeforeStart <= 0)
            {
                double oldVelocity = arrowRotationVelocity;
                arrowRotationVelocity += arrowRotationDeceleration;
                if (arrowRotationVelocity <= Math.PI / 80.0 && oldVelocity > Math.PI / 80.0)
                {
                    bool colorChoiceGreen2 = Game1.currentLocation.currentEvent.specialEventVariable2;
                    if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.3196898986859651 && Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 15f))
                    {
                        if (colorChoiceGreen2)
                        {
                            arrowRotationVelocity = Math.PI / 48.0;
                            Game1.playSound("dwop");
                        }
                    }
                    else if ((arrowRotation + Math.PI) % (Math.PI * 2.0) <= 4.3196898986859651 && !colorChoiceGreen2 && Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 20f))
                    {
                        arrowRotationVelocity = Math.PI / 48.0;
                        Game1.playSound("dwop");
                    }
                }
                if (arrowRotationVelocity <= 0.0 && !doneSpinning)
                {
                    doneSpinning = true;
                    arrowRotationDeceleration = 0.0;
                    arrowRotationVelocity = 0.0;
                    int color = GetColor();
                    bool won = false;
                    if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.71238898038469)
                    {
                        if (color == optionPicked)
                        {
                            won = true;
                        }
                    }
                    else if (color == optionPicked)
                    {
                        won = true;
                    }
                    if (won)
                    {
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11829"), Color.Lime, Color.White);
                        Game1.player.festivalScore += wager;
                    }
                    else
                    {
                        resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11830"), Color.Red, Color.Transparent);
                        Game1.playSound("fishEscape");
                        Game1.player.festivalScore -= wager;
                    }
                }
                double num = arrowRotation;
                arrowRotation += arrowRotationVelocity;
                if (num % (Math.PI / 2.0) > arrowRotation % (Math.PI / 2.0))
                {
                    Game1.playSound("Cowboy_gunshot");
                }
                arrowRotation %= Math.PI * 2.0;
            }
            else
            {
                timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
                if (timerBeforeStart <= 0)
                {
                    Game1.playSound("cowboy_monsterhit");
                }
            }
            if (resultText != null && resultText.update(time))
            {
                resultText = null;
            }
            if (doneSpinning && resultText == null)
            {
                Game1.exitActiveMenu();
                Game1.player.canMove = true;
            }
        }

        //TODO fix blue option in menu so it persists
        public static bool AnswerDialogue(string questionKey, int answerChoice)
        {
            if (questionKey == "wheelBet")
            {
                if (answerChoice == 4)
                {
                    Game1.activeClickableMenu.emergencyShutDown();
                    return false;
                } else if (answerChoice == 2)
                {
                    var instance = new Event();
                    instance.answerDialogue("wheelBet", 0);

                    return false;
                } else
                {
                    return true;
                }
            }

            return true;
        }

        public static bool WheelDialogue(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
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

                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1652")), colors, "wheelBet");
                optionPicked = DetermineOptionPicked(colors);

                return false;

            } else
            {
                return true;
            }
        }

        public static int DetermineOptionPicked(Response[] colors)
        {

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

        //TODO, this determines what color was landed on based on arrow rotation
        //0, 45, 90, 135, 180, 225, 270, 315, 360

        /* 1 = orange, 2 = green, 3 = blue, 4 = pink, 5 = I */

        public static int GetColor()
        {

            if (arrowRotation <= 45)
            {
                return 4;

            }
            else if (arrowRotation <= 90)
            {
                return 3;
            }
            else if (arrowRotation <= 135)
            {
                return 2;
            }
            else if (arrowRotation <= 180)
            {
                return 1;
            }
            else if (arrowRotation <= 225)
            {
                return 4;
            }
            else if (arrowRotation <= 270)
            {
                return 3;
            }
            else if (arrowRotation <= 315)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public static void Wage(int value)
        {
            if (value <= Game1.player.festivalScore)
            {
                Game1.playSound("smallSelect");
                Game1.activeClickableMenu = new StardewValley.Menus.WheelSpinGame(value);
                wager = value;
            }
        }
    }
}
