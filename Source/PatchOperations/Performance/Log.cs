using System.Xml;

namespace XmlExtensions
{
    internal class Log : PatchOperationExtended
    {
        protected string text;
        protected string warning;
        protected string error;
        protected string xpath;

        protected override void SetException()
        {
            if (xpath != null)
            {
                CreateExceptions(xpath, "xpath");
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (text == null && xpath == null && error == null && warning == null)
            {
                Verse.Log.Message("XmlExtensions.Log");
            }
            if (text != null)
                Verse.Log.Message(text);
            if (warning != null)
                Verse.Log.Warning(warning);
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