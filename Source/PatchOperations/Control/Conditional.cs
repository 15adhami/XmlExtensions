using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
    public class Conditional : PatchOperationExtendedPathed
    {
        public XmlContainer caseTrue;
        public XmlContainer caseFalse;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                XmlNode node = xml.SelectSingleNode(xpath);
                if (node != null)
                {
                    if(caseTrue != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseTrue, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): Error in <caseTrue> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }                    
                }
                else
                {
                    if (caseFalse != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(caseFalse, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): Error in <caseFalse> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }                    
                }
                return caseTrue != null || caseFalse != null;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }
}

