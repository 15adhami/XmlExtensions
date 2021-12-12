using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class ErrorManager
    {
        public static int depth = 0;
        public static bool bootedWithAdvancedDebugging = false;

        private static List<string> errors;

        static ErrorManager()
        {
            errors = new List<string>();
        }

        /// <summary>
        /// Prints the stack of errors, then clears them.
        /// </summary>
        /// <param name="source">The path to the source file.</param>
        /// <param name="mod">The ModContentPack of the active mod.</param>
        public static void PrintErrors(string source, ModContentPack mod)
        {
            string trace = "";
            trace += "[" + mod.Name + " - Start of stack trace]\n";
            foreach (string error in errors)
            {
                trace += error + "\n";
            }
            trace += "[End of stack trace]\nThe top operation is the one that failed, the ones below it are the parents\nSource file: " + source + "\n";
            if (XmlMod.allSettings.trace)
                Verse.Log.Error(trace);
            ClearErrors();
        }

        /// <summary>
        /// Prints the stack of errors, then clears them.
        /// </summary>
        public static void PrintErrors()
        {

            string trace = "";
            trace += "[Start of stack trace]\n";
            foreach (string error in errors)
            {
                trace += error + "\n";
            }
            trace += "[End of stack trace]\nThe top operation is the one that failed, the ones below it are the parents\n";
            if (XmlMod.allSettings.trace)
                Verse.Log.Error(trace);
            ClearErrors();
        }

        public static void ClearErrors()
        {
            errors.Clear();
        }

        public static void AddError(string msg)
        {
            errors.Add(msg);
        }

        public static int ErrorCount()
        {
            return errors.Count;
        }

        public static void PrintModsThatPatched(HashSet<ModContentPack> mods, string msg)
        {
            if (mods == null)
            {
                return;
            }
            string str = msg + "\n";
            foreach (ModContentPack mod in mods)
            {
                str += mod.Name + " (" + mod.PackageId + ")\n";
            }
            Verse.Log.Warning(str);
        }

        public static void PrintSusMods(XmlNode xmlNode)
        {
            HashSet<ModContentPackContainer> modsTemp;
            HashSet<ModContentPack> mods = new();
            XmlNode tempNode = xmlNode;
            do
            {
                if (PatchManager.DefModDict.TryGetValue(Helpers.GetDefNameFromNode(tempNode)??"", out modsTemp))
                {
                    foreach (ModContentPackContainer pack in modsTemp)
                    {
                        if (!mods.Contains(pack.Pack))
                        {
                            mods.Add(pack.Pack);
                        }
                    }
                }
                if (tempNode.Attributes["ParentName"] != null)
                {
                    tempNode = xmlNode.OwnerDocument.SelectSingleNode("Defs/" + xmlNode.Name + "[@Name=\"" + tempNode.Attributes["ParentName"].ToString() + "\"]");
                }
                else
                {
                    break;
                }
            }
            while (tempNode != null);
            if (mods.Count > 0)
            {
                PrintModsThatPatched(mods, "Possibly relevant mods for above error:");
            }
        }
    }
}