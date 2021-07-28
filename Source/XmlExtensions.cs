using Verse;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace XmlExtensions
{ 
    public class XmlExtensions : Mod
    {


        public XmlExtensions(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("Imranfish.XmlExtensions.patch");
            harmony.PatchAll();
        }

        public override string SettingsCategory()
        {
            return "XML Extensions";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
           // settings.DoSettingsWindowContents(inRect);
        }
    }


}