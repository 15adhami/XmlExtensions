using System.Collections.Generic;
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
            errors = [];
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
            trace += "[End of stack trace]\nSource file: " + source + "\n";
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
            trace += "[End of stack trace]\n";
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

        public static void AddError(ErrorContext context)
        {
            errors.Add(context.ToString());
        }

        public static int ErrorCount()
        {
            return errors.Count;
        }
    }
}