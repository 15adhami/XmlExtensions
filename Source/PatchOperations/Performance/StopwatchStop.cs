using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class StopwatchStop : PatchOperation
    {

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                PatchManager.watch.Stop();
                Verse.Log.Message("XmlExtensions.Stopwatch: " + PatchManager.watch.ElapsedMilliseconds.ToString() + "ms");
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
