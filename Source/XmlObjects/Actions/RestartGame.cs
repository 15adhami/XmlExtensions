using Verse;

namespace XmlExtensions.Action
{
    internal class RestartGame : ActionContainer
    {
        protected override bool ApplyAction()
        {
            Find.WindowStack.TryRemove(typeof(ModSettings_Window));
            Find.WindowStack.TryRemove(typeof(XmlExtensions_MenuModSettings));
            GenCommandLine.Restart();
            return true;
        }
    }
}