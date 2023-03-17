using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using StardewValley.Monsters;
using StardewValley.Tools;
using System.Runtime.CompilerServices;
using StardewValley.Menus;

namespace ExpandedOcean
{
    public class ModEntry : Mod
    {
        public static IModHelper modHelper = null;

        public static bool fishDataAdded = false;
        public static bool objectDataAdded = false;
        public static bool locationDataAdded = false;

        public static int myIndexOfAnimation = 0;
        public static float myRotation = 0;
        public static Vector2 myPosition = new Vector2(514f, 306f);
        public static int fishID;

        public static int animationTimer = 65;
        public static bool isDarting = false;
        public static float targetRotation = 0;
        public static float dartingTimer = 0;

        public override void Entry(IModHelper helper)
        {

            var harmony = new Harmony(this.ModManifest.UniqueID);

            /* Allows for new fish to be added to available fish queue. */

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SpecialFish))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.Fish), nameof(StardewValley.Menus.Fish.draw)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawFish))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.Fish), nameof(StardewValley.Menus.Fish.Update)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.InitializeValues))
            );


            helper.Events.Content.AssetRequested += AddFishData;

            modHelper = helper;
        }

        /* Adds fish information to game files. */

        private void AddFishData(object sender, AssetRequestedEventArgs e)
        {

            if (fishDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<int, string>();

                        /* Any key above 935 should work as of v1.5 update.
                         * Format: Name/Difficulty/Behavior/MinimumSize/MaximumSize/StartTime EndTime/Seasons/RainyOrSunnyWeather/UNUSED/MinimumDepth/SpawnMultiplier/DepthMultiplier/MinimumLevelNeeded
                         * More info here: https://stardewcommunitywiki.com/Modding:Fish_data */

                        //TODO time changed from 1900 2600. seasons changed from spring summer for chimaera
                        //TODO sunfish last lines: /both/ /3/.2/.3/0

                        editor.Data.Add(936, "Ocean Sunfish/70/smooth/70/121/600 1900/summer fall/both/ /1/1/1/5");
                        editor.Data.Add(937, "Chimaera/80/sinker/24/80/600 2600/spring summer winter/both/ /1/1/1/5");
                    });

                    fishDataAdded = true;

                }
            }

            if (objectDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<int, string>();

                        /* Format: Name/Price/Edibility/TypeAndCategory/DisplayName/Description/TimesAvailable^SeasonsAvailable
                         * More info here: https://stardewcommunitywiki.com/Modding:Object_data */

                        editor.Data.Add(936, "Ocean Sunfish/800/15/Fish -4/Ocean Sunfish/A very heavy and very bony fish./Day Night^Summer Fall");

                        //TODO chimaera is Night^Spring Summer
                        editor.Data.Add(937, "Chimaera/500/2/Fish -4/Chimaera/A bottom-dwelling, deep-sea fish./Day Night^Spring Summer Winter");
                    });

                    objectDataAdded = true;
                }
            }

            if (locationDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, string>();

                        /* Original value: "372 .9 718 .1 719 .3 723 .3/372 .9 394 .5 718 .1 719 .3 723 .3/372 .9 718 .1 719 .3 723 .3/372 .4 392 .8 718 .05 719 .2 723 .2/129 -1 131 -1 147 -1 148 -1 152 -1 708 -1 267 -1/128 -1 130 -1 146 -1 149 -1 150 -1 152 -1 155 -1 708 -1 701 -1 267 -1/129 -1 131 -1 148 -1 150 -1 152 -1 154 -1 155 -1 705 -1 701 -1/708 -1 130 -1 131 -1 146 -1 147 -1 150 -1 151 -1 152 -1 154 -1 705 -1/384 .08 589 .09 102 .15 390 .25 330 1"
                         * More info here: https://stardewvalleywiki.com/Modding:Location_data */

                        string forageInfo = "372 .9 718 .1 719 .3 723 .3/372 .9 394 .5 718 .1 719 .3 723 .3/372 .9 718 .1 719 .3 723 .3/372 .4 392 .8 718 .05 719 .2 723 .2/";
                        string artifactData = "384 .08 589 .09 102 .15 390 .25 330 1";

                        /* These are the only strings that need to be edited. */

                        string springInfo = "129 -1 131 -1 147 -1 148 -1 152 -1 708 -1 267 -1 937 -1/";
                        string summerInfo = "128 -1 130 -1 146 -1 149 -1 150 -1 152 -1 155 -1 708 -1 701 -1 267 -1 937 -1 936 -1/";

                        //TODO remove chimaera from fallInfo

                        string fallInfo = "129 -1 131 -1 148 -1 150 -1 152 -1 154 -1 155 -1 705 -1 701 -1 936 -1 /";
                        string winterInfo = "708 -1 130 -1 131 -1 146 -1 147 -1 150 -1 151 -1 152 -1 154 -1 705 -1 937 -1/";

                        string updatedBeach = forageInfo + springInfo + summerInfo + fallInfo + winterInfo + artifactData;

                        editor.Data["Beach"] = updatedBeach;
                    });

                    locationDataAdded = true;
                }
            }
        }

        /* Determines if the fish accessed is one of the new fish added. */

        public static StardewValley.Object SpecialFish(StardewValley.Object result)
        {
            if (result.Name.Equals("Chimaera"))
            {
                fishID = 937;
                return new StardewValley.Object(937, 1);
            }
            else if (result.Name.Equals("Ocean Sunfish"))
            {
                fishID = 936;
                return new StardewValley.Object(936, 1);
            }

            return result;
        }

        /* Determines whether the base game code or the modified base game code should be run. */

        public static bool InitializeValues(GameTime time)
        {
            string data = null;
            if (fishID == 936)
            {
                data = "Ocean Sunfish/70/smooth/70/121/600 1900/summer fall/both/ /1/1/1/5";
                string[] rawData = data.Split('/');
                Update(time, rawData);
            }
            else if (fishID == 937)
            {
                data = "Chimaera/80/sinker/24/80/600 2600/spring summer winter/both/ /1/1/1/5";
                string[] rawData = data.Split('/');
                Update(time, rawData);
            }
            else
            {
                return true;
            }

            return false;
        }

        /* Performs the same functions as the code from base game, now with obtainable variables. */

        public static void Update(GameTime time, string[] rawData)
        {

            float chanceToDart = Convert.ToInt32(rawData[1]);
            float dartingRandomness = Convert.ToInt32(rawData[2]);
            float dartingDuration = Convert.ToInt32(rawData[4]);
            float turnFrequency = Convert.ToInt32(rawData[5]);
            float turnSpeed = Convert.ToInt32(rawData[6]);
            float turnIntensity = Convert.ToInt32(rawData[7]);
            float minSpeed = Convert.ToInt32(rawData[8]);
            float maxSpeed = Convert.ToInt32(rawData[9]);
            float speedChangeFrequency = Convert.ToInt32(rawData[10]);

            float targetSpeed = minSpeed / 50f;
            float currentSpeed = 0;
            Rectangle fishingField = new Rectangle(0, 0, 1028, 612);

            animationTimer -= time.ElapsedGameTime.Milliseconds;
            if (animationTimer <= 0)
            {
                animationTimer = 65 - (int)(currentSpeed * 10f);
                myIndexOfAnimation = (myIndexOfAnimation + 1) % 8;
            }
            if (!isDarting && Game1.random.NextDouble() < (double)(chanceToDart / 10000f))
            {
                myRotation += (float)((double)Game1.random.Next(-(int)dartingRandomness, (int)dartingRandomness) * Math.PI / 100.0);
                targetSpeed = myRotation;
                dartingTimer = dartingDuration * 10f + (float)Game1.random.Next(-(int)dartingDuration, (int)dartingDuration) * 0.1f;
                isDarting = true;
            }
            if (dartingTimer > 0f)
            {
                dartingTimer -= time.ElapsedGameTime.Milliseconds;
                if (dartingTimer <= 0f && isDarting)
                {
                    isDarting = false;
                    dartingTimer = dartingDuration * 10f + (float)Game1.random.Next(-(int)dartingDuration, (int)dartingDuration) * 0.1f;
                }
            }
            if (Game1.random.NextDouble() < (double)(turnFrequency / 10000f))
            {
                targetRotation = (float)((double)((float)Game1.random.Next((int)(0f - turnIntensity), (int)turnIntensity) / 100f) * Math.PI);
            }
            if (Game1.random.NextDouble() < (double)(speedChangeFrequency / 10000f))
            {
                targetSpeed = (int)((float)Game1.random.Next((int)minSpeed, (int)maxSpeed) / 20f);
            }
            if (Math.Abs(myRotation - targetRotation) > Math.Abs(targetRotation / (100f - turnSpeed)))
            {
                myRotation += targetRotation / (100f - turnSpeed);
            }

            myRotation %= (float)Math.PI * 2f;
            currentSpeed += (targetSpeed - currentSpeed) / 10f;
            currentSpeed = Math.Min(maxSpeed / 20f, currentSpeed);
            currentSpeed = Math.Max(minSpeed / 20f, currentSpeed);
            myPosition.X += (float)((double)currentSpeed * Math.Cos(myRotation));
            int wallsHit = 0;

            if (!fishingField.Contains(new Rectangle((int)myPosition.X - 32, (int)myPosition.Y - 32, 64, 64)))
            {
                Vector2 cartesian = new Vector2(currentSpeed * (float)Math.Cos(myRotation), currentSpeed * (float)Math.Sin(myRotation));
                cartesian.X = 0f - cartesian.X;
                myRotation = (float)Math.Atan(cartesian.Y / cartesian.X);
                if (cartesian.X < 0f)
                {
                    myRotation += (float)Math.PI;
                }
                else if (cartesian.Y < 0f)
                {
                    myRotation += (float)Math.PI / 2f;
                }
                myPosition.X += (float)((double)currentSpeed * Math.Cos(myRotation));
                wallsHit++;
            }
            myPosition.Y += (float)((double)currentSpeed * Math.Sin(myRotation));
            if (!fishingField.Contains(new Rectangle((int)myPosition.X - 32, (int)myPosition.Y - 32, 64, 64)))
            {
                Vector2 cartesian2 = new Vector2(currentSpeed * (float)Math.Cos(myRotation), currentSpeed * (float)Math.Sin(myRotation));
                cartesian2.Y = 0f - cartesian2.Y;
                myRotation = (float)Math.Atan(cartesian2.Y / cartesian2.X);
                if (cartesian2.X < 0f)
                {
                    myRotation += (float)Math.PI;
                }
                else if (cartesian2.Y > 0f)
                {
                    myRotation += (float)Math.PI / 2f;
                }
                myPosition.Y += (float)((double)currentSpeed * Math.Sin(myRotation));
                wallsHit++;
            }
            if (wallsHit >= 2)
            {
                Vector2 targetLocation = Utility.getVelocityTowardPoint(new Point((int)myPosition.X, (int)myPosition.Y), new Vector2(514f, 306f), currentSpeed);
                myRotation = (float)Math.Atan(targetLocation.Y / targetLocation.X);
                if (targetLocation.X < 0f)
                {
                    myRotation += (float)Math.PI;
                }
                else if (targetLocation.Y < 0f)
                {
                    myRotation += (float)Math.PI / 2f;
                }
                myPosition.X += (float)((double)currentSpeed * Math.Cos(myRotation));
                myPosition.Y += (float)((double)currentSpeed * Math.Sin(myRotation));
            }
            else if (wallsHit == 1)
            {
                targetRotation = myRotation;
            }
        }

        /* Draws the fish in the player's hand after catching it. */

        public static void DrawFish(SpriteBatch b, Vector2 positionOfFishingField)
        {
            b.Draw(Game1.mouseCursors, myPosition + positionOfFishingField, new Rectangle(561, 1846 + myIndexOfAnimation * 16, 16, 16), Color.White, myRotation + (float)Math.PI / 2f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.5f);
        }
    }
}
