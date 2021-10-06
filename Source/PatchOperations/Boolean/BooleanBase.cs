using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public abstract class BooleanBase
    {
        public string xmlDoc;

        public bool Evaluate(ref bool b, XmlDocument xml)
        {
            XmlDocument doc = xml;
            if (xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    PatchManager.errors.Add(GetType().ToString() + "(xmlDoc=" + xmlDoc + "): No document exists with the given name");
                    return false;
                }
                else
                    doc = PatchManager.XmlDocs[xmlDoc];
            }
            try
            {
                return Evaluation(ref b, doc);
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(GetType().ToString() + ": Error");
                return false;
            }
        }

        protected abstract bool Evaluation(ref bool b, XmlDocument xml);
    }
}
