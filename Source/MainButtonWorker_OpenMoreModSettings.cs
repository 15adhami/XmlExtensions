using RimWorld;
using Verse;

namespace XmlExtensions
{
    internal class MainButtonWorker_OpenMoreModSettings : MainButtonWorker
    {
        public override void Activate()
        {
            Find.WindowStack.Add(new XmlExtensions_MenuModSettings());
        }
    }
}