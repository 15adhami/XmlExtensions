using System.Xml;
using XmlExtensions.Boolean;
using System;

namespace XmlExtensions
{
    public class WhileLoop : PatchOperationExtended
    {
        public BooleanBase condition;
        public PatchContainer apply;

        protected override bool applyWorker(XmlDocument xml)
        {
            int c = 0;
            try
            {
                c++;
                if (condition == null)
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop: <condition> is null");
                    return false;
                }
                if (apply == null)
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop: <apply> is null");
                    return false;
                }
                bool b = false;
                if(!condition.evaluate(ref b, xml))
                {
                    PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Failed to evaluate condition");
                    return false;
                }
                while(b)
                {
                    int errNum = 0;
                    if(!Helpers.runPatchesInPatchContainer(apply, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    c++;
                    if(c>=10000)
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Loop limit reached");
                        return false;
                    }
                    if (!condition.evaluate(ref b, xml))
                    {
                        PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): Failed to evaluate condition");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.WhileLoop(iter_num=" + c.ToString() + "): " + e.Message);
                return false;
            }
        }
    }
}

