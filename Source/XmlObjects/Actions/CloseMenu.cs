using Verse;

namespace XmlExtensions.Action
{
    internal class CloseMenu : ActionContainer
    {
        protected override bool ApplyAction()
        {
            Find.WindowStack.TryGetWindow(out BaseSettingsWindow window);
            if (window != null)
            {
                window.shouldClose = true;
            }
            return true;
        }
    }
}