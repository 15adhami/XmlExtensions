using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Action;

namespace XmlExtensions
{
    // Window that appears when you press More Mod Settings
    public class ModSettingsWindow : BaseSettingsWindow
    {
        public override Vector2 InitialSize => new Vector2(900f, 700f);

        public ModSettingsWindow(SettingsMenuDef initialMenu = null, bool isXmlExtensions = false) : base(initialMenu, isXmlExtensions)
        {
            if (initialMenu != null)
            {
                SetSelectedMod(new ModContainer(initialMenu.modId));
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rightRect = inRect.RightPartPixels(864f);
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, rightRect.width - 150f - 17f, 35f), SelectedMod != null ? SelectedMod.ToString() : "XML Extensions");
            Text.Font = GameFont.Small;
            Rect rect = new Rect(0f, 0f, rightRect.width, rightRect.height - Window.CloseButSize.y);
            GUI.BeginGroup(rect);

            if (SelectedMod != null && activeMenu != null)
            {
                DrawModSettings(rect);
            }
            GUI.EndGroup();
            base.DoWindowContents(inRect);
        }

        public override void PreClose()
        {
            SetSelectedMod(null);
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
            base.PreClose();
        }
    }
}