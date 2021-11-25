using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace XmlExtensions.Boolean
{
    internal class FindMod : BooleanBase
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            bool flag = false;
            if (mods == null)
            {
                NullError("mods");
                return false;
            }
            foreach (string mod in mods)
            {
                if (LoadedModManager.RunningMods.Any(m => (packageId ? m.PackageId.ToLower() : m.Name.ToLower()) == mod.ToLower()))
                {
                    flag = true;
                    if (logic.ToLower() == "or")
                    {
                        break;
                    }
                }
                else
                {
                    flag = false;
                    if (logic.ToLower() == "and")
                    {
                        break;
                    }
                }
            }
            b = flag;
            return true;
        }
    }
}