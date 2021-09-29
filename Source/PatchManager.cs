using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    static class PatchManager
    {
        public static XmlDocument xmlDoc;
        public static XmlDocument defaultDoc;
        public static Stopwatch watch;
        public static Stopwatch watch2;
        public static int depth = 0;
        public static List<string> errors;
        public static bool loadingPatches = false;
        public static int rangeCount = 1;
        public static bool context = false;
        public static string contextPath;
        public static Dictionary<Type, Delegate> patchConstructors;
        public static Dictionary<string, XmlDocument> XmlDocs;
        public static Dictionary<string, Dictionary<XmlNode, XmlNode>> nodeMap;

        static PatchManager()
        {
            patchConstructors = new Dictionary<Type, Delegate>();
            nodeMap = new Dictionary<string, Dictionary<XmlNode, XmlNode>>();
            XmlDocs = new Dictionary<string, XmlDocument>();
            watch = new Stopwatch();
            watch2 = new Stopwatch();
            xmlDoc = new XmlDocument();
            defaultDoc = new XmlDocument();
            depth = 0;
            errors = new List<string>();
        }

        public static void printError(string source)
        {
            string trace = "";
            foreach(string error in errors)
            {
                trace += error + "\n";
            }
            trace += "[End of stack trace]\nThe top operation is the one that failed, the ones below it are the parents\nSource file: " + source + "\n";
            if (XmlMod.allSettings.trace)
                Verse.Log.Error("[Start of stack trace]\n" + trace);
            errors.Clear();
        }
    }
}
