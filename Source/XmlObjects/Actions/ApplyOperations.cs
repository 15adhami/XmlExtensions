using System;
using System.Xml;
using Verse;

namespace XmlExtensions.Action
{
    public class ApplyOperations : ActionContainer
    {
        public XmlContainer apply;

        protected override bool ApplyAction()
        {
            int c = 0;
            foreach (XmlNode node in apply.node)
            {
                c++;
                PatchOperationExtended operation = DirectXmlToObject.ObjectFromXml<PatchOperationExtended>(node, false);
                try
                {
                    if (!operation.Apply(null))
                    {
                        Error("Failed to apply the operation at position=" + c.ToString());
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Error("Failed to apply the operation at position=" + c.ToString() + "\n" + e.Message);
                    return false;
                }
            }
            return true;
        }
    }
}