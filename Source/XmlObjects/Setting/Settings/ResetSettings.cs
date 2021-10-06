using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Setting
{
    public class ResetSettings : SettingContainer
    {
        protected string label;
        protected List<string> keys = null;
        protected bool confirm = true;
        public string message;
        public string tKeyMessage;
        public string tKey;
        public string tKeyTip;
        public string tooltip;

        protected override void DrawSettingContents(Listing_Standard listingStandard, string selectedMod)
        {
            if (!confirm)
            {
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.TryTranslate(label, tKey), Helpers.TryTranslate(tooltip, tKeyTip)))
                {
                    foreach (string key in keys)
                        XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                }
                    
            }
            else
            {
                
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.TryTranslate(label, tKey), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.TryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
                    {
                        foreach(string key in keys)
                        {
                            if (XmlMod.allSettings.dataDict.ContainsKey(selectedMod + ";" + key))
                                XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                        }
                            
                    }, "No".Translate(), null, null, false, null, null));
                }
            }
        }

        protected override bool Init()
        {
            base.Init();
            if (label == null)
            {
                label = "Reset Settings";
                tKey = "XmlExtensions_ResetSettings";
            }
            if (message == null)
            {
                tKeyMessage = "XmlExtensions_Confirmation";
                message = "Are you sure?";
            }
            return true;
        }

        protected override int CalcHeight(float width, string selectedMod) { return (30 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
