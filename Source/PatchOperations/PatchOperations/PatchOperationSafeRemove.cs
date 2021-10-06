using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSafeRemove : PatchOperationExtended
    {
        public string xpath;

        protected override void SetException()
        {
            exceptionVals = new string[] { xpath };
            exceptionFields = new string[] { "xpath" };
        }

        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in xml.SelectNodes(xpath))
            {
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }
            return true;
        }
    }
}