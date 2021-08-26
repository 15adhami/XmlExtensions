using Verse;
using HarmonyLib;
using System.Reflection;

// This code is so bad

namespace XmlExtensions
{
    public class XmlExtensions : Mod
    {


        public XmlExtensions(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.github.15adhami.xmlextensions");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /*public override string SettingsCategory()
        {
            return "XML Extensions";
        }*/

       /* public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            List<KeyValuePair<string, string>> kvpList = XmlMod.allSettings.dataDict.ToList<KeyValuePair<string, string>>();
            int num = kvpList.Count();
            foreach (KeyValuePair<string, string> pair in kvpList)
            {
                bool del = false;
                listingStandard.CheckboxLabeled(pair.Key, ref del, "Delete");
                if (del)
                {
                    XmlMod.allSettings.dataDict.Remove(pair.Key);
                }                
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }*/
    }


}