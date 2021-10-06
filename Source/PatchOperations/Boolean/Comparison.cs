using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlExtensions.Boolean
{
    public class Comparison : BooleanBase
    {
        protected string value1;
        protected string value2;
        protected string relation = "eq";
        protected string logic = "and";
        protected bool fromXml1 = false;
        protected bool fromXml2 = false;
        protected bool nonNumeric = false;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            string str1 = value1;
            string str2 = value2;
            if (fromXml1)
            {
                XmlNodeList nodes1 = xml.SelectNodes(value1);
                if (nodes1 == null || nodes1.Count == 0)
                {
                    PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value1=" + value1 + "): Failed to find a node with the given xpath");
                    return false;
                }
                if (fromXml2)
                {
                    XmlNodeList nodes2 = xml.SelectNodes(value2);
                    if (nodes2 == null || nodes2.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value2=" + value2 + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    foreach (XmlNode node1 in nodes1)
                    {
                        foreach (XmlNode node2 in nodes2)
                        {
                            str1 = node1.InnerText;
                            str2 = node2.InnerText;
                            if (Helpers.RelationOnString(str1, str2, relation, nonNumeric))
                            {
                                b = true;
                                if (logic == "or")
                                    return true;
                            }
                            else
                            {
                                b = false;
                                if (logic == "and")
                                    return true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (XmlNode node1 in nodes1)
                    {
                        str1 = node1.InnerText;
                        if (Helpers.RelationOnString(str1, str2, relation, nonNumeric))
                        {
                            b = true;
                            if (logic == "or")
                                return true;
                        }
                        else
                        {
                            b = false;
                            if (logic == "and")
                                return true;
                        }
                    }
                }
            }
            else
            {
                if (fromXml2)
                {
                    XmlNodeList nodes2 = xml.SelectNodes(value2);
                    if (nodes2 == null || nodes2.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value2=" + value2 + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    foreach (XmlNode node2 in nodes2)
                    {
                        str2 = node2.InnerText;
                        if (Helpers.RelationOnString(str1, str2, relation, nonNumeric))
                        {
                            b = true;
                            if (logic == "or")
                                return true;
                        }
                        else
                        {
                            b = false;
                            if (logic == "and")
                                return true;
                        }
                    }
                }
                else
                {
                    b = Helpers.RelationOnString(str1, str2, relation, nonNumeric);
                    return true;
                }
            }
            return true;
        }
    }
}
