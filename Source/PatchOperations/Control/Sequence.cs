using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class Sequence : PatchOperationExtended
    {
        public XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            return RunPatches(apply, xml);
        }
    }
}