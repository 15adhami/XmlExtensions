using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSafeRemove : PatchOperationExtendedPathed
    {
        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                foreach (XmlNode xmlNode in xml.SelectNodes(this.xpath))
                {
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
            }
            catch { }
            return true;
        }
    }
}