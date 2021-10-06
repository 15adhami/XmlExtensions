using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class StopwatchPause : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            if (PatchManager.watch.IsRunning)
            {
                PatchManager.watch.Stop();
            }
            return true;
        }
    }
}
