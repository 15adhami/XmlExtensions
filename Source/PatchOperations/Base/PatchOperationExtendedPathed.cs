using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationExtendedPathed : PatchOperationExtended
    {
        public string xpath;
        public bool selectSingleNode = false;
        protected List<XmlNode> nodes;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (xpath == null)
            {
                NullError("xpath");
                return false;
            }
            if (selectSingleNode)
                nodes = new() { Helpers.SelectSingleNode(xpath, xml) };
            else
                nodes = Helpers.SelectNodes(xpath, xml).Cast<XmlNode>().ToList();
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