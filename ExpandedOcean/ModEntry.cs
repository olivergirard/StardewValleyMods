using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;

namespace ExpandedOcean
{
    public class ModEntry : Mod
    {
        public static bool fishDataAdded = false;
        public static bool objectDataAdded = false;
        public static bool locationDataAdded = false;


        /* Entry point. */

        public override void Entry(IModHelper helper)
        {

            var harmony = new Harmony(this.ModManifest.UniqueID);

            /* Allows for new fish to be added to available fish queue. */

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SpecialFish))
            );

            helper.Events.Content.AssetRequested += AddFishData;
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

                        //TODO Edit values to MinimumDepth = 1 SpawnMultiplier = 1 DepthMultiplier = 1 during debugging. Change seasons/time if necessary.

                        //editor.Data.Add("936", "Ocean Sunfish/70/smooth/70/121/600 1900/summer fall/both/ /3/.2/.3/0");
                        editor.Data.Add(937, "Chimaera/80/sinker/24/80/1900 2600/spring summer/both/ /1/1/1/5");
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

                        //editor.Data.Add("936", "Ocean Sunfish/800/15/Fish -4/Ocean Sunfish/A very heavy and very bony fish./Day Night^Summer Fall");
                        editor.Data.Add(937, "Chimaera/500/2/Fish -4/Chimaera/A bottom-dwelling, deep-sea fish./Night^Spring Summer");
                    });

                    objectDataAdded = true;
                }
            }

            if (fishDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, string>();

                        /* Original value: "372 .9 718 .1 719 .3 723 .3/372 .9 394 .5 718 .1 719 .3 723 .3/372 .9 718 .1 719 .3 723 .3/372 .4 392 .8 718 .05 719 .2 723 .2/129 -1 131 -1 147 -1 148 -1 152 -1 708 -1 267 -1/128 -1 130 -1 146 -1 149 -1 150 -1 152 -1 155 -1 708 -1 701 -1 267 -1/129 -1 131 -1 148 -1 150 -1 152 -1 154 -1 155 -1 705 -1 701 -1/708 -1 130 -1 131 -1 146 -1 147 -1 150 -1 151 -1 152 -1 154 -1 705 -1/384 .08 589 .09 102 .15 390 .25 330 1"
                         * More info here: https://stardewvalleywiki.com/Modding:Location_data */

                        string beachInfo = "372 .9 718 .1 719 .3 723 .3/372 .9 394 .5 718 .1 719 .3 723 .3/372 .9 718 .1 719 .3 723 .3/";
                        string artifactData = "384 .08 589 .09 102 .15 390 .25 330 1";

                        /* These are the only strings that need to be edited. */

                        string springInfo = "129 -1 131 -1 147 -1 148 -1 152 -1 708 -1 267 -1 937 -1/";
                        string summerInfo = "128 -1 130 -1 146 -1 149 -1 150 -1 152 -1 155 -1 708 -1 701 -1 267 -1 937 -1/";
                        string fallInfo = "129 - 1 131 - 1 148 - 1 150 - 1 152 - 1 154 - 1 155 - 1 705 - 1 701 - 1/";
                        string winterInfo = "708 - 1 130 - 1 131 - 1 146 - 1 147 - 1 150 - 1 151 - 1 152 - 1 154 - 1 705 - 1/";


                        string updatedBeach = beachInfo + springInfo + summerInfo + fallInfo + winterInfo + artifactData;

                        editor.Data["Beach"] = updatedBeach;
                    });

                    fishDataAdded = true;

                }
            }
        }

        /* Determines if the fish accessed is one of the new fish added. */

        public static Object SpecialFish(Object result)
        {
            Texture2D fish = null;

            if (result.Name.Equals("Chimaera"))
            {
                fish = Texture2D.FromFile(Game1.graphics.GraphicsDevice, "assets/Chimaera.png");
                Game1.spriteBatch.Draw(fish, Game1.GlobalToLocal(Game1.viewport, Game1.player.Position + new Vector2(0f, -56f)), Color.White);
                return new Object(937, 1);
            }

            return result;

        }
    }
}
