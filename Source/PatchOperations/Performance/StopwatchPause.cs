using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchPause : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            if (PatchManager.Profiler.IsRunning())
            {
                PatchManager.Profiler.StopWatch();
            }
            return true;
        }
    }
}