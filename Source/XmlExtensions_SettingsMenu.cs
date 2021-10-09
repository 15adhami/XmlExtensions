using UnityEngine;
using Verse;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    public class XmlExtensions_SettingsMenu : Window
    {
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(900f + 256f + 6f, 700f);
            }
        }

        public XmlExtensions_SettingsMenu()
        {
            doCloseButton = true;
            forcePause = true;
            absorbInputAroundWindow = true;
            doCloseX = true;
            closeOnAccept = false;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            Text.Font = GameFont.Medium;
            listing.verticalSpacing = 0;
            listing.Label("More Mod Settings");
            Text.Font = GameFont.Small;
            listing.GapLine(9f);
            listing.End();
            Rect drawRect = inRect.BottomPartPixels(inRect.height - 38);
            Rect bottomRect = drawRect.BottomPartPixels(40);
            Rect settingRect = drawRect.TopPartPixels(drawRect.height - 40);
            XmlMod.DrawSettingsWindow(settingRect);
        }

        public override void PreClose()
        {
            XmlMod.SetSelectedMod(null);
            XmlMod.PreClose();
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }
    }
}
