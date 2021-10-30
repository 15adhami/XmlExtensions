using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationExtendedPathed : PatchOperationExtended
    {
        public string xpath;
        protected XmlNodeList nodes;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (xpath == null)
            {
                NullError("xpath");
                return false;
            }
            nodes = xml.SelectNodes(xpath);
            if (nodes == null || nodes.Count == 0)
            {
                XPathError();
                return false;
            }
            return true;
        }

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
        }
    }
}