using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Conditional : BooleanBase
    {
        public string xpath;

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
        }

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            XmlNode node = xml.SelectSingleNode(xpath);
            b = (node != null);
            return true;
        }
    }
}