using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TackyMobs
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.SaveLoaded += this.SlimeColorChanger;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Characters/Monsters/Ghost"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    e.LoadFromModFile<IAssetDataForImage>("assets/Ghost.png", AssetLoadPriority.Medium);
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Characters/Monsters/Carbon Ghost"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsImage();
                    e.LoadFromModFile<IAssetDataForImage>("assets/Carbon Ghost.png", AssetLoadPriority.Medium);
                });
            }
        }

        private void SlimeColorChanger(object sender, SaveLoadedEventArgs e)
        {

            //this may not work, its from autoquarrymod
            StardewValley.Object monster;

            //do the same for loop struct thing?

        }
    }
}
