using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationSafeRemove : PatchOperationExtended
    {
        public string xpath;

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
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