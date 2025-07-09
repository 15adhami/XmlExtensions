using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class XmlDocumentManager
    {
        public XmlDocument MainDocument { get; private set; }
        public Dictionary<string, XmlDocument> NamedDocuments { get; } = new();
        public Dictionary<string, Dictionary<XmlNode, XmlNode>> NodeMaps { get; } = new();

        public void Register(string name, XmlDocument doc) { /* ... */ }
        public XmlDocument Get(string name) { /* ... */ }
    }
}
