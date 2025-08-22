using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ScopedPatch : PatchOperationExtendedPathed
    {
        public XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            XmlNode originalNode = nodes[0];
            XmlDocument tempDoc = new();
            XmlNode clonedRoot = tempDoc.ImportNode(originalNode, true);
            tempDoc.AppendChild(clonedRoot);

            if (!RunPatches(apply, tempDoc))
            {
                return false;
            }

            originalNode.ReplaceWith(clonedRoot);

            return true;
        }
    }
}
