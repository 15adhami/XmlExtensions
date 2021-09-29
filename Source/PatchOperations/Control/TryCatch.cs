using System.Xml;
using Verse;
using System;

namespace XmlExtensions
{
    public class TryCatch : PatchOperationExtended
    {
        public PatchContainer tryApply;
        public XmlContainer catchApply;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                if (!Helpers.runPatchesInPatchContainer(tryApply, xml, ref errNum))
                {
                    errNum = 0;
                    PatchManager.errors.Clear();
                    if (catchApply != null)
                    {
                        if (!Helpers.runPatchesInXmlContainer(catchApply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.TryCatch: Error in <catchApply> in the operation at position=" + errNum.ToString());
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.TryCatch: " + e.Message);
                return false;
            }
        }
    }
}

