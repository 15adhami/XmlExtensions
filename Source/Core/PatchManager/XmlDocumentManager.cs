using RimWorld;
using System.Collections.Generic;
using System.Xml;

namespace XmlExtensions
{
    internal class XmlDocumentManager
    {
        public XmlDocument MainDocument { get; set; }
        public Dictionary<string, XmlDocument> NamedDocuments { get; } = new();
        public Dictionary<string, Dictionary<XmlNode, XmlNode>> NodeMaps { get; } = new();

        public void Register(string name, XmlDocument doc)
        {
            NamedDocuments.Add(name, doc);
        }
        public XmlDocument Get(string name)
        {
            return NamedDocuments[name];
        }

        public void Add(string name, XmlDocument xmlDoc)
        {
            NamedDocuments.Add(name, xmlDoc);
        }

        public void Clear()
        {
            NamedDocuments.Clear();
        }

        public bool Contains(string name)
        {
            return NamedDocuments.ContainsKey(name);
        }

        public void Remove(string name)
        {
            NamedDocuments.Remove(name);
        }

        public void RegisterNodeMap(string name)
        {
            NodeMaps.Add(name, new());
        }

        public void ClearNodeMaps()
        {
            NodeMaps.Clear();
        }

        public Dictionary<XmlNode, XmlNode> GetNodeMap(string name)
        {
            return NodeMaps[name];
        }

        public void AddNodesToMap(string name, XmlNode newNode, XmlNode node)
        {
            GetNodeMap(name).Add(newNode, node);
        }

        public bool NodeMapContainsKey(string name, XmlNode node)
        {
            return GetNodeMap(name).ContainsKey(node);
        }

        public XmlNode GetNodeFromNodeMap(string name, XmlNode node)
        {
            return GetNodeMap(name)[node];
        }

        public void ClearNodeMap(string name)
        {
            GetNodeMap(name).Clear();
        }

        public void RemoveNodeFromNodeMap(string docName, XmlNode node)
        {
            GetNodeMap(docName).Remove(node);
        }

        public void RemoveNodeMap(string docName)
        {
            NodeMaps.Remove(docName);
        }
    }
}