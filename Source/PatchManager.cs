using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public static class PatchManager
    {
        public static Queue<string> delayQueue;

        static PatchManager()
        {
            delayQueue = new Queue<string>();
        }

        public static void enqueuePatch(XmlNode node)
        {
            delayQueue.Enqueue(node.OuterXml);
        }

        public static void applyPatches(XmlDocument xmlDoc)
        {
            for (int i = 0; i < delayQueue.Count; i++)
            {
                try
                {
                    PatchOperation patch = Helpers.getPatchFromString(delayQueue.Dequeue());
                    patch.Apply(xmlDoc);
                }
                catch (Exception e)
                {
                    Log.Error("Error in delayed patch.Apply(): " + e, false);
                }
            }
        }
    }
}
