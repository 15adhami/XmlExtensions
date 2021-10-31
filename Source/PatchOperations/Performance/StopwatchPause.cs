using System.Xml;

namespace XmlExtensions
{
    internal class StopwatchPause : PatchOperationExtended
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