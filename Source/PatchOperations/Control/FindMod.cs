using System.Xml;
using System.Collections.Generic;
using Verse;
using System.Linq;
using System;

namespace XmlExtensions
{
    public class FindMod : PatchOperationExtended
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                bool flag = false;
                int errNum = 0;
                if (mods == null)
                {
                    PatchManager.errors.Add("XmlExtensions.FindMod: <mods> is null");
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

                if (flag)
                {
                    if (caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.FindMod: Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                else
                {
                    if (caseFalse != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.FindMod: Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.FindMod: " + e.Message);
                return false;
            }
        }
    }
}

