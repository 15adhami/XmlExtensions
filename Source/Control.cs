using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class ForLoop : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from = 0;
        protected int to = 1;
        protected int increment = 1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string oldXml = this.apply.node.OuterXml;
            if (this.increment > 0)
            {
                for (int i = this.from; i < this.to; i += increment)
                {
                    string newXml = Helpers.substituteVariable(oldXml, this.storeIn, i.ToString(), this.brackets);
                    XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                    for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                        patch.Apply(xml);
                    }
                }
            }
            else if (this.increment < 0)
            {
                for (int i = this.from - 1; i >= this.to; i -= increment)
                {
                    string newXml = Helpers.substituteVariable(oldXml, this.storeIn, i.ToString(), this.brackets);
                    XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                    for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                        patch.Apply(xml);
                    }
                }
            }       
            return true;
        }
    }

    public class ForEach : PatchOperationPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;
        protected override bool ApplyWorker(XmlDocument xml)
        {

            foreach (object obj in xml.SelectNodes(this.xpath))
            {
                //Calculate prefix for variable
                XmlNode xmlNode = obj as XmlNode;
                string path = xmlNode.GetXPath();
                string prefix = Helpers.getPrefix(path, prefixLength);
                string temp = Helpers.substituteVariable(this.apply.node.OuterXml, storeIn, prefix, brackets);
                XmlContainer newContainer = new XmlContainer() { node =  Helpers.getNodeFromString(temp)};
                for (int i = 0; i < this.apply.node.ChildNodes.Count; i++)
                {
                    PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[i].OuterXml);
                    patch.Apply(xml);
                }
            }
            return true;
        }

    }
  
    public class IfStatement : PatchOperationPathed
    {
        protected PatchOperationBoolean condition = null;
        protected XmlContainer caseTrue =  null;
        protected XmlContainer caseFalse = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (this.condition.evaluate(xml))
            {
                if (this.caseTrue != null)
                {
                    for (int i = 0; i < this.caseTrue.node.ChildNodes.Count; i++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(this.caseTrue.node.ChildNodes[i].OuterXml);
                        patch.Apply(xml);
                    }
                    return true;
                }
            }
            else
            {
                if (this.caseFalse != null)
                {
                    for (int i = 0; i < this.caseFalse.node.ChildNodes.Count; i++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(this.caseFalse.node.ChildNodes[i].OuterXml);
                        patch.Apply(xml);
                    }
                    return true;
                }
            }
                return false;
        }

    }

    public class DelayPatches : PatchOperation
    {
        protected XmlContainer patches = null;
        protected bool applyHere = false;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            for (int i = 0; i < this.patches.node.ChildNodes.Count; i++)
            {
                XmlNode node = patches.node.ChildNodes[i];
                PatchManager.enqueuePatch(node);
                if (applyHere)
                {
                    PatchOperation patch = Helpers.getPatchFromString(node.OuterXml);
                    patch.Apply(xml);
                }
            }
            return true;
        }
    }
    
}

