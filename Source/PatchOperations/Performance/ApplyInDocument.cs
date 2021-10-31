using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class ApplyInDocument : PatchOperation
    {
        public string docName = "Defs";
        public PatchContainer apply;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                if (!PatchManager.XmlDocs.ContainsKey(docName))
                {
                    ErrorManager.AddError("XmlExtensions.ApplyInDocument(docName=" + docName + "): No document exists with the given name");
                    return false;
                }
                if (apply == null)
                {
                    ErrorManager.AddError("XmlExtensions.ApplyInDocument(docName=" + docName + "): <apply> is null");
                    return false;
                }
                int errNum = 0;
                XmlDocument doc = PatchManager.XmlDocs[docName];
                if (!Helpers.RunPatchesInPatchContainer(apply, doc, ref errNum))
                {
                    ErrorManager.AddError("XmlExtensions.ApplyInDocument(docName=" + docName + "): Error in the operation at position=" + errNum.ToString());
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorManager.AddError("XmlExtensions.ApplyInDocument(docName=" + docName + "): " + e.Message);
                return false;
            }
        }
    }
}