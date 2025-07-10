using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchStop : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.Profiler.StopWatch();
            Verse.Log.Message("XmlExtensions.Stopwatch: " + PatchManager.Profiler.ElapsedMilliseconds().ToString() + "ms");
            PatchManager.Profiler.ResetWatch();
            return true;
        }
    }
}