using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class AggregateValues : PatchOperationExtended
    {
        public XmlContainer valueOperations;
        public XmlContainer apply;

        protected override bool Patch(XmlDocument xml)
        {
            List<string> values = new List<string>();
            List<string> vars = new List<string>();
            for (int i = 0; i < valueOperations.node.ChildNodes.Count; i++)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Helpers.SubstituteVariables(valueOperations.node.ChildNodes[i].OuterXml, vars, values, "{}"));
                XmlNode newNode = doc.DocumentElement;
                PatchOperationValue patchOperation = DirectXmlToObject.ObjectFromXml<PatchOperationValue>(newNode, false);
                if (!patchOperation.GetValue(values, xml))
                {
                    Error("Error in getting a value in <valueOperations> at position=" + (i + 1).ToString());
                    return false;
                }
                if (!patchOperation.getVars(vars))
                {
                    Error("Error in getting a variable name in <valueOperations> at position=" + (i + 1).ToString());
                    return false;
                }
            }
            XmlContainer newContainer = Helpers.SubstituteVariablesXmlContainer(apply, vars, values, "{}");
            return RunPatches(newContainer, xml);
        }
    }

}
