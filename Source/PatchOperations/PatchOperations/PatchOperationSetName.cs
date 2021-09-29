using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationSetName : PatchOperationExtendedPathed
	{
		protected string name;

		protected override bool applyWorker(XmlDocument xml)
		{
            try
            {
				if (name == null)
                {
					PatchManager.errors.Add("XmlExtensions.PatchOperationSetName(xpath=" + xpath + "): Name is null");
					return false;
				}
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationSetName(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (XmlNode xmlNode in nodeList)
				{
					result = true;
					XmlNode xmlNode2 = xmlNode.OwnerDocument.CreateElement(name);
					xmlNode2.InnerXml = xmlNode.InnerXml;
					xmlNode.ParentNode.InsertBefore(xmlNode2, xmlNode);
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationSetName(xpath=" + xpath + ", name=" + name + "): " + e.Message);
				return false;
			}
		}
	}
}
