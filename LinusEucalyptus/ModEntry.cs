using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace LinusEucalyptus
{
    public class ModEntry : Mod
    {

        /* Entry point. */

        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        /* Edits the NPCDispositions.xnb file within the Data folder. */

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions"))
            {
                e.Edit(asset =>
                {
                    var editor = asset.AsDictionary<string, string>();
                    editor.Data["Linus"] = "adult/neutral/shy/positive/male/not-datable/null/Town/winter 3//Tent 2 2/Eucalyptus";
                });
            }
        }
    }
}
