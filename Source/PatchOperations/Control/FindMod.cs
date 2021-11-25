using System.Collections.Generic;
using System.Xml;
using Verse;
using System.Linq;

namespace XmlExtensions
{
    internal class FindMod : PatchOperationExtended
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        private string foundMod;

        protected override void SetException()
        {
            if (foundMod != null)
            {
                CreateExceptions(foundMod, "Mod");
            }
        }

        protected override bool Patch(XmlDocument xml)
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
                    foundMod = mod;
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
            return RunPatchesConditional(flag, caseTrue, caseFalse, xml);
        }
    }
}