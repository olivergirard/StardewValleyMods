using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using StardewValley.Menus;
using StardewValley.Tools;
using System.Drawing;
using xTile.Dimensions;
using System.Runtime.CompilerServices;
using System.Reflection.Metadata.Ecma335;

namespace ExpandedOcean
{
    public class ModEntry : Mod
    {
        public static IModContentHelper modContentHelper = null;

        public static bool fishDataAdded = false;
        public static bool objectDataAdded = false;
        public static bool locationDataAdded = false;

        public static int myIndexOfAnimation = 0;
        public static float myRotation = 0;
        public static Vector2 myPosition = new Vector2(514f, 306f);
        public static int animationTimer = 65;
        public static bool isDarting = false;
        public static float targetRotation = 0;
        public static float dartingTimer = 0;

        public static int fishID;
        public static Texture2D fishTexture;
        public static StardewValley.Object fishObject;

        public override void Entry(IModHelper helper)
        {

            var harmony = new Harmony(this.ModManifest.UniqueID);

            /* Allows for new fish to be added to available fish queue. */

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SpecialFish))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawWhenHeld)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawWhenHeld))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawInMenu), new Type[] {typeof(SpriteBatch) , typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Microsoft.Xna.Framework.Color), typeof(bool)}),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawInInventoryMenu))
            );


            helper.Events.Content.AssetRequested += AddFishData;

            modContentHelper = helper.ModContent;

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

                        //TODO change chimaera time back to 1900 2600. seasons changed from spring summer for chimaera

                        editor.Data.Add(936, "Ocean Sunfish/70/smooth/70/121/600 1900/summer fall/both/ /3/.2/.3/5");
                        editor.Data.Add(937, "Chimaera/80/sinker/24/80/600 2600/spring summer winter/both/ /1/1/1/7");
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
                         * More info here: https://stardewcommunitywiki.com/Modding:Object_data 
                         * Trash goes under "Fish -20" */

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
                        string fallInfo = "129 -1 131 -1 148 -1 150 -1 152 -1 154 -1 155 -1 705 -1 701 -1 936 -1/";

                        //TODO remove Chimaera from winterInfo //add: "708 -1 130 -1 131 -1 146 -1 147 -1 150 -1 151 -1 152 -1 154 -1 705 -1 "
                        string winterInfo = "937 -1/";

                        string updatedBeach = forageInfo + springInfo + summerInfo + fallInfo + winterInfo + artifactData;

                        editor.Data["Beach"] = updatedBeach;
                    });

                    locationDataAdded = true;
                }
            }
        }

        /* Determines if the fish accessed is one of the new fish added. Sets the object in question to the fish. */

        //TODO either add onto this method or make a new method (depending on what the game code uses) to accomodate trash

        public static StardewValley.Object SpecialFish(StardewValley.Object result)
        {

            if (result.Name.Equals("Chimaera"))
            {
                fishID = 937;
                fishTexture = modContentHelper.Load<Texture2D>("assets/Chimaera.png");
                fishObject = new StardewValley.Object(937, 1);
                return fishObject;
            }

            return result;
        }

        /* Draws the fish when it is held, not immediately after catch. */

        public static bool DrawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {

            if (f.ActiveObject.ParentSheetIndex == 937)
            {
                
                Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16);

                spriteBatch.Draw(fishTexture, objectPosition, rectangle, Microsoft.Xna.Framework.Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingY() + 3) / 10000f));
                
                if (!(Math.Abs(Game1.starCropShimmerPause) <= 0.05f) || !(Game1.random.NextDouble() < 0.97))
                {
                    Game1.starCropShimmerPause += 0.04f;
                    if (Game1.starCropShimmerPause >= 0.8f)
                    {
                        Game1.starCropShimmerPause = -0.8f;
                    }
                }

                return true;
            }

            return false;
        }

        /* Draws the fish in the dock AND inventory menus... I think... */

        public static bool DrawInInventoryMenu(StardewValley.Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Microsoft.Xna.Framework.Color color, bool drawShadow)
        {

            /* If the fish is a Chimaera. */
            if (__instance.ParentSheetIndex == 937)
            {
                spriteBatch.Draw(modContentHelper.Load<Texture2D>("assets/Chimaera.png"), location + new Vector2((int)(32f * scaleSize), (int)(32f * scaleSize)), new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);

                /* "Draws" the number of items that are in a stack. */
                bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && 999 > 1 && __instance.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && __instance.Stack != int.MaxValue;
                if (shouldDrawStackNumber)
                {
                    Utility.drawTinyDigits(__instance.Stack, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 1f), 3f * scaleSize, 1f, color);
                }

                /* Draws the quality star. */
                if (drawStackNumber != 0 && (int)__instance.Quality > 0)
                {
                    Microsoft.Xna.Framework.Rectangle quality_rect = ((int)__instance.Quality < 4) ? new Microsoft.Xna.Framework.Rectangle(338 + ((int)__instance.Quality - 1) * 8, 400, 8, 8) : new Microsoft.Xna.Framework.Rectangle(346, 392, 8, 8);
                    Texture2D quality_sheet = Game1.mouseCursors;
                    float yOffset = ((int)__instance.Quality < 4) ? 0f : (((float)Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);
                    spriteBatch.Draw(quality_sheet, location + new Vector2(12f, 52f + yOffset), quality_rect, color * transparency, 0f, new Vector2(4f, 4f), 3f * scaleSize * (1f + yOffset), SpriteEffects.None, layerDepth);
                }

                return true;
            }

            return false;
        }

        //TODO fish needs to be drawn immediately after catch and during eating animation.
        //TODO fish needs to be drawn in collection menu
    }
}
