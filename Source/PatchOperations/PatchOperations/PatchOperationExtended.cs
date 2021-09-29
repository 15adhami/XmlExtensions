using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchOperationExtended : PatchOperation
    {
        public string xmlDoc;

        protected sealed override bool ApplyWorker(XmlDocument xml)
        {
            return applyWorker(xmlDoc == null?xml:PatchManager.XmlDocs[xmlDoc]);
        }

        protected virtual bool applyWorker(XmlDocument xml)
        {
            return false;
        }
    }
}