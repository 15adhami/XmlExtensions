using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class UseSetting : PatchOperation
    {
        protected string modId;
        protected string key;
        protected string brackets = "{}";
        protected string defaultValue;
        protected string type;
        protected XmlContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.key, this.type);            
            string value;
            bool didContain = XmlMod.allSettings.dataDict.TryGetValue(this.modId + "." + this.key, out value);
            XmlContainer newContainer;
            if (!didContain)
            {
                value = defaultValue;
                XmlMod.addSetting(this.modId, this.key, defaultValue);
            }
            newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.key, value, this.brackets);
            Helpers.runPatchesInXmlContainer(newContainer, xml);
            return true;
        }
    }

    public class CreateSettings : PatchOperation
    {
        protected string modId;
        protected string label;
        protected List<SettingContainer> settings;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.label);
            foreach(SettingContainer setting in settings)
            {
                XmlMod.tryAddSettings(setting, modId);
            }
            return true;
        }
    }

    public abstract class SettingContainer : XmlContainer
    {
        public string key;
        public string label;
        public string defaultValue;
    }

    public class Textbox : SettingContainer
    {
     
    }

    public class Text : SettingContainer
    {

    }

    public class Checkbox : SettingContainer
    {

    }

    public class Range : SettingContainer
    {
        public int defaultMin;
        public int defaultMax;
    }
    public class Slider : SettingContainer
    {
        public int defaultMin;
        public int defaultMax;
    }
}
