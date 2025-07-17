using System.Collections.Generic;
using UnityEngine;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions.Action
{
    internal class DisplayWindow : ActionContainer
    {
        public Vector2 size = new(500, 500);
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
                        setting.DrawSetting(listing.GetRect(setting.GetHeight(inRect.width)));
                    }
                    listing.End();
                }
            }
        }

        protected override bool Init()
        {
            if (menu != null)
            {
                ScrollView scrollView = new() { settings = DefDatabase<SettingsMenuDef>.GetNamed(menu).settings };
                settings = [ scrollView ];
            }
            else if (settings != null)
            {
                ScrollView scrollView = new() { settings = settings };
                settings = [scrollView];
            }
            return InitializeContainers(menuDef, settings);
        }

        internal override bool PreOpen()
        {
            return PreOpenContainers(settings);
        }

        internal override bool PostClose()
        { 
            return PostCloseContainers(settings);
        }
        protected override bool ApplyAction()
        {
            SettingsWindow window = new();
            window.initSize.x = size.x + 36 + 16;
            window.initSize.y = size.y + 36;
            window.settings = settings;
            window.modId = modId;
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
            return true;
        }
    }
}
