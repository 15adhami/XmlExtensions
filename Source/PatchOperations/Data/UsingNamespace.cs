using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class UsingNamespace : PatchOperationExtended
    {
        public XmlContainer apply;
        public string namespaces;

        protected override bool Patch(XmlDocument xml)
        {
            CustomXmlLoader.defaultNamespaces.Add(namespaces);
            if (!RunPatches(apply, xml))
            {
                CustomXmlLoader.defaultNamespaces.Remove(namespaces);
                return false;
            }
            CustomXmlLoader.defaultNamespaces.Remove(namespaces);
            return true;
        }
    }
}