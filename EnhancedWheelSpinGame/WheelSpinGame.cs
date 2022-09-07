using StardewModdingAPI;
using System;
using StardewValley;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;

namespace EnhancedWheelSpinGame
{

    public class WheelSpinGame
    {
        private static IMonitor Monitor;
        private static int optionPicked;
        private static double arrowRotation;
        private static SparklingText resultText;
        private static bool doneSpinning;
        private static int timerBeforeStart;
        private static double arrowRotationDeceleration;
        private static double arrowRotationVelocity;

        private static Response[] colors = new Response[5]
        {
            new Response("Orange", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1645")),
            new Response("Green", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1647")),
            new Response("Blue", "Blue"),
            new Response("Pink", "Pink"),
            new Response("I", Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1650"))
        };

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
            doneSpinning = false;
            timerBeforeStart = 1000;
            arrowRotationDeceleration = -0.00062831853071795862;
            arrowRotationVelocity = getArrowRotationVelocity();
            arrowRotation = 0;

            if (timerBeforeStart <= 0)
            {
                double oldVelocity = arrowRotationVelocity;
                arrowRotationVelocity += arrowRotationDeceleration;
                if (arrowRotationVelocity <= Math.PI / 80.0 && oldVelocity > Math.PI / 80.0)
                {
                    if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.3196898986859651 && Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 15f))
                    {
                        arrowRotationVelocity = Math.PI / 48.0;
                        Game1.playSound("dwop");
                    }
                    else if ((arrowRotation + Math.PI) % (Math.PI * 2.0) <= 4.3196898986859651 && Game1.random.NextDouble() < (double)((float)Game1.player.LuckLevel / 20f))
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

                    if (color == optionPicked)
                    {
                        won = true;
                    }

                    if (won)
                    {
                        Game1.playSound("reward");
                        resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11829"), Color.Lime, Color.White);
                    }
                    else
                    {
                        resultText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:WheelSpinGame.cs.11830"), Color.Red, Color.Transparent);
                        Game1.playSound("fishEscape");
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

        public static bool WheelDialogue(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            int tileIndex = Game1.currentLocation.getTileIndexAt(tileLocation.X, tileLocation.Y, "Buildings");

            if ((tileIndex == 308) || (tileIndex == 309))
            {

                Game1.currentLocation.createQuestionDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Event.cs.1652")), colors, delegate (Farmer _, string answer)
                {
                    switch (answer)
                    {
                        case "Orange":
                            optionPicked = 1;
                            break;
                        case "Green":
                            optionPicked = 2;
                            break;
                        case "Blue":
                            optionPicked = 3;
                            break;
                        case "Pink":
                            optionPicked = 4;
                            break;
                        case "I":
                            optionPicked = 5;
                            break;
                    }

                    Monitor.Log("" + optionPicked, LogLevel.Error);
                    AnswerDialogue("wheelBet", optionPicked);
                });

                return false;

            }
            else
            {
                return true;
            }
        }

        public static void AnswerDialogue(string questionKey, int answerChoice)
        {

            if (answerChoice == 5)
            {
                Game1.activeClickableMenu.emergencyShutDown();
            }
            else 
            {
                var instance = new Event();
                instance.answerDialogue("wheelBet", 0);
            }

        }

        /* 1 = orange, 2 = green, 3 = blue, 4 = pink */

        public static int GetColor()
        {

            // making sure 0 <= arrowRotation < 360

            while (arrowRotation >= 360 * (Math.PI / 180))
            {
                arrowRotation -= 360 * (Math.PI / 180);
            }


            if (arrowRotation < Math.PI)
            {
                //top half of wheel

                if (arrowRotation < 45 * (Math.PI / 180))
                {
                    return 4;
                }
                else if (arrowRotation < 90 * (Math.PI / 180))
                {
                    return 3;
                }
                else if (arrowRotation < 135 * (Math.PI / 180))
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                //bottom half of wheel

                if (arrowRotation < 225 * (Math.PI / 180))
                {
                    return 4;
                }
                else if (arrowRotation < 270 * (Math.PI / 180) / 180)
                {
                    return 3;
                }
                else if (arrowRotation < 315 * (Math.PI / 180))
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
