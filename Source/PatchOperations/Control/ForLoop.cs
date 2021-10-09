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

        private int i = 0;

        protected override void SetException()
        {
            CreateExceptions(i.ToString(), storeIn);
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (increment > 0)
            {
                for (i = from; i < to; i += increment)
                {
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, i.ToString(), brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            else if (increment < 0)
            {
                for (i = from - 1; i >= to; i -= increment)
                {
                    XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, i.ToString(), brackets);
                    if (!RunPatches(newContainer, xml)) { return false; }
                }
            }
            return true;
        }
    }
}

