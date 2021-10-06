using System;
using System.Xml;

namespace XmlExtensions
{
    public class Log : PatchOperationExtended
    {
        protected string text;
        protected string error;
        protected string xpath;

        protected override void SetException()
        {
            if (xpath != null)
            {
                exceptionVals = new string[] { xpath };
                exceptionFields = new string[] { "xpath" };
            }
        }

        protected override bool Patch(XmlDocument xml)
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
                nodeList = xml.SelectNodes(xpath);
                if (nodeList == null || nodeList.Count == 0)
                {
                    XPathError();
                    return false;
                }
                foreach (XmlNode node in nodeList)
                {
                    Verse.Log.Message(node.OuterXml);
                }
            }
            return true;
        }
    }

}
