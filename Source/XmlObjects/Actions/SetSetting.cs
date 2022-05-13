namespace XmlExtensions.Action
{
    internal class SetSetting : ActionContainer
    {
        public string key;
        public string value;
        public string value2;
        public string operation = "";
        public bool isKey = false;
        public bool isKey2 = false;

        protected override bool ApplyAction()
        {
            string val;
            string newStr1 = value;
            string newStr2 = value2;

            if (isKey)
            {
                newStr1 = SettingsManager.GetSetting(modId, value);
            }
            if (isKey2)
            {
                newStr2 = SettingsManager.GetSetting(modId, value2);
            }
            if (operation == "")
            {
                val = newStr1;
            }
            else
            {
                val = Helpers.OperationOnString(newStr1, newStr2, operation);
            }
            SettingsManager.SetSetting(modId, key, val);
            return true;
        }
    }
}