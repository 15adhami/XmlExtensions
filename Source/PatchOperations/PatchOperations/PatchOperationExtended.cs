using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public abstract class PatchOperationExtended : PatchOperation
    {
        public string xmlDoc;

        protected string[] exceptionVals;
        protected string[] exceptionFields;

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
            try
            {
                SetException();
                return Patch(doc);
            }
            catch (Exception e)
            {
                ExceptionError(e, exceptionVals, exceptionFields);
                return false;
            }
        }

        protected virtual bool Patch(XmlDocument xml)
        {
            return false;
        }

        protected abstract void SetException();

        protected void XPathError(string xpath, string field)
        {
            PatchManager.errors.Add(this.GetType().ToString() + "(" + field + "=" + xpath + "): Failed to find a node with the given xpath");
        }

        protected void ExceptionError(Exception e, string[] values, string[] fields)
        {
            string str = this.GetType().ToString();
            if (values != null)
            {
                str += "(" + fields[0] + "=" + values[0];
                for (int i = 1; i < values.Length; i++)
                {
                    str += ", " + fields[i] + "=" + values[i];
                }
                str += ")";
            }
            str += ": " + e.Message;
            PatchManager.errors.Add(str);
        }
    }
}