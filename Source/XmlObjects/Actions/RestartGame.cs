using Verse;

namespace XmlExtensions.Action
{
    internal class RestartGame : ActionContainer
    {
        protected override bool ApplyAction()
        {
            Find.WindowStack.TryGetWindow(out BaseSettingsWindow window);
            if (window != null)
            {
                window.shouldClose = 2;
            }
            return true;
        }
    }
}