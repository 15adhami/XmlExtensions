using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Conditional : BooleanBase
    {
        public string xpath;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                XmlNode node = xml.SelectSingleNode(xpath);
                b = (node != null);
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

}
