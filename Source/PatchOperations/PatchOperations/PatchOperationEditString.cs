using System.Linq;
using System.Xml;

namespace XmlExtensions
{
    internal class PatchOperationEditString : PatchOperationExtendedPathed
    {
        public string substring;
        public string replaceWith;

        protected override bool Patch(XmlDocument xml)
        {
            substring ??= nodes.First().InnerText;
            foreach (XmlNode xmlNode in nodes)
            {
                if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.InnerText.Contains(substring))
                {
                    xmlNode.InnerText = xmlNode.InnerText.Replace(substring, replaceWith);
                }
            }
            return true;
        }
    }
}