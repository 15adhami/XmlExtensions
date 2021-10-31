using System.Xml;

namespace XmlExtensions
{
    internal class Test : PatchOperationExtendedPathed
    {
        protected override bool Patch(XmlDocument xml)
        {
            return true;
        }
    }
}