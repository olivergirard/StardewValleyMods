using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoQuarry
{
    public class ModEntry : Mod
    {
        
        /* Entry function. */

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.CleanOutQuarry;
        }

        /* Determines location of next tile to remove object from if applicable. */

        private void CleanOutQuarry(object sender, DayStartedEventArgs e)
        {
            GameLocation mountain = Game1.getLocationFromName("Mountain");
            StardewValley.Object obby;

            /* Quarry coordinates determined manually. Subject to change.*/

            for (int x = 106; x < 128; x++)
            {
                for (int y = 13; y < 37; y++)
                {
                    obby = mountain.getObjectAtTile(x, y);
                   
                    if (obby != null)
                    {
                        AddToInventory(obby);
                    }
                }
            }
        }

        /* Determines what resource and how much of it to add to the player's inventory. */

        private void AddToInventory(StardewValley.Object obby)
        {
            Game1.getLocationFromName("Mountain").removeObject(obby.TileLocation, false);

            int resourceID = 0;
            int resourceCount = 1;
            int prismaticShardChance;

            var rand = new Random();

            /* cases 2, 4, 6, 8, 10, 14: diamond, ruby, jade, amethyst, topaz, emerald, aquamarine nodes
             * cases 290, 751, 764: iron, copper, gold nodes
             * case 44: gem node
             * case 46: mystic node
             * case 765: iridium node
             * cases 32-847: different types of stones
             * cases 752-758: different types of boulders 
             */

            switch (obby.ParentSheetIndex)
            {
                case 2:
                    resourceID = 72;
                    break;
                case 4:
                    resourceID = 64;
                    break;
                case 6:
                    resourceID = 70;
                    break;
                case 8: 
                    resourceID = 66;
                    break;
                case 10:
                    resourceID = 68;
                    break;
                case 12:
                    resourceID = 60;
                    break;
                case 14:
                    resourceID = 62;
                    break;
                case 290:
                    resourceID = 380;
                    resourceCount = rand.Next(1, 4);
                    break;
                case 751:
                    resourceID = 378;
                    resourceCount = rand.Next(1, 4);
                    break;
                case 764:
                    resourceID = 384;
                    resourceCount = rand.Next(1, 4);
                    break;
                case 44:
                    resourceID = GemFromGemNode();
                    break;
                case 46:
                    resourceCount = rand.Next(1, 4);
                    Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(386, resourceCount, false, -1, 0));

                    prismaticShardChance = rand.Next(1, 101);
                    if (prismaticShardChance <= 25)
                    {
                        Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(74, 1, false, -1, 0));
                    }

                    resourceID = 384;
                    resourceCount = rand.Next(1, 5);
                    break;
                case 765:

                    prismaticShardChance = rand.Next(1, 101);
                    if (prismaticShardChance <= 4)
                    {
                        Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(74, 1, false, -1, 0));
                    }

                    resourceID = 386;
                    resourceCount = rand.Next(1, 6);
                    break;
                case 32:
                case 34:
                case 36:
                case 38:
                case 40:
                case 42:
                case 48:
                case 50:
                case 52:
                case 54:
                case 56:
                case 58:
                case 343:
                case 450:
                case 668:
                case 670:
                case 760:
                case 762:
                case 845:
                case 846:
                case 847:
                    resourceID = 390;
                    resourceCount = rand.Next(1, 4);
                    break;
                case 752:
                case 754:
                case 756:
                case 758:
                    resourceID = 390;
                    resourceCount = 10;
                    break;
            }

            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(resourceID, resourceCount, false, -1, 0));
        }

        /* Randomly determines the gem received from a Gem Node. */
        private int GemFromGemNode()
        {
            int gemID = 0;
            var rand = new Random();
            int randomGem = rand.Next(1, 9);

            switch (randomGem) 
            {
                case 1:
                    gemID = 72;
                    break;
                case 2:
                    gemID = 64;
                    break;
                case 3:
                    gemID = 70;
                    break;
                case 4: 
                    gemID = 66;
                    break;
                case 5:
                    gemID = 68;
                    break;
                case 6:
                    gemID = 60;
                    break;
                case 7:
                    gemID = 62;
                    break;
                case 8:
                    gemID = 74;
                    break;
            }

            return gemID;
        }
    }
}