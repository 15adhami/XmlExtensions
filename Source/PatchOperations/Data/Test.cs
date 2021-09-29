using System;
using System.Linq;
using System.Xml;

namespace XmlExtensions
{


    public class Test : PatchOperationExtendedPathed
    {
        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                if (xml.SelectSingleNode(xpath) == null)
                {
                    PatchManager.errors.Add("XmlExtensions.Test(xpath=" + xpath + "): Failed to find a node with the given xpath");
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Test(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

}
