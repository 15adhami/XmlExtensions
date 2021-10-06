using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSetName : PatchOperationExtendedPathed
    {
        protected string name;

        protected override bool Patch(XmlDocument xml)
        {
            if (name == null)
            {
                Error("<name> is null");
                return false;
            }
            foreach (XmlNode xmlNode in nodes)
            {
                XmlNode xmlNode2 = xmlNode.OwnerDocument.CreateElement(name);
                xmlNode2.InnerXml = xmlNode.InnerXml;
                xmlNode.ParentNode.InsertBefore(xmlNode2, xmlNode);
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }
            return true;
        }

        protected override void SetException()
        {
            exceptionVals = new string[] { xpath, name };
            exceptionFields = new string[] { "xpath", "name" };
        }
    }
}
