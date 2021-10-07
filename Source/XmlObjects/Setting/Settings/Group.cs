using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public class Group : SettingContainer
    {
        public List<SettingContainer> settings;

        protected override bool Init()
        {
            return InitializeSettingsList(settings);
        }

        protected override bool SetDefaultValue(string modId)
        {
            return DefaultValueSettingsList(modId, settings);
        }

        protected override float CalcHeight(float width, string selectedMod)
        {
            return GetHeightSettingsList(width, selectedMod, settings);
        }

        protected override void DrawSettingContents(Rect inRect, string selectedMod)
        {
            DrawSettingsList(inRect, selectedMod, settings);
        }

        protected override bool PreClose(string selectedMod)
        {
            return DoPreCloseSettingsList(selectedMod, settings);
        }
    }
}
