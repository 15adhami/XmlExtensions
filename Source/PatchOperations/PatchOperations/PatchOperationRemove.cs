using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationRemove : PatchOperationExtendedPathed
    {
        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode xmlNode in nodes)
            {
                xmlNode.ParentNode.RemoveChild(xmlNode);
            }
            return true;
        }
    }
}