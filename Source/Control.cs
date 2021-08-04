using System.Xml;
using System.Collections.Generic;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public class ForLoop : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from;
        protected int to;
        protected int increment = 1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string oldXml = this.apply.node.OuterXml;
            if (this.increment > 0)
            {
                for (int i = this.from; i < this.to; i += increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    Helpers.runPatchesInXmlContainer(newContainer, xml);
                }
            }
            else if (this.increment < 0)
            {
                for (int i = this.from - 1; i >= this.to; i -= increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    Helpers.runPatchesInXmlContainer(newContainer, xml);
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
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, prefix, this.brackets);
                Helpers.runPatchesInXmlContainer(newContainer, xml);
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
                    Helpers.runPatchesInXmlContainer(this.caseTrue, xml);
                    return true;
                }
            }
            else
            {
                if (this.caseFalse != null)
                {
                    Helpers.runPatchesInXmlContainer(this.caseFalse, xml);
                }
            }
                return false;
        }

    }
    
}

