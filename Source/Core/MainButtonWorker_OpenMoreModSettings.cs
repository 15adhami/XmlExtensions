using RimWorld;
using Verse;
using Verse.Sound;

namespace XmlExtensions
{
    internal class MainButtonWorker_OpenMoreModSettings : MainButtonWorker
    {
        public MainButtonDef OpenTab => Find.WindowStack.WindowOfType<MainTabWindow>()?.def;
        
        public override void Activate()
        {
            if (this.OpenTab != null)
                Find.WindowStack.TryRemove((Window) this.OpenTab.TabWindow, false);
            Find.WindowStack.Add(new XmlExtensions_MenuModSettings());
            SoundDefOf.TabOpen.PlayOneShotOnCamera();
        }
    }
}