using System.Xml;

namespace XmlExtensions
{
    public abstract class PatchOperationValue : PatchOperationExtendedPathed
    {
        public bool GetValue(ref string val, XmlDocument xml)
        {
            XmlDocument doc = xml;
            if (xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + "(xmlDoc=" + xmlDoc + "): No document exists with the given name");
                    return false;
                }
                else
                    doc = PatchManager.XmlDocs[xmlDoc];
            }
            return getValue(ref val, doc);
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
