using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class Sequence : PatchOperationExtended
    {
        public XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            return RunPatches(apply, xml);
        }
    }
}
