using System.Xml;
using System.Collections.Generic;
using System;
using Verse;

namespace XmlExtensions
{
    public class Case
    {
        public string value;
        public XmlContainer apply;
    }

    public class PatchByCase : PatchOperationExtended
    {
        public string value;
        public List<Case> cases;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                int errNum = 0;
                if (value == null)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchByCase: <value> is null");
                    return false;
                }
                if (cases == null)
                {
                    PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): <cases> is null");
                    return false;
                }
                int c = 0;
                foreach (Case casePatch in cases)
                {
                    c++;
                    if (value == casePatch.value)
                    {
                        if (!Helpers.runPatchesInXmlContainer(casePatch.apply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): Error in case with <value>=" + value + ", in the operation at position=" + errNum.ToString());
                            return false;
                        }
                        return true;
                    }

                    // run first case as default case
                    if (c == cases.Count)
                    {
                        if (!Helpers.runPatchesInXmlContainer(cases[0].apply, xml, ref errNum))
                        {
                            PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): Error while running the first case as default case, in the operation at position=" + errNum.ToString());
                            return false;
                        }
                        return true;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.PatchByCase(value=" + value + "): " + e.Message);
                return false;
            }
        }

    }
}

