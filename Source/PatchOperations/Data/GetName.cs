using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class GetName : PatchOperationValue
    {
        public string storeIn;
        public string brackets = "{}";
        public XmlContainer apply;

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
                    PatchManager.errors.Add("XmlExtensions.GetName(xpath=" + xpath + "): Error in operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetName(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }

        public override bool getValue(ref string value, XmlDocument xml)
        {
            try
            {
                XmlNode node = xml.SelectSingleNode(xpath);
                if(node == null)
                {
                    PatchManager.errors.Add("XmlExtensions.GetName(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                value = node.Name.ToString();
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.GetName(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

}
