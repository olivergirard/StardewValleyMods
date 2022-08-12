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
            if (e.Name.StartsWith("Characters/Monsters/", false, true))
            {
                LoadMonsterAsset(sender, e);
            }
        }

        /* Loads the asset belonging to a monster. */

        private void LoadMonsterAsset(object sender, AssetRequestedEventArgs e)
        {
            //TODO: double-check names once all done

            if (e.Name.IsEquivalentTo("Characters/Monsters/Angry Roger"))
            {
                e.LoadFromModFile<Texture2D>("assets/Angry Roger.png", AssetLoadPriority.Medium);
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
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Carbon Ghost"))
            {
                e.LoadFromModFile<Texture2D>("assets/Carbon Ghost.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Cat"))
            {
                e.LoadFromModFile<Texture2D>("assets/Cat.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Crow"))
            {
                e.LoadFromModFile<Texture2D>("assets/Crow.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Duggy"))
            {
                e.LoadFromModFile<Texture2D>("assets/Duggy.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Duggy_dangerous"))
            {
                e.LoadFromModFile<Texture2D>("assets/Duggy_dangerous.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Dwarvish Sentry"))
            {
                e.LoadFromModFile<Texture2D>("assets/Dwarvish Sentry.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/False Magma Cap"))
            {
                e.LoadFromModFile<Texture2D>("assets/False Magma Cap.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Fireball"))
            {
                e.LoadFromModFile<Texture2D>("assets/Fireball.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Fly"))
            {
                e.LoadFromModFile<Texture2D>("assets/Fly.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Fly_dangerous"))
            {
                e.LoadFromModFile<Texture2D>("assets/Fly_dangerous.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Frog"))
            {
                e.LoadFromModFile<Texture2D>("assets/Frog.png", AssetLoadPriority.Medium);
            }
            else if (e.Name.IsEquivalentTo("Characters/Monsters/Grub"))
            {
                e.LoadFromModFile<Texture2D>("assets/Grub.png", AssetLoadPriority.Medium);
            }
        }
    }
}