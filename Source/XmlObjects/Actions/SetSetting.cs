﻿namespace XmlExtensions.Action
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
                if (!SettingsManager.TryGetSetting(modId, value, out newStr1))
                {
                    Error("The key given in <value> does not exist");
                    return false;
                }
            }
            if (isKey2)
            {
                if (!SettingsManager.TryGetSetting(modId, value2, out newStr2))
                {
                    Error("The key given in <value2> does not exist");
                    return false;
                }
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