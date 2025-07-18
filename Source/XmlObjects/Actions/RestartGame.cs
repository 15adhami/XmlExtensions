using Verse;

namespace XmlExtensions.Action
{
    internal class RestartGame : ActionContainer
    {
        protected override bool ApplyAction()
        {
            Find.WindowStack.TryRemove(typeof(ModSettingsWindow));
            Find.WindowStack.TryRemove(typeof(XmlExtensionsMenuModSettings));
            GenCommandLine.Restart();
            return true;
        }
    }
}