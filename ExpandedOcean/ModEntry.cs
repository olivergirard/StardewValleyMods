using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EnhancedWhenSpinGame
{
    public class ModEntry : Mod
    {

        /* Entry point. */

        public override void Entry(IModHelper helper)
        {

            helper.Events.GameLoop.SaveLoaded += this.Debug;
            helper.Events.Content.AssetRequested += this.LoadWheelAssets;
        }
    }
}
