using System;
using System.Xml;
using Verse;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    [Obsolete]
    internal class CreateSettings : PatchOperation
    {
        public string modId;
        public string label;
        public int defaultSpacing = 2;
        public XmlContainer settings;
        public string tKey;

        public override bool ApplyWorker(XmlDocument xml)
        {
            //XmlMod.WarnUsingObselete(modId, this, [typeof(SettingsMenuDef)]);
            if (modId == null)
            {
                ErrorManager.AddError("XmlExtensions.CreateSettings: <modId>=null");
                return false;
            }
            if (label == null)
            {
                ErrorManager.AddError("XmlExtensions.CreateSettings(" + modId + "): <label>=null");
                return false;
            }
            try
            {
                string defStr = "";
                defStr += "<defName>" + modId.Replace('.', '_') + "</defName>";
                defStr += "<modId>" + modId + "</modId>";
                if (tKey != null)
                {
                    defStr += "<tKey>" + tKey + "</tKey>";
                }
                defStr += "<defaultSpacing>" + defaultSpacing.ToString() + "</defaultSpacing>";
                defStr += "<label>" + label + "</label>";
                defStr += settings.node.OuterXml;
                XmlNode node = xml.CreateNode("element", "XmlExtensions.SettingsMenuDef", null);
                node.InnerXml = defStr;
                xml.SelectSingleNode("/Defs").AppendChild(node);
            }
            catch
            {
                ErrorManager.AddError("XmlExtensions.CreateSettings(" + modId + "): Error");
                return false;
            }
            return true;
        }
    }
}