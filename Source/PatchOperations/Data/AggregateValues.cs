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
        public string root;
        public string docName;

        protected override bool applyWorker(XmlDocument xml)
        {
            try
            {
                XmlDocument xmlDoc = xml;
                if(root!=null)
                {
                    PatchManager.contextPath = root;
                    XmlNode node = xml.SelectSingleNode(root);
                    if (node == null)
                    {
                        PatchManager.errors.Add("XmlExtensions.AggregateValues(root=" + root + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    xmlDoc = new XmlDocument();
                    xmlDoc.AppendChild(xmlDoc.ImportNode(node.Clone(), true));
                }
                else if (docName != null)
                {
                    xmlDoc = PatchManager.XmlDocs[docName];
                }
                List<string> values = new List<string>();
                List<string> vars = new List<string>();
                for (int i = 0; i < valueOperations.node.ChildNodes.Count; i++)
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(Helpers.substituteVariables(valueOperations.node.ChildNodes[i].OuterXml, vars, values, "{}"));
                        XmlNode newNode = doc.DocumentElement;
                        PatchOperationValue patchOperation = DirectXmlToPatch.ObjectFromXml<PatchOperationValue>(newNode, false);
                        string temp = "";
                        if (!patchOperation.GetValue(ref temp, xmlDoc))
                        {
                            PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in getting a value in <valueOperations> at position=" + (i + 1).ToString());
                            return false;
                        }
                        values.Add(temp);
                        if (!patchOperation.getVar(ref temp))
                        {
                            PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in getting a variable name in <valueOperations> at position=" + (i + 1).ToString());
                            return false;
                        }
                        vars.Add(temp);
                    }
                    catch
                    {
                        PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in the valueOperation at position=" + (i + 1).ToString());
                        return false;
                    }
                }
                int errNum = 0;
                try
                {
                    XmlContainer newContainer = new XmlContainer();
                    newContainer = Helpers.substituteVariablesXmlContainer(apply, vars, values, "{}");
                    if (!Helpers.runPatchesInXmlContainer(newContainer, xml, ref errNum))
                    {
                        PatchManager.errors.Add("XmlExtensions.AggregateValues: Error in the operation at position=" + errNum.ToString());
                        return false;
                    }
                    return true;
                }
                catch
                {
                    PatchManager.errors.Add("XmlExtensions.AggregateValues: " + errNum.ToString());
                    return false;
                }
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.AggregateValues: " + e.Message);
                return false;
            }
        }
    }

}
