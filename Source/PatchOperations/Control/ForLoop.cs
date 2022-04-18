using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ForLoop : PatchOperationExtended
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
            int count = 0;
            for (i = from; (increment > 0) ? (i < to) : (i > to); i += increment)
            {
                count++;
                if (count >= 10000)
                {
                    Error("Loop limit reached");
                    return false;
                }
                XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, i.ToString(), brackets);
                if (!RunPatches(newContainer, xml)) { return false; }
            }
            return true;
        }
    }
}