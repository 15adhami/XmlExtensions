using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class GetAttribute : PatchOperationValue
    {
        public XmlContainer apply;
        public string attribute;
        public string storeIn;
        public string brackets = "{}";        

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                string temp = "";
                if (!GetValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, temp, brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Error in operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): " + e.Message);
                return false;
            }
        }

        public override bool getValue(ref string value, XmlDocument xml)
        {
            try
            {
                XmlAttribute xattribute;
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Failed to find a node with the given xpath");
                    return false;
                }
                xattribute = node.Attributes[this.attribute];
                if (xattribute == null)
                {
                    PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): Could not find attribute");
                    return false;
                }
                value = xattribute.Value;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetAttribute(xpath=" + xpath + ", attribute=" + attribute + "): "+e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }
    }

}
