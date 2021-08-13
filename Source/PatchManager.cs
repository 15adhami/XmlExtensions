using System;
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

        static PatchManager()
        {
            delayedPatches = new Queue<PatchOperation>();
            xmlDoc = new XmlDocument();
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
