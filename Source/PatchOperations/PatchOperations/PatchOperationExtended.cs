using System.Xml;
using Verse;

namespace XmlExtensions
{
    public abstract class PatchOperationExtended : PatchOperation
    {
        public string xmlDoc;

        protected sealed override bool ApplyWorker(XmlDocument xml)
        {
            XmlDocument doc = xml;
            if(xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + "(xmlDoc=" + xmlDoc + "): No document exists with the given name");
                    return false;
                }
                else
                    doc = PatchManager.XmlDocs[xmlDoc];
            }
            return Patch(doc);
        }

        protected virtual bool Patch(XmlDocument xml)
        {
            return false;
        }

        protected void xpathError(string xpath, string field)
        {
            PatchManager.errors.Add(this.GetType().ToString() + "(" + field + "=" + xpath + "): Failed to find a node with the given xpath");
        }
    }
}