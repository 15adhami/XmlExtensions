using System.Xml;

namespace XmlExtensions
{
    public class Test : PatchOperationExtendedPathed
    {
        protected override bool Patch(XmlDocument xml)
        {
            return true;
        }
    }
}