using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class TryCatch : PatchOperationExtended
    {
        public XmlContainer tryApply;
        public XmlContainer catchApply;

        protected override bool Patch(XmlDocument xml)
        {
            if (!RunPatches(tryApply, "tryApply", xml))
            {
                ErrorManager.ClearErrors();
                if (catchApply != null)
                {
                    return RunPatches(catchApply, "catchApply", xml);
                }
            }
            return true;
        }
    }
}