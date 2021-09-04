using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
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
            this.doCloseButton = true;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.doCloseX = true;
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
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }
    }
}
