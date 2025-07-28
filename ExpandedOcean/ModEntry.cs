using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using StardewValley;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.IO;
using StardewValley.Tools;
using StardewValley.ItemTypeDefinitions;
using StardewValley.GameData.Locations;
using System.Collections.Generic;

namespace ExpandedOcean
{

    // TODO: Verify that fish can be caught naturally in the game, without forcing them in. 
    public class ModEntry : Mod
    {
        public static IModContentHelper modContentHelper = null;
        public static BindingFlags allMethods = BindingFlags.NonPublic;

        /* Used to ensure data is not added more than once. */
        public static bool fishDataAdded = false;
        public static bool objectDataAdded = false;
        public static bool locationDataAdded = false;

        public static int fishID;
        public static string textureName;
        public static StardewValley.Object fishObject;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            /* Function used for adding fish data to the game. */

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.SpecialFish))
            );

            /* Functions used for drawing the fish. */

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawWhenHeld)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawWhenHeld))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.drawInMenu), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Microsoft.Xna.Framework.Color), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DrawInInventoryMenu))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.showEatingItem)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Eat))
            );

            harmony.Patch(
                original: typeof(StardewValley.Tools.FishingRod).GetMethod("doPullFishFromWater", BindingFlags.Instance | BindingFlags.NonPublic),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DoPullFromWater))
            );

            harmony.Patch(
                original: typeof(TemporaryAnimatedSprite).GetMethod("loadTexture", BindingFlags.Instance | BindingFlags.NonPublic),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.LoadTexture))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Tools.FishingRod), nameof(StardewValley.Tools.FishingRod.draw)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FishFrame))
            );

            helper.Events.Content.AssetRequested += AddFishData;

            modContentHelper = helper.ModContent;

        }

        /* Adds fish information to game files. */
        private void AddFishData(object sender, AssetRequestedEventArgs e)
        {
            if (fishDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data\\Fish"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, string>();

                        editor.Data.Add("936", "Ocean Sunfish/30/smooth/70/121/600 1900/summer fall/both/690 .4 685 .1/3/.3/.3/5/false");
                        editor.Data.Add("937", "Sea Slug/20/smooth/1/24/600 2600/spring summer fall winter/both/690 .4 685 .1/5/.3/.2/3/false");
                        
                    });

                    fishDataAdded = true;
                }
            }

            if (objectDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data\\Objects"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, StardewValley.GameData.Objects.ObjectData>();

                        string texturePath = modContentHelper.GetInternalAssetName("assets/Ocean Sunfish.png").BaseName;
                        StardewValley.GameData.Objects.ObjectData oceanSunfish = new()
                        {
                            Name = "Ocean Sunfish",
                            DisplayName = "Ocean Sunfish",
                            Description = "A very heavy and very bony fish.",
                            Type = "Fish",
                            Category = -4,
                            Price = 800,
                            Texture = texturePath,
                            SpriteIndex = 0,
                            Edibility = 15,
                            IsDrink = false,
                            Buffs = null,
                            GeodeDropsDefaultItems = false,
                            GeodeDrops = null,
                            ArtifactSpotChances = null,
                            ExcludeFromFishingCollection = false,
                            ExcludeFromShippingCollection = false,
                            ExcludeFromRandomSale = false,
                            ContextTags = [
                              "color_blue",
                              "fish_ocean",
                              "season_fall",
                              "season_summer"
                            ],
                            CustomFields = null
                        };
                        editor.Data.Add("936", oceanSunfish);

                        texturePath = modContentHelper.GetInternalAssetName("assets/Sea Slug.png").BaseName;
                        StardewValley.GameData.Objects.ObjectData seaSlug = new()
                        {
                            Name = "Sea Slug",
                            DisplayName = "Sea Slug",
                            Description = "Actually a mollusk!",
                            Type = "Fish",
                            Category = -4,
                            Price = 50,
                            Texture = texturePath,
                            SpriteIndex = 0,
                            Edibility = 7,
                            IsDrink = false,
                            Buffs = null,
                            GeodeDropsDefaultItems = false,
                            GeodeDrops = null,
                            ArtifactSpotChances = null,
                            ExcludeFromFishingCollection = false,
                            ExcludeFromShippingCollection = false,
                            ExcludeFromRandomSale = false,
                            ContextTags = [
                              "color_blue",
                              "fish_ocean",
                              "season_spring",
                              "season_summer",
                              "season_fall",
                              "season_winter"
                            ],
                            CustomFields = null
                        };

                        editor.Data.Add("937", seaSlug);
                    });

                    objectDataAdded = true;
                }
            }

            if (locationDataAdded == false)
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data\\Locations"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsDictionary<string, LocationData>();

                        if (editor.Data.TryGetValue("Beach", out var beachData) == true)
                        {
                            if (beachData.Fish == null)
                            {
                                beachData.Fish = new List<StardewValley.GameData.Locations.SpawnFishData>();
                            }

                            /* Ocean Sunfish. */
                            var fish = new SpawnFishData
                            {
                                Chance = 1,
                                Season = null,
                                FishAreaId = "Ocean",
                                BobberPosition = null,
                                PlayerPosition = null,
                                MinFishingLevel = 3,
                                MinDistanceFromShore = 3,
                                MaxDistanceFromShore = -1,
                                ApplyDailyLuck = false,
                                CuriosityLureBuff = -1,
                                CatchLimit = -1,
                                IsBossFish = false,
                                SetFlagOnCatch = null,
                                RequireMagicBait = false,
                                Precedence = -25,
                                IgnoreFishDataRequirements = false,
                                CanBeInherited = false,
                                ChanceModifiers = null,
                                ChanceModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                ChanceBoostPerLuckLevel = 0,
                                UseFishCaughtSeededRandom = false,
                                Condition = "LOCATION_SEASON Here summer fall",
                                Id = "(O)936",
                                ItemId = "(O)936",
                                RandomItemId = null,
                                MinStack = -1,
                                MaxStack = -1,
                                Quality = -1,
                                ToolUpgradeLevel = -1,
                                IsRecipe = false,
                                StackModifiers = null,
                                StackModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                QualityModifiers = null,
                                QualityModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                ObjectInternalName = null,
                                ObjectDisplayName = null,
                                MaxItems = null,
                                ModData = null,
                                PerItemCondition = null
                            };

                            beachData.Fish.Add(fish);

                            /* Sea Slug. */
                            fish = new SpawnFishData
                            {
                                Chance = 1,
                                Season = null,
                                FishAreaId = "Ocean",
                                BobberPosition = null,
                                PlayerPosition = null,
                                MinFishingLevel = 0,
                                MinDistanceFromShore = 0,
                                MaxDistanceFromShore = -1,
                                ApplyDailyLuck = false,
                                CuriosityLureBuff = -1,
                                CatchLimit = -1,
                                IsBossFish = false,
                                SetFlagOnCatch = null,
                                RequireMagicBait = false,
                                Precedence = 0,
                                IgnoreFishDataRequirements = false,
                                CanBeInherited = false,
                                ChanceModifiers = null,
                                ChanceModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                ChanceBoostPerLuckLevel = 0,
                                UseFishCaughtSeededRandom = false,
                                Condition = "LOCATION_SEASON Here spring summer fall winter",
                                Id = "(O)937",
                                ItemId = "(O)937",
                                RandomItemId = null,
                                MinStack = -1,
                                MaxStack = -1,
                                Quality = -1,
                                ToolUpgradeLevel = -1,
                                IsRecipe = false,
                                StackModifiers = null,
                                StackModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                QualityModifiers = null,
                                QualityModifierMode = StardewValley.GameData.QuantityModifier.QuantityModifierMode.Stack,
                                ObjectInternalName = null,
                                ObjectDisplayName = null,
                                MaxItems = null,
                                ModData = null,
                                PerItemCondition = null
                            };

                            beachData.Fish.Add(fish);
                        }

                    }
                    );

                    locationDataAdded = true;
                }
            }
        }

        /* Determines if the fish accessed is one of the new fish added. Sets the object in question to the fish. */
        public static StardewValley.Object SpecialFish(StardewValley.Object result)
        {

            if (result.Name.Equals("Ocean Sunfish"))
            {
                fishID = 936;
                fishObject = new StardewValley.Object("936", 1, false, -1, 0);
                return fishObject;
            }
            else if (result.Name.Equals("Sea Slug"))
            {
                fishID = 937;
                fishObject = new StardewValley.Object("937", 1, false, -1, 0);
                return fishObject;
            }

            return result;
        }

        /* Draws the fish when it is held, not immediately after catch. */
        public static bool DrawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            Rectangle rectangle = new Rectangle(0, 0, 16, 16);

            if (f.ActiveObject.ParentSheetIndex == 936)
            {
                spriteBatch.Draw(modContentHelper.Load<Texture2D>("assets/Ocean Sunfish"), objectPosition, rectangle, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingPosition().Y + 3) / 10000f));
            }
            else if (f.ActiveObject.ParentSheetIndex == 937)
            {
                spriteBatch.Draw(modContentHelper.Load<Texture2D>("assets/Sea Slug"), objectPosition, rectangle, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (float)(f.getStandingPosition().Y + 3) / 10000f));
            }
            else
            {
                return true;
            }

            return false;
        }

        /* Draws the fish in the dock AND inventory menus... I think... */
        public static bool DrawInInventoryMenu(StardewValley.Object __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {

            if (__instance.ParentSheetIndex == 936)
            {
                spriteBatch.Draw(modContentHelper.Load<Texture2D>("assets/Ocean Sunfish"), location + new Vector2((int)(32f * scaleSize), (int)(32f * scaleSize)), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
            }
            else if (__instance.ParentSheetIndex == 937)
            {
                spriteBatch.Draw(modContentHelper.Load<Texture2D>("assets/Sea Slug"), location + new Vector2((int)(32f * scaleSize), (int)(32f * scaleSize)), new Rectangle(0, 0, 16, 16), color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
            }
            else
            {
                return true;
            }

            /* "Draws" the number of items that are in a stack. */
            bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && 999 > 1 && __instance.Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && __instance.Stack != int.MaxValue;
            if (shouldDrawStackNumber)
            {
                Utility.drawTinyDigits(__instance.Stack, spriteBatch, location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(__instance.Stack, 3f * scaleSize)) + 3f * scaleSize, 64f - 18f * scaleSize + 1f), 3f * scaleSize, 1f, color);
            }

            /* Draws the quality star. */
            if (drawStackNumber != 0 && (int)__instance.Quality > 0)
            {
                Rectangle quality_rect = ((int)__instance.Quality < 4) ? new Rectangle(338 + ((int)__instance.Quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8);
                Texture2D quality_sheet = Game1.mouseCursors;
                float yOffset = ((int)__instance.Quality < 4) ? 0f : (((float)Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1f) * 0.05f);
                spriteBatch.Draw(quality_sheet, location + new Vector2(12f, 52f + yOffset), quality_rect, color * transparency, 0f, new Vector2(4f, 4f), 3f * scaleSize * (1f + yOffset), SpriteEffects.None, layerDepth);
            }

            return false;
        }

        /* Selects the sprite to use for the eating animation. */
        public static bool Eat(Farmer who)
        {
            if (who.itemToEat != null)
            {
                switch (who.itemToEat.ParentSheetIndex)
                {
                    case 936:
                        textureName = "assets/Ocean Sunfish";
                        break;
                    case 937:
                        textureName = "assets/Sea Slug";
                        break;
                    default:
                        return true;
                }
            }

            OriginalEat(who);
            return false;
        }

        /* Part of the function used in the base game to control the eating animation. */
        public static void OriginalEat(Farmer who)
        {
            TemporaryAnimatedSprite tempSprite2 = null;

            switch (who.FarmerSprite.currentAnimationIndex)
            {
                case 1:

                    if (!who.IsLocalPlayer || who.itemToEat == null || !(who.itemToEat is StardewValley.Object))
                    {
                        tempSprite2 = new TemporaryAnimatedSprite(textureName, new Rectangle(0, 0, 16, 16), 254f, 1, 0, who.Position + new Vector2(-21f, -112f), flicker: false, flipped: false, (float)who.getStandingPosition().Y / 10000f + 0.01f, 0f, Microsoft.Xna.Framework.Color.White, 4f, 0f, 0f, 0f);
                    }
                    break;
                case 2:
                    if (Game1.currentLocation == who.currentLocation)
                    {
                        Game1.playSound("dwop");
                    }
                    tempSprite2 = new TemporaryAnimatedSprite(textureName, new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 650f, 1, 0, who.Position + new Vector2(-21f, -108f), flicker: false, flipped: false, (float)who.getStandingPosition().Y / 10000f + 0.01f, 0f, Microsoft.Xna.Framework.Color.White, 4f, -0.01f, 0f, 0f)
                    {
                        motion = new Vector2(0.8f, -11f),
                        acceleration = new Vector2(0f, 0.5f)
                    };
                    break;
                case 3:
                    who.yJumpVelocity = 6f;
                    who.yJumpOffset = 1;
                    break;
                case 4:
                    {
                        if (Game1.currentLocation == who.currentLocation && who.ShouldHandleAnimationSound())
                        {
                            Game1.playSound("eat");
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            Rectangle r = new Rectangle(0, 0, 16, 16);
                            r.X += 8;
                            r.Y += 8;
                            r.Width = 4;
                            r.Height = 4;
                            tempSprite2 = new TemporaryAnimatedSprite(textureName, r, 400f, 1, 0, who.Position + new Vector2(24f, -48f), flicker: false, flipped: false, (float)who.getStandingPosition().Y / 10000f + 0.01f, 0f, Microsoft.Xna.Framework.Color.White, 4f, 0f, 0f, 0f)
                            {
                                motion = new Vector2((float)Game1.random.Next(-30, 31) / 10f, Game1.random.Next(-6, -3)),
                                acceleration = new Vector2(0f, 0.5f)
                            };
                            who.currentLocation.temporarySprites.Add(tempSprite2);
                        }
                        break;
                    }
                default:
                    who.freezePause = 0;
                    break;
            }
            if (tempSprite2 != null)
            {
                who.currentLocation.temporarySprites.Add(tempSprite2);
            }
        }

        /* Loading the texture so that it may be used by the TemporaryAnimatedSprite class. */
        public static bool LoadTexture(TemporaryAnimatedSprite __instance)
        {

            if (textureName == "assets/Ocean Sunfish")
            {
                __instance.texture = modContentHelper.Load<Texture2D>(textureName);
                return false;
            }
            else if (textureName == "assets/Sea Slug")
            {
                __instance.texture = modContentHelper.Load<Texture2D>(textureName);
                return false;
            }
            else
            {
                return true;
            }
        }

        /* Pulling the fish from the water and drawing the fish's texture. */
        public static bool DoPullFromWater(BinaryReader argReader, FishingRod __instance)
        {
            if ((fishID != 937) && (fishID != 936))
            {
                return true;
            }

            Farmer who = Game1.player;
            string fishId = argReader.ReadString();

            int fishSize = argReader.ReadInt32();
            int fishQuality = argReader.ReadInt32();
            int fishDifficulty = argReader.ReadInt32();
            bool treasureCaught = argReader.ReadBoolean();
            bool wasPerfect = argReader.ReadBoolean();
            bool fromFishPond = argReader.ReadBoolean();
            string setFlagOnCatch = argReader.ReadString();
            bool isBossFish = argReader.ReadBoolean();
            int numCaught = argReader.ReadInt32();
            __instance.treasureCaught = treasureCaught;
            __instance.fishSize = fishSize;
            __instance.fishQuality = fishQuality;
            __instance.whichFish = ItemRegistry.GetMetadata(fishId);
            __instance.fromFishPond = fromFishPond;
            __instance.setFlagOnCatch = ((setFlagOnCatch != string.Empty) ? setFlagOnCatch : null);
            __instance.numberOfFishCaught = numCaught;

            Vector2 bobberTile = new Vector2(__instance.bobber.X / 64f, __instance.bobber.Y / 64f);

            bool fishIsObject = __instance.whichFish.TypeIdentifier == "(O)";

            if (fishQuality >= 2 && wasPerfect)
            {
                __instance.fishQuality = 4;
            }
            else if (fishQuality >= 1 && wasPerfect)
            {
                __instance.fishQuality = 2;
            }

            if (!Game1.isFestival() && who.IsLocalPlayer && !fromFishPond && fishIsObject)
            {
                int experience = Math.Max(1, (fishQuality + 1) * 3 + fishDifficulty / 3);
                if (treasureCaught)
                {
                    experience += (int)((float)experience * 1.2f);
                }
                if (wasPerfect)
                {
                    experience += (int)((float)experience * 1.4f);
                }
                if (isBossFish)
                {
                    experience *= 5;
                }
                who.gainExperience(1, experience);
            }

            if (__instance.fishQuality < 0)
            {
                __instance.fishQuality = 0;
            }

            string sprite_sheet_name;
            Rectangle sprite_rect;
            if (fishIsObject)
            {
                if (fishId == "936")
                {
                    ParsedItemData parsedOrErrorData = __instance.whichFish.GetParsedOrErrorData();
                    //sprite_sheet_name = "";
                    sprite_sheet_name = modContentHelper.GetInternalAssetName("assets/Ocean Sunfish.png").BaseName;
                    sprite_rect = new Rectangle(0, 0, 16, 16);
                }
                else if (fishId == "937")
                {
                    ParsedItemData parsedOrErrorData = __instance.whichFish.GetParsedOrErrorData();
                    //sprite_sheet_name = "";
                    sprite_sheet_name = modContentHelper.GetInternalAssetName("assets/Sea Slug.png").BaseName;
                    sprite_rect = new Rectangle(0, 0, 16, 16);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            float t;
            if (who.FacingDirection == 1 || who.FacingDirection == 3)
            {
                float distance = Vector2.Distance(__instance.bobber.Value, who.Position);
                float gravity = 0.001f;
                float height = 128f - (who.Position.Y - __instance.bobber.Y + 10f);
                double angle = 1.1423973285781066;
                float yVelocity = (float)((double)(distance * gravity) * Math.Tan(angle) / Math.Sqrt((double)(2f * distance * gravity) * Math.Tan(angle) - (double)(2f * gravity * height)));
                if (float.IsNaN(yVelocity))
                {
                    yVelocity = 0.6f;
                }
                float xVelocity = (float)((double)yVelocity * (1.0 / Math.Tan(angle)));
                t = distance / xVelocity;
                __instance.animations.Add(new TemporaryAnimatedSprite(sprite_sheet_name, sprite_rect, t, 1, 0, __instance.bobber.Value, flicker: false, flipped: false, __instance.bobber.Y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f)
                {
                    motion = new Vector2((float)((who.FacingDirection != 3) ? 1 : (-1)) * (0f - xVelocity), 0f - yVelocity),
                    acceleration = new Vector2(0f, gravity),
                    timeBasedMotion = true,
                    endFunction = delegate
                    {
                        __instance.playerCaughtFishEndFunction(isBossFish);
                    },
                    endSound = "tinyWhip"
                });
                if (__instance.numberOfFishCaught > 1)
                {
                    for (int i = 1; i < __instance.numberOfFishCaught; i++)
                    {
                        distance = Vector2.Distance(__instance.bobber.Value, who.Position);
                        gravity = 0.0008f - (float)i * 0.0001f;
                        height = 128f - (who.Position.Y - __instance.bobber.Y + 10f);
                        angle = 1.1423973285781066;
                        yVelocity = (float)((double)(distance * gravity) * Math.Tan(angle) / Math.Sqrt((double)(2f * distance * gravity) * Math.Tan(angle) - (double)(2f * gravity * height)));
                        if (float.IsNaN(yVelocity))
                        {
                            yVelocity = 0.6f;
                        }
                        xVelocity = (float)((double)yVelocity * (1.0 / Math.Tan(angle)));
                        t = distance / xVelocity;
                        __instance.animations.Add(new TemporaryAnimatedSprite(sprite_sheet_name, sprite_rect, t, 1, 0, __instance.bobber.Value, flicker: false, flipped: false, __instance.bobber.Y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            motion = new Vector2((float)((who.FacingDirection != 3) ? 1 : (-1)) * (0f - xVelocity), 0f - yVelocity),
                            acceleration = new Vector2(0f, gravity),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = who.currentLocation,
                            delayBeforeAnimationStart = (i - 1) * 100
                        });
                    }
                }
            }
            else
            {
                int playerStandingY = who.StandingPixel.Y;
                float distance = __instance.bobber.Y - (float)(playerStandingY - 64);
                float height = Math.Abs(distance + 256f + 32f);
                if (who.FacingDirection == 0)
                {
                    height += 96f;
                }
                float gravity = 0.003f;
                float velocity = (float)Math.Sqrt(2f * gravity * height);
                t = (float)(Math.Sqrt(2f * (height - distance) / gravity) + (double)(velocity / gravity));
                float xVelocity = 0f;
                if (t != 0f)
                {
                    xVelocity = (who.Position.X - __instance.bobber.X) / t;
                }
                __instance.animations.Add(new TemporaryAnimatedSprite(sprite_sheet_name, sprite_rect, t, 1, 0, __instance.bobber.Value, flicker: false, flipped: false, __instance.bobber.Y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f)
                {
                    motion = new Vector2(xVelocity, 0f - velocity),
                    acceleration = new Vector2(0f, gravity),
                    timeBasedMotion = true,
                    endFunction = delegate
                    {
                        __instance.playerCaughtFishEndFunction(isBossFish);
                    },
                    endSound = "tinyWhip"
                });
                if (__instance.numberOfFishCaught > 1)
                {
                    for (int i = 1; i < __instance.numberOfFishCaught; i++)
                    {
                        distance = __instance.bobber.Y - (float)(playerStandingY - 64);
                        height = Math.Abs(distance + 256f + 32f);
                        if (who.FacingDirection == 0)
                        {
                            height += 96f;
                        }
                        gravity = 0.004f - (float)i * 0.0005f;
                        velocity = (float)Math.Sqrt(2f * gravity * height);
                        t = (float)(Math.Sqrt(2f * (height - distance) / gravity) + (double)(velocity / gravity));
                        xVelocity = 0f;
                        if (t != 0f)
                        {
                            xVelocity = (who.Position.X - __instance.bobber.X) / t;
                        }
                        __instance.animations.Add(new TemporaryAnimatedSprite(sprite_sheet_name, sprite_rect, t, 1, 0, new Vector2(__instance.bobber.X, __instance.bobber.Y), flicker: false, flipped: false, __instance.bobber.Y / 10000f, 0f, Color.White, 4f, 0f, 0f, 0f)
                        {
                            motion = new Vector2(xVelocity, 0f - velocity),
                            acceleration = new Vector2(0f, gravity),
                            timeBasedMotion = true,
                            endSound = "fishSlap",
                            Parent = who.currentLocation,
                            delayBeforeAnimationStart = (i - 1) * 100
                        });
                    }
                }
            }
            if (who.IsLocalPlayer)
            {
                who.currentLocation.playSound("pullItemFromWater", bobberTile);
                who.currentLocation.playSound("dwop", bobberTile);
            }
            __instance.castedButBobberStillInAir = false;
            __instance.pullingOutOfWater = true;
            __instance.isFishing = false;
            __instance.isReeling = false;
            who.FarmerSprite.PauseForSingleAnimation = false;
            switch (who.FacingDirection)
            {
                case 0:
                    who.FarmerSprite.animateBackwardsOnce(299, t);
                    break;
                case 1:
                    who.FarmerSprite.animateBackwardsOnce(300, t);
                    break;
                case 2:
                    who.FarmerSprite.animateBackwardsOnce(301, t);
                    break;
                case 3:
                    who.FarmerSprite.animateBackwardsOnce(302, t);
                    break;
            }

            return false;
        }

        public static void FishFrame(SpriteBatch b, FishingRod __instance)
        {
            if ((__instance.whichFish != null) && ((fishID == 936) || (fishID == 937)))
            {
                Farmer who = Game1.player;
                bool fishIsObject = __instance.whichFish.TypeIdentifier == "(O)";
                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                int playerStandingY = who.StandingPixel.Y;
                ParsedItemData parsedOrErrorData = __instance.whichFish.GetParsedOrErrorData();

                if (__instance.whichFish == ItemRegistry.GetMetadata("936"))
                {
                    Texture2D texture = modContentHelper.Load<Texture2D>("assets/Ocean Sunfish");
                    Rectangle sourceRect = new Rectangle(0, 0, 16, 16);

                    if (__instance.fishCaught == true)
                    {
                        /* This draws it in the little frame. */
                        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-124f, -284f + yOffset) + new Vector2(44f, 68f)), sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)playerStandingY / 10000f + 0.0001f + 0.06f);

                        /* This draws it in the player's hand. */
                        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(0f, -56f)), sourceRect, Color.White, (__instance.fishSize == -1 || __instance.whichFish.QualifiedItemId == "(O)800" || __instance.whichFish.QualifiedItemId == "(O)798" || __instance.whichFish.QualifiedItemId == "(O)149" || __instance.whichFish.QualifiedItemId == "(O)151") ? 0f : ((float)Math.PI * 3f / 4f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)playerStandingY / 10000f + 0.002f + 0.06f);
                    }

                }
                else if (__instance.whichFish == ItemRegistry.GetMetadata("937"))
                {
                    Texture2D texture = modContentHelper.Load<Texture2D>("assets/Sea Slug");
                    Rectangle sourceRect = new Rectangle(0, 0, 16, 16);

                    if (__instance.fishCaught == true)
                    {
                        /* This draws it in the little frame. */
                        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(-124f, -284f + yOffset) + new Vector2(44f, 68f)), sourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)playerStandingY / 10000f + 0.0001f + 0.06f);

                        /* This draws it in the player's hand. */
                        b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, who.Position + new Vector2(0f, -56f)), sourceRect, Color.White, (__instance.fishSize == -1 || __instance.whichFish.QualifiedItemId == "(O)800" || __instance.whichFish.QualifiedItemId == "(O)798" || __instance.whichFish.QualifiedItemId == "(O)149" || __instance.whichFish.QualifiedItemId == "(O)151") ? 0f : ((float)Math.PI * 3f / 4f), new Vector2(8f, 8f), 3f, SpriteEffects.None, (float)playerStandingY / 10000f + 0.002f + 0.06f);
                    }
                }
            }
        }
    }
}
