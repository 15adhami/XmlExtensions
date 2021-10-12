using System.Xml;
using Verse;

namespace XmlExtensions.Action
{
    public class ApplyOperations : MenuAction
    {
        public XmlContainer apply;

        protected override bool ApplyAction()
        {
            int c = 0;
            foreach (XmlNode node in apply.node)
            {
                c++;
                PatchOperationExtended operation = DirectXmlToObject.ObjectFromXml<PatchOperationExtended>(node, false);
                if (!operation.Apply(null))
                {
                    Error("Failed to apply the operation at position=" + c.ToString());
                    return false;
                }
            }
            return true;
        }
    }
}