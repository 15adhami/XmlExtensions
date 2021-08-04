using Verse;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace XmlExtensions
{
    public class XmlExtensions : Mod
    {


        public XmlExtensions(ModContentPack content) : base(content)
        {
        }

        public override string SettingsCategory()
        {
            return "XML Extensions";
        }

        public override void DoSettingsWindowContents(Rect inRect)
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
        }
    }


}