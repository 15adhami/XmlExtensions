﻿using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class ErrorManager
    {
        public static int depth = 0;

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
    }
}