using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class UsingNamespace : PatchOperationExtended
    {//TODO: Fix UsingNamespace
        public XmlContainer apply;
        public List<string> namespaces;
        /*

        protected override bool Patch(XmlDocument xml)
        {
            foreach (string name in namespaces)
            {
                CustomXmlLoader.defaultNamespaces.Add(name);
            }
            if (!RunPatches(apply, xml))
            {
                foreach (string name in namespaces)
                {
                    CustomXmlLoader.defaultNamespaces.Remove(name);
                }
                return false;
            }
            foreach (string name in namespaces)
            {
                CustomXmlLoader.defaultNamespaces.Remove(name);
            }
            return true;
        }
        */
    }
}