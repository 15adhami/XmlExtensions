using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal abstract class PatchOperationValue : PatchOperationExtended
    {
        public XmlContainer apply;
        public string storeIn;
        public string brackets = "{}";

        public bool GetValue(List<string> vals, XmlDocument xml)
        {
            XmlDocument doc = xml;
            if (xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    Error(new string[] { xmlDoc }, new string[] { "xmlDoc" }, "No document exists with the given name");
                    return false;
                }
                else
                    doc = PatchManager.XmlDocs[xmlDoc];
            }
            try
            {
                if (!PreCheck(doc)) { return false; }
                return getValues(vals, doc);
            }
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }
        }

        protected override bool Patch(XmlDocument xml)
        {
            List<string> vals = new List<string>();
            List<string> vars = new List<string>();
            if (!getVars(vars)) { return false; }
            if (!getValues(vals, xml)) { return false; }
            XmlContainer newContainer = Helpers.SubstituteVariablesXmlContainer(apply, vars, vals, brackets);
            return RunPatches(newContainer, xml);
        }

        public virtual bool getValues(List<string> vals, XmlDocument xml)
        {
            return true;
        }

        public virtual bool getVars(List<string> vars)
        {
            vars.Add(storeIn);
            return true;
        }
    }
}