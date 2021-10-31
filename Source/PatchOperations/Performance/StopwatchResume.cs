using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchResume : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.watch.Start();
            return true;
        }
    }
}