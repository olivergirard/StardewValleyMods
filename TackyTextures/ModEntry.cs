using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TackyTextures
{
    public class ModEntry : Mod
    {
        /* Entry function. */
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.AssetFilter;
        }

        /* Helps to filter the assets by what folder they are in. Saves time. */

        private void AssetFilter(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.StartsWith("Characters/Monsters/", false, true)) {
                LoadMonsterAsset(sender, e);
            }
        }

        /* Loads the asset belonging to a monster. */

        private void LoadMonsterAsset(object sender, AssetRequestedEventArgs e)
        {
            //TODO: double-check names once all done

            if (e.Name.IsEquivalentTo("Characters/Monsters/Armored Bug"))
            {
                e.LoadFromModFile<Texture2D>("assets/Armored Bug.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Armored Bug_dangerous"))
            {
                e.LoadFromModFile<Texture2D>("assets/Armored Bug_dangerous.PNG", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Bat"))
            {
                e.LoadFromModFile<Texture2D>("assets/Bat.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Bat_dangerous"))
            {
                e.LoadFromModFile<Texture2D>("assets/Bat_dangerous.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Big Slime"))
            {
                e.LoadFromModFile<Texture2D>("assets/Big Slime.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Blue Squid"))
            {
                e.LoadFromModFile<Texture2D>("assets/Blue Squid.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Bug"))
            {
                e.LoadFromModFile<Texture2D>("assets/Bug.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Bug_dangerous"))
            {
                e.LoadFromModFile<Texture2D>("assets/Bug_dangerous.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Carbon Ghost"))
            {
                e.LoadFromModFile<Texture2D>("assets/Carbon Ghost.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Ghost"))
            {
                e.LoadFromModFile<Texture2D>("assets/Ghost.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Grub"))
            {
                e.LoadFromModFile<Texture2D>("assets/Grub.png", AssetLoadPriority.Medium);
            }
        }
    }
}
