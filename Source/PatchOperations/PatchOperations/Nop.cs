using System.Xml;

namespace XmlExtensions
{
    internal class Nop : PatchOperationExtended
    {
        protected override bool Patch(XmlDocument xml)
        {
            return true;
        }
    }
}