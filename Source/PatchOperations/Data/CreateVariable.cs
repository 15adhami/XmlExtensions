using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class CreateVariable : PatchOperationValue
    {
        public XmlContainer apply;
        public string storeIn;
        public string brackets = "{}";
        public string value = "";
        public string value2 = "";
        public string defaultValue;
        public string defaultValue2;
        public bool fromXml = false;
        public bool fromXml2 = false;
        public string operation = "";        

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                string newStr1 = this.value;
                string newStr2 = this.value2;
                int errNum = 0;
                string ans = "";
                if(!GetValue(ref ans, xml))
                { // Error message already added
                    return false;
                }
                if (operation == "")
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, ans, brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                else
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, ans, brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.CreateVariable(calculatedValue=" + ans + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): " + e.Message);
                return false;
            }
        }

        public override bool getVar(ref string var)
        {
            var = storeIn;
            return true;
        }

        public override bool getValue(ref string val, XmlDocument xml)
        {
            try
            {
                string newStr1 = this.value;
                string newStr2 = this.value2;
                if (this.fromXml)
                {
                    XmlNode node = xml.SelectSingleNode(this.value);
                    if (node == null)
                    {
                        if (defaultValue == null)
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        newStr1 = defaultValue;
                    }
                    else
                    {
                        newStr1 = node.InnerXml;
                    }
                }
                if (this.fromXml2)
                {
                    XmlNode node = xml.SelectSingleNode(this.value2);
                    if (node == null)
                    {
                        if (defaultValue2 == null)
                        {
                            PatchManager.errors.Add("XmlExtensions.CreateVariable(value2=" + value2 + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        newStr2 = defaultValue2;
                    }
                    else
                    {
                        newStr2 = node.InnerXml;
                    }
                }
                if (operation == "")
                {
                    val = newStr1;
                }
                else
                {
                    val = Helpers.operationOnString(newStr1, newStr2, operation);
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.CreateVariable(value=" + value + "): " + e.Message);
                return false;
            }
        }
    }

}
