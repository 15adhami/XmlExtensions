using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class PatchDef : Def
    {
        public List<string> parameters;
        public XmlContainer apply;
        public string brackets = "{}";
    }

    public class UIDef : Def
    {
        public List<UIContainer> UIElements;

        public void DrawUI(Rect rect, List<object> inputs)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(rect);
            foreach(UIContainer UI in UIElements)
            {
                UI.DrawSettingContents(listing.GetRect(UI.CalculateHeight(rect.width)), inputs);
            }
            listing.End();
        }
    }
}
