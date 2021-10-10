using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public abstract class BooleanBase : ErrorHandler
    {
        public string xmlDoc;

        public bool Evaluate(ref bool b, XmlDocument xml)
        {
            XmlDocument doc = xml;
            if (xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    Error(new string[] { xmlDoc }, new string[] { "xmlDoc" }, "(xmlDoc=" + xmlDoc + "): No document exists with the given name");
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
                Error(e.Message);
                return false;
            }
        }

        protected void EvaluationError(string name)
        {
            Error("Failed to evaluate <" + name + ">");
        }

        protected void EvaluationError(int errNum)
        {
            Error("Failed to evaluate the condition at position=" + errNum.ToString());
        }

        protected abstract bool Evaluation(ref bool b, XmlDocument xml);
    }
}