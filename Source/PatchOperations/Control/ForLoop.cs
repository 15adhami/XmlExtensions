using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class ForLoop : PatchOperationExtended
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from;
        protected int to;
        protected int increment = 1;

        protected override bool Patch(XmlDocument xml)
        {
            int errNum = 0;
            string oldXml = this.apply.node.OuterXml;
            if (this.increment > 0)
            {
                for (int i = this.from; i < this.to; i += increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.ForLoop: Error at iteration " + i.ToString() + ", in the operation as position=" + errNum.ToString());
                        return false;
                    }
                }
            }
            else if (this.increment < 0)
            {
                for (int i = this.from - 1; i >= this.to; i -= increment)
                {
                    XmlContainer newContainer = Helpers.substituteVariableXmlContainer(this.apply, this.storeIn, i.ToString(), this.brackets);
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.ForLoop: Error at iteration " + i.ToString() + ", in the operation as position=" + errNum.ToString());
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

