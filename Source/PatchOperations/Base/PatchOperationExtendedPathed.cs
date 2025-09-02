using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace XmlExtensions
{
    internal abstract class PatchOperationExtendedPathed : PatchOperationExtended
    {
        public string xpath;
        public bool selectSingleNode = false;
        protected IEnumerable<XmlNode> nodes;
        //protected XmlNodeList nodes;
        protected int nodeCount = 0;

        protected override bool PreCheck(XmlDocument xml)
        {
            if (xpath == null)
            {
                NullError("xpath");
                return false;
            }
            if (selectSingleNode)
            {
                XmlNode node = Helpers.SelectSingleNode(xpath, xml, this);
                if (node != null)
                    nodeCount = 1;
                List<XmlNode> tempList = [node];
                //nodes = (XmlNodeList)tempList;
                nodes = (IEnumerable<XmlNode>)tempList;
            }
            else
            {
                XmlNodeList nodeList = Helpers.SelectNodes(xpath, xml, this);
                if (nodeList != null)
                    nodeCount = nodeList.Count;
                nodes = nodeList.Cast<XmlNode>();
            }
            if (nodes == null || nodeCount == 0)
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