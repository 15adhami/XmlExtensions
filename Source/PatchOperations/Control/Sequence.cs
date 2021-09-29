using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlExtensions
{
    public class Sequence : PatchOperationExtended
    {
        public PatchContainer apply;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                if(!Helpers.runPatchesInPatchContainer(apply, xml, ref errNum))
                {
                    PatchManager.errors.Add("XmlExtensions.Sequence: Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Sequence: " + e.Message);
                return false;
            }
        }
    }
}
