using System;
using System.Xml;
using Verse;
using XmlExtensions.Boolean;

namespace XmlExtensions
{
    public class EvaluateBoolean : PatchOperationValue
    {
        public BooleanBase condition;
        public string storeIn;
        public string brackets = "{}";
        public XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            try
            {
                if (storeIn == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean: <storeIn>=null");
                    return false;
                }                
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <apply>=null");
                    return false;
                }                
                int errNum = 0;
                string temp = "";
                if (!GetValue(ref temp, xml))
                {
                    return false;
                }
                XmlContainer newContainer = Helpers.substituteVariableXmlContainer(apply, storeIn, temp, brackets);
                if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): Error in operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }            
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): " + e.Message);
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
                if (condition == null)
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): <condition>=null");
                    return false;
                }
                bool b = false;
                if (!condition.evaluate(ref b, xml))
                {
                    PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): Failed to evaluate <condition>");
                    return false;
                }
                value = b.ToString();
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.EvaluateBoolean(storeIn=" + storeIn + "): " + e.Message);
                return false;
            }
        }
    }

}
