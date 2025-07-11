using System.Xml;

namespace XmlExtensions.Boolean
{
    internal class Comparison : BooleanBase
    {
        protected string value1;
        protected string value2;
        protected string relation = "eq";
        protected string logic = "and";
        protected bool fromXml1 = false;
        protected bool fromXml2 = false;
        protected bool isKey1 = false;
        protected bool isKey2 = false;
        protected string modId = null;
        protected bool nonNumeric = false;

        private protected override void SetException()
        {
            CreateExceptions(value1, "value1", value2, "value2", relation, "relation");
        }

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            string str1 = value1;
            string str2 = value2;
            if (fromXml1)
            {
                XmlNodeList nodes1 = xml.SelectNodes(value1);
                if (nodes1 == null || nodes1.Count == 0)
                {
                    XPathError("value1");
                    return false;
                }
                if (fromXml2)
                {
                    XmlNodeList nodes2 = xml.SelectNodes(value2);
                    if (nodes2 == null || nodes2.Count == 0)
                    {
                        XPathError("value2");
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
                    if (isKey2)
                    {
                        str2 = SettingsManager.GetSetting(modId, value2);
                    }
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
                if (isKey1)
                {
                    str1 = SettingsManager.GetSetting(modId, value1);
                }
                if (fromXml2)
                {
                    XmlNodeList nodes2 = xml.SelectNodes(value2);
                    if (nodes2 == null || nodes2.Count == 0)
                    {
                        XPathError("value2");
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
                    if (isKey2)
                    {
                        str2 = SettingsManager.GetSetting(modId, value2);
                    }
                    b = Helpers.RelationOnString(str1, str2, relation, nonNumeric);
                    return true;
                }
            }
            return true;
        }
    }
}