using System;
using Verse;

namespace XmlExtensions
{
    public static class SettingsManager
    {
        public static string GetSetting(string modId, string key)
        {
            return XmlMod.allSettings.dataDict[modId + ";" + key];
        }

        public static bool TryGetSetting(string modId, string key, out string value)
        {
            string temp = "";
            bool b;
            b = XmlMod.allSettings.dataDict.TryGetValue(modId + ";" + key, out temp);
            value = temp;
            return b;
        }

        public static void SetSetting(string modId, string key, string value)
        {
            XmlMod.setSetting(modId, key, value);
        }

        public static void ScribeLookf()
        {
            LoadedModManager.GetMod(typeof(XmlMod)).WriteSettings();
        }
		/*
		public static void ScribeLook<T>(Type t, ref T value, string label, T defaultValue = default(T), bool forceSave = false)
        {
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				if (typeof(T) == typeof(TargetInfo))
				{
					Verse.Log.Error("Saving a TargetInfo " + label + " with Scribe_Values. TargetInfos must be saved with Scribe_TargetInfo.");
					return;
				}
				if (typeof(Thing).IsAssignableFrom(typeof(T)))
				{
					Verse.Log.Error("Using Scribe_Values with a Thing reference " + label + ". Use Scribe_References or Scribe_Deep instead.");
					return;
				}
				if (typeof(IExposable).IsAssignableFrom(typeof(T)))
				{
					Verse.Log.Error("Using Scribe_Values with a IExposable reference " + label + ". Use Scribe_References or Scribe_Deep instead.");
					return;
				}
				if (typeof(Def).IsAssignableFrom(typeof(T)))
				{
					Verse.Log.Error("Using Scribe_Values with a Def " + label + ". Use Scribe_Defs instead.");
					return;
				}
				if (forceSave || (value == null && defaultValue != null) || (value != null && !value.Equals(defaultValue)))
				{
					if (value == null)
					{
						if (!Scribe.EnterNode(label))
						{
							return;
						}
						try
						{
							Scribe.saver.WriteAttribute("IsNull", "True");
							return;
						}
						finally
						{
							Scribe.ExitNode();
						}
					}
					Scribe.saver.WriteElement(label, value.ToString());
					return;
				}
			}
			else if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				value = ScribeExtractor.ValueFromNode<T>(Scribe.loader.curXmlParent[label], defaultValue);
			}
		}*/
    }
}
