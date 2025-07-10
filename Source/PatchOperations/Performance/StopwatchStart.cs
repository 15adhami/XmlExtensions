using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchStart : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.Profiler.ResetWatch();
            PatchManager.Profiler.StartWatch();
            return true;
        }
    }
}