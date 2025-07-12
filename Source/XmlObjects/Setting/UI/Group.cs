using System.Collections.Generic;
using UnityEngine;

namespace XmlExtensions.Setting
{
    internal class Group : SettingContainer
    {
        public List<SettingContainer> settings;

        protected override bool Init()
        {
            addDefaultSpacing = false;
            return InitializeContainers(modId, settings);
        }

        protected override float CalculateHeight(float width)
        {
            return CalculateHeightSettingsList(width, settings);
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            DrawSettingsList(inRect, settings);
        }

        internal override bool PreOpen()
        {
            return PreOpenContainers(settings);
        }

        internal override bool PostClose()
        {
            return PostCloseContainers(settings);
        }
    }
}