using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchResume : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.Profiler.StartWatch();
            return true;
        }
    }
}