namespace XmlExtensions.Action
{
    internal class Log : ActionContainer
    {
        protected string text;
        protected string warning;
        protected string error;
        protected string key;

        protected override bool ApplyAction()
        {
            if (text == null && key == null && error == null && warning == null)
                Verse.Log.Message("XmlExtensions.Log");
            if (text != null)
                Verse.Log.Message(text);
            if (warning != null)
                Verse.Log.Warning(warning);
            if (error != null)
                Verse.Log.Error(error);
            if (key != null)
                Verse.Log.Message(SettingsManager.GetSetting(modId, key));
            return true;
        }
    }
}