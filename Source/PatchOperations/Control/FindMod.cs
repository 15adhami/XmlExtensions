using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class FindMod : PatchOperationExtended
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
            foreach (ModContentPack mod in LoadedModManager.RunningMods)
            {
                string str;
                if (packageId)
                {
                    str = mod.PackageId;
                }
                else
                {
                    str = mod.Name;
                }
                if (mods.Contains(str))
                {
                    foundMod = str;
                    flag = true;
                    if (logic == "or")
                    {
                        break;
                    }
                }
                else
                {
                    flag = false;
                    if (logic == "and")
                    {
                        break;
                    }
                }
            }
            return RunPatchesConditional(flag, caseTrue, caseFalse, xml);
        }
    }
}