using Verse;

namespace XmlExtensions.Action
{
    public class PrintKey : KeyedAction
    {
        public XmlContainer apply;

        protected override bool ApplyKeyedAction(string oldValue, string newValue)
        {
            Verse.Log.Message(key + ": " + SettingsManager.GetSetting(modId, key));
            return true;
        }
    }
}