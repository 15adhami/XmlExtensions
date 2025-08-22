using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ScopedPatch : PatchOperationExtended
    {
        internal string xpath;
        public XmlContainer apply;

        protected override void SetException()
        {
            CreateExceptions(xpath, "xpath");
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (xpath == null)
            {
                NullError("xpath");
                return false;
            }
            XmlNode originalNode = xml.SelectSingleNode(xpath);
            if (originalNode == null)
            {
                XPathError("xpath");
                return false;
            }
            if (originalNode.NodeType != XmlNodeType.Element)
            {
                Error("The node selected by <xpath> must be an element.");
                return false;
            }

            XmlDocument tempDoc = new();
            XmlNode clonedRoot = tempDoc.ImportNode(originalNode, true);
            tempDoc.AppendChild(clonedRoot);

            if (!RunPatches(apply, tempDoc))
            {
                return false;
            }

            XmlElement origElem = (XmlElement)originalNode;
            XmlElement patchedElem = tempDoc.DocumentElement;

            origElem.RemoveAll();

            if (patchedElem.HasAttributes)
            {
                foreach (XmlAttribute attr in patchedElem.Attributes)
                {
                    XmlAttribute importedAttr = (XmlAttribute)xml.ImportNode(attr, true);
                    origElem.Attributes.Append(importedAttr);
                }
            }

            foreach (XmlNode child in patchedElem.ChildNodes)
            {
                XmlNode importedChild = xml.ImportNode(child, true);
                origElem.AppendChild(importedChild);
            }

            return true;
        }
    }
}
