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

        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlMod.loadedMod = this.modId;
            XmlMod.addXmlMod(this.modId, this.label);
            return true;
        }
    }
}
