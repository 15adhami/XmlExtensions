using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class StopwatchStart : PatchOperation
    {
        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                PatchManager.watch.Reset();
                PatchManager.watch.Start();
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStart: " + e.Message);
                return false;
            }
        }
    }
}
