using System;
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

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool flag = false;
                if (mods == null)
                {
                    PatchManager.errors.Add("XmlExtensions.Boolean.FindMod: <mods> is null");
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
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Boolean.FindMod: " + e.Message);
                return false;
            }
        }
    }

}
