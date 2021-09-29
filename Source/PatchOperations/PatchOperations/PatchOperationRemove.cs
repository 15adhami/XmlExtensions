using System;
using System.Xml;

namespace XmlExtensions
{
    public class PatchOperationRemove : PatchOperationExtendedPathed
	{
		protected override bool applyWorker(XmlDocument xml)
		{
            try
            {
				bool result = false;
				XmlNodeList nodeList = xml.SelectNodes(xpath);
				if (nodeList == null || nodeList.Count == 0)
				{
					PatchManager.errors.Add("XmlExtensions.PatchOperationRemove(xpath=" + xpath + "): Failed to find a node with the given xpath");
					return false;
				}
				foreach (XmlNode xmlNode in nodeList)
				{
					result = true;
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
				return result;
			}
			catch (Exception e)
			{
				PatchManager.errors.Add("XmlExtensions.PatchOperationRemove(xpath=" + xpath + "): " + e.Message);
				return false;
			}
		}
	}
}
