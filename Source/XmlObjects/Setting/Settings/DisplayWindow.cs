using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class DisplayWindow : SettingContainer
    {
        public string label;
        public string tKey;
        public Vector2 size = new Vector2(500, 500);
        public string menu;
        public List<SettingContainer> settings;

        public bool doCloseX = true;
        public bool doCloseButton = false;
        public bool resizeable = false;
        public bool draggable = false;
        public bool absorbInputAroundWindow = true;
        public bool allowMultipleWindows = false;

        public class SettingsWindow : Window
        {
            public List<SettingContainer> settings;
            public string modId;
            public Vector2 initSize;

            public override Vector2 InitialSize
            {
                get
                {
                    return initSize;
                }
            }

            public SettingsWindow()
            {
                forcePause = true;
                closeOnAccept = true;
                layer = WindowLayer.SubSuper;
            }

            public override void DoWindowContents(Rect inRect)
            {
                Rect rect = inRect;
                if (doCloseX)
                {
                    rect.yMin += 5;
                }
                if (doCloseButton)
                {
                    rect.yMax -= 50;
                }
                WindowContents(rect);
            }

            private void WindowContents(Rect inRect)
            {
                if (settings != null)
                {
                    ((ScrollView)settings[0]).height = inRect.height;
                    Listing_Standard listing = new Listing_Standard();
                    listing.Begin(inRect);
                    foreach (SettingContainer setting in settings)
                    {
                        setting.DrawSetting(listing.GetRect(setting.GetHeight(inRect.width, modId)), modId);
                    }
                    listing.End();
                }
            }
        }

        protected override bool Init(string selectedMod)
        {
            if (label == null)
            {
                label = "Open";
                tKey = "XmlExtensions_Open";
            }
            if (menu != null)
            {
                ScrollView scrollView = new ScrollView
                {
                    settings = DefDatabase<SettingsMenuDef>.GetNamed(menu).settings
                };
                settings = new List<SettingContainer>() { scrollView };
            }
            return InitializeSettingsList(selectedMod, settings);
        }

        protected override float CalculateHeight(float width, string selectedMod)
        {
            return 30 + GetDefaultSpacing();
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            if (Widgets.ButtonText(inRect, Helpers.TryTranslate(label, tKey)))
            {
                SettingsWindow window = new SettingsWindow();
                window.initSize.x = size.x + 36 + 16;
                window.initSize.y = size.y + 36;
                window.settings = settings;
                window.modId = selectedMod;
                window.resizeable = resizeable;
                window.doCloseButton = doCloseButton;
                window.doCloseX = doCloseX;
                window.draggable = draggable;
                window.absorbInputAroundWindow = absorbInputAroundWindow;
                window.onlyOneOfTypeAllowed = !allowMultipleWindows;
                if (doCloseButton)
                {
                    window.initSize.y += 50;
                }
                if (doCloseX)
                {
                    window.initSize.y += 5;
                }
                Find.WindowStack.Add(window);
            }
        }
    }
}