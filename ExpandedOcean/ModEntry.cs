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

            //TODO: give these fish locations

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();

                    /* Any key above 935 should work as of v1.5 update.
                     * Format: Name/Difficulty/Behavior/MinimumSize/MaximumSize/StartTime EndTime/Seasons/RainyOrSunnyWeather/UNUSED/MinimumDepth/SpawnMultiplier/DepthMultiplier/MinimumLevelNeeded
                     * More info here: https://stardewcommunitywiki.com/Modding:Fish_data */

                    //TODO Edit values to MinimumDepth = 1 SpawnMultiplier = 1 DepthMultiplier = 1 during debugging. Change seasons/time if necessary.

                    editor.Data.Add("936", "Ocean Sunfish/70/smooth/70/121/600 1900/summer fall/both/ /3/.2/.3/0");
                    editor.Data.Add("937", "Chimaera/80/sinker/24/80/1900 2600/spring summer/both/ /4/.3/.3/5");
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();

                    /* Format: Name/Price/Edibility/TypeAndCategory/DisplayName/Description/TimesAvailable^SeasonsAvailable
                     * More info here: https://stardewcommunitywiki.com/Modding:Object_data */

                    editor.Data.Add("936", "Ocean Sunfish/800/15/Fish -4/Ocean Sunfish/A very heavy and very bony fish./Day Night^Summer Fall");
                    editor.Data.Add("937", "Chimaera/500/2/Fish -4/Chimaera/A bottom-dwelling, deep-sea fish./Night^Spring Summer");
                });
            }
        }

        /* Determines if the fish accessed is one of the new fish added. */

        public Object SpecialFish(ref Object __result)
        {
            Texture2D fish = null;

            if (__result.Name.Equals("Chimaera"))
            {
                fish = Texture2D.FromFile(Game1.graphics.GraphicsDevice, "assets/Chimaera.png");
                Game1.spriteBatch.Draw(fish, Game1.GlobalToLocal(Game1.viewport, Game1.player.Position + new Vector2(0f, -56f)), Color.White);
            }

            return __result;
        }
    }
}
