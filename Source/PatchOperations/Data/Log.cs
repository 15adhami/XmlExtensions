using System;
using System.Xml;

namespace XmlExtensions
{
    public class Log : PatchOperationExtendedPathed
    {
        protected string text = null;
        protected string error = null;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                if (text == null && xpath == null && error == null)
                {
                    Verse.Log.Message("XmlExtensions.Log");
                }
                if (text != null)
                    Verse.Log.Message(text);
                if (error != null)
                    Verse.Log.Error(error);
                if (xpath != null)
                {
                    XmlNodeList nodeList;
                    nodeList = xml.SelectNodes(this.xpath);
                    if (nodeList == null || nodeList.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.Log(xpath=" + xpath + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    foreach (XmlNode node in nodeList)
                    {
                        Verse.Log.Message(node.OuterXml);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Log: " + e.Message);
                return false;
            }
        }
    }

}
