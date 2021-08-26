﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    static class PatchManager
    {
        public static XmlDocument xmlDoc;
        public static Queue<PatchOperation> delayedPatches;
        public static int depth = 0;
        public static List<string> errors;

        static PatchManager()
        {
            delayedPatches = new Queue<PatchOperation>();
            xmlDoc = new XmlDocument();
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
            trace += "End of stack trace. The top operation is the one that failed, the ones below it are the parents.\nSource file: " + source + "\n";
            Verse.Log.Error(trace);
            errors.Clear();
        }

        public static void runPatches()
        {
            for(int i = 0; i < delayedPatches.Count; i++)
            {
                PatchOperation patch = delayedPatches.Dequeue();
                patch.Apply(xmlDoc);
            }
        }
    }
}
