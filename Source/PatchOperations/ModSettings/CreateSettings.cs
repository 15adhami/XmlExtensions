using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    [Obsolete]
    public class CreateSettings : PatchOperation
    {
        public string modId;
        public string label;
        public int defaultSpacing = 2;
        public XmlContainer settings;
        public string tKey;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            
            if (modId == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings: <modId>=null");
                return false;
            }
            if (label == null)
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): <label>=null");
                return false;
            }
            try
            {
                string defStr = "";
                defStr += "<defName>" + modId.Replace('.', '_') + "</defName>";
                defStr += "<modId>" + modId + "</modId>";
                defStr += "<tKey>" + (tKey==null?"":tKey) + "</tKey>";
                defStr += "<defaultSpacing>" + defaultSpacing.ToString() + "</defaultSpacing>";
                defStr += "<label>" + label + "</label>";
                defStr += settings.node.OuterXml;
                XmlNode node = xml.CreateNode("element", "XmlExtensions.SettingsMenuDef", null);
                node.InnerXml = defStr;
                xml.SelectSingleNode("/Defs").AppendChild(node);
            }
            catch
            {
                PatchManager.errors.Add("XmlExtensions.CreateSettings(" + modId + "): Error");
                return false;
            }
            return true;
        }
    }

}
