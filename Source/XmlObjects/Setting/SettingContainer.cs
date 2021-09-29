using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    public abstract class SettingContainer 
    {
        public int errHeight = -1;
        public string tag;

        public virtual void DrawSetting(Listing_Standard listingStandard, string selectedMod)
        {
            try
            {
                this.drawSetting(listingStandard, selectedMod);
            }
            catch
            {
                GUI.color = Color.red;
                listingStandard.Label("Error drawing setting");
                errHeight = 22;
                GUI.color = Color.white;
            }
            
        }

        public virtual void drawSetting(Listing_Standard listingStandard, string selectedMod) { }

        public virtual bool SetDefaultValue(string modId) { return true; }

        public int GetHeight(float width, string selectedMod) { return (errHeight < 0 ? this.getHeight(width, selectedMod) : errHeight); }

        public virtual int getHeight(float width, string selectedMod) { return 0; }

        public virtual void Init() { }
    }
}
