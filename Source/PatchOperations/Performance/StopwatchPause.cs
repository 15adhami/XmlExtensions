using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class StopwatchPause : PatchOperation
    {

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (PatchManager.watch.IsRunning)
                {
                    PatchManager.watch.Stop();
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.StopwatchStop: " + e.Message);
                return false;
            }
        }
    }
}
