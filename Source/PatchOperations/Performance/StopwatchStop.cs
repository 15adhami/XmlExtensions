using System.Xml;

namespace XmlExtensions
{
    public class StopwatchStop : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            PatchManager.watch.Stop();
            Verse.Log.Message("XmlExtensions.Stopwatch: " + PatchManager.watch.ElapsedMilliseconds.ToString() + "ms");
            return true;
        }
    }
}