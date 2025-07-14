namespace XmlExtensions.Action
{
    internal class SetActiveMenu : ActionContainer
    {
        public string menu;

        protected override bool ApplyAction()
        {
            SetActiveMenu(menu);
            return true;
        }
    }
}
