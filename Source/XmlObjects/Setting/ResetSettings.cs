using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Setting
{
    public class ResetSettings : SettingContainer
    {
        protected string label = "Reset settings";
        protected List<string> keys = null;
        protected bool confirm = true;
        public string message;
        public string tKeyMessage;
        public string tKey;
        public string tKeyTip;
        public string tooltip;

        public override void drawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            if (!confirm)
            {
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), Helpers.tryTranslate(tooltip, tKeyTip)))
                {
                    foreach (string key in keys)
                        XmlMod.allSettings.dataDict[selectedMod + ";" + key] = XmlMod.settingsPerMod[selectedMod].defValues[key];
                }
                    
            }
            else
            {
                if (tKeyMessage == null && message == null)
                    tKeyMessage = "XmlExtensions_ConfirmationResetMod";
                if(message == null)
                    message = "Are you sure you want to reset every setting of the current mod?";
                if (keys == null) { keys = XmlMod.settingsPerMod[selectedMod].keys; }
                if (listingStandard.ButtonText(Helpers.tryTranslate(label, tKey), null))
                {
                    Find.WindowStack.Add(new Dialog_MessageBox(Helpers.tryTranslate(message, tKeyMessage), "Yes".Translate(), delegate ()
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

        public override int getHeight(float width, string selectedMod) { return (30 + XmlMod.menus[XmlMod.activeMenu].defaultSpacing); }
    }
}
