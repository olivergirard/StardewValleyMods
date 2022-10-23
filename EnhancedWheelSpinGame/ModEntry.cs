using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;

namespace EnhancedWhenSpinGame
{
    public class ModEntry : Mod
    {

        /* Entry point. */

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            /* edits the wheel game itself */

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Menus.WheelSpinGame), nameof(StardewValley.Menus.WheelSpinGame.update)),
               prefix: new HarmonyMethod(typeof(EnhancedWheelSpinGame.WheelSpinGame), nameof(EnhancedWheelSpinGame.WheelSpinGame.update))
            );

            /* edits the color options before the wheel game begins */

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.checkAction)),
               prefix: new HarmonyMethod(typeof(EnhancedWheelSpinGame.WheelSpinGame), nameof(EnhancedWheelSpinGame.WheelSpinGame.WheelDialogue))
            );

            EnhancedWheelSpinGame.WheelSpinGame.Initialize(this.Monitor);

            helper.Events.Content.AssetRequested += this.LoadWheelAssets;
        }

        /* Changes the .png files that contain the wheel. */

        private void LoadWheelAssets(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("LooseSprites/Cursors"))
            {
                e.LoadFromModFile<Texture2D>("assets/Cursors.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Maps/Festivals"))
            {
                e.LoadFromModFile<Texture2D>("assets/Festivals.png", AssetLoadPriority.Medium);
            }

        }
    }
}
