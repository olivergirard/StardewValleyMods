using StardewModdingAPI;
using System;
using StardewValley;
using xTile.Dimensions;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;

namespace EnhancedWheelSpinGame {

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

            return getArrowRotationVelocity();
        }

        public static bool NewWheel(GameTime time)
        {
            try
            {

                NewWheel(time);
                if (timerBeforeStart > 0)
                {
                    timerBeforeStart -= time.ElapsedGameTime.Milliseconds;
                    if (timerBeforeStart <= 0)
                    {
                        //TODO make sure the sound is drawn from the right folder
                        Game1.playSound("cowboy_monsterhit");
                    }
                } else
                {
                    double oldVelocity = arrowRotationVelocity;
                    arrowRotationVelocity += arrowRotationDeceleration;

                    if (arrowRotationVelocity <= Math.PI / 80.0 && oldVelocity > Math.PI / 80.0)
                    {
                        int color = GetColor();

                        //"dwop" indicates wrong color chosen
                        //arrowRotationVelocity = Math.PI / 48.0; after any failure

                        if (optionPicked != color)
                        {
                            arrowRotationVelocity = Math.PI / 48.0;
                            Game1.playSound("dwop");
                        }

                        if ((arrowRotationVelocity <= 0.0) && (doneSpinning == false))
                        {
                            doneSpinning = true;
                            arrowRotationDeceleration = 0.0;
                            arrowRotationVelocity = 0.0;

                            bool won = false;
                            if (arrowRotation > Math.PI / 2.0 && arrowRotation <= 4.71238898038469)
                            {
                                if (optionPicked == color)
                                {
                                    won = true;
                                }
                            }
                            
                            if (won == true)
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

                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(NewWheel)}:\n{ex}", LogLevel.Error);
                return true;
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

        //TODO, this determines what color was landed on based on arrow rotation
        //0, 45, 90, 135, 180, 225, 270, 315, 360
        public int GetColor()
        {

        }

        public void Wage(int value)
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
