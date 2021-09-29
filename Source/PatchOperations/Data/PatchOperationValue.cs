using System.Xml;

namespace XmlExtensions
{
    public abstract class PatchOperationValue : PatchOperationExtendedPathed
    {
        public bool GetValue(ref string val, XmlDocument xml)
        {
            XmlDocument xmldoc = xml;
            if (xmlDoc != null)
                xmldoc = PatchManager.XmlDocs[xmlDoc];
            return getValue(ref val, xmldoc);
        }

        public virtual bool getValue(ref string val, XmlDocument xml)
        {
            return false;
        }

        public virtual bool getVar(ref string var)
        {
            return false;
        }
    }

}
