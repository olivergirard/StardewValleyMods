using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.WorldMaps;
using StardewValley.Menus;

namespace MapForEveryone
{

    public class ModEntry : Mod
    {
        public static IModContentHelper? modContentHelper;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.MapPage), nameof(StardewValley.Menus.MapPage.drawMiniPortraits)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.AddNPCPortraits))
            );

            modContentHelper = helper.ModContent;

        }

        public static void AddNPCPortraits(SpriteBatch b, MapPage __instance)
        {
            Dictionary<Vector2, int> usedPositions = new Dictionary<Vector2, int>();

            foreach (GameLocation location in Game1.locations)
            {
                foreach (NPC villager in location.characters)
                {

                    MapAreaPosition positionData = WorldMapManager.GetPositionData(villager.currentLocation, villager.TilePoint);

                    if (positionData != null && !(positionData.Region.Id != __instance.mapRegion.Id))
                    {
                        Vector2 position = positionData.GetMapPixelPosition(villager.currentLocation, villager.TilePoint);
                        position = new Vector2(position.X + (float)__instance.mapBounds.X - 32f, position.Y + (float)__instance.mapBounds.Y - 32f);
                        usedPositions.TryGetValue(position, out var count);
                        usedPositions[position] = count + 1;
                        if (count > 0)
                        {
                            position += new Vector2(48 * (count % 2), 48 * (count / 2));
                        }

                        if ((modContentHelper != null))
                        {
                            switch (villager.Name)
                            {
                                case "Alex":
                                case "Abigail":
                                case "Elliott":
                                case "Emily":
                                case "Haley":
                                case "Harvey":
                                case "Leah":
                                case "Maru":
                                case "Penny":
                                case "Sam":
                                case "Sebastian":
                                case "Shane":
                                    string fileName = "assets/" + villager.Name + ".png";
                                    Texture2D portrait = modContentHelper.Load<Texture2D>(fileName);
                                    b.Draw(portrait, position, new Rectangle(0, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}
