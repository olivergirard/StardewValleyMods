using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TackyMobs
{
    public class ModEntry : Mod
    {
        /* Entry function. */
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.LoadAsset;
        }

        private void LoadAsset(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo("Characters/Monsters/Ghost"))
            {
                e.LoadFromModFile<Texture2D>("assets/Ghost.png", AssetLoadPriority.Medium);
            } else if (e.Name.IsEquivalentTo("Characters/Monsters/Carbon Ghost"))
            {
                e.LoadFromModFile<Texture2D>("assets/Carbon Ghost.png", AssetLoadPriority.Medium);
            } else if (e.Name.IsEquivalentTo("Characters/Monsters/Grub"))
            {
                e.LoadFromModFile<Texture2D>("assets/Grub.png", AssetLoadPriority.Medium);
            } 
        }
    }
}
