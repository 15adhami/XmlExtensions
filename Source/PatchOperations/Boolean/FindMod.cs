using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace XmlExtensions.Boolean
{
    public class FindMod : BooleanBase
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
            if (logic == "or")
            {
                if (!packageId)
                    flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                else
                    flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
            }
            else
            {
                if (!packageId)
                    flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                else
                    flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
            }
            b = flag;
            return true;
        }
    }
}