using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class ForLoop : PatchOperation
    {
        protected XmlContainer apply;
        protected string storeIn = "i";
        protected string brackets = "{}";
        protected int from = 0;
        protected int to = 1;
        protected int increment = 1;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string oldXml = this.apply.node.OuterXml;
            if (this.increment > 0)
            {
                for (int i = this.from; i < this.to; i += increment)
                {
                    string newXml = Helpers.substituteVariable(oldXml, this.storeIn, i.ToString(), this.brackets);
                    XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                    for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                        patch.Apply(xml);
                    }
                }
            }
            else if (this.increment < 0)
            {
                for (int i = this.from - 1; i >= this.to; i -= increment)
                {
                    string newXml = Helpers.substituteVariable(oldXml, this.storeIn, i.ToString(), this.brackets);
                    XmlContainer newContainer = new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
                    for (int j = 0; j < this.apply.node.ChildNodes.Count; j++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[j].OuterXml);
                        patch.Apply(xml);
                    }
                }
            }       
            return true;
        }
    }

    public class ForEach : PatchOperationPathed
    {
        protected XmlContainer apply;
        protected string storeIn = "DEF";
        protected string brackets = "{}";
        protected int prefixLength = 2;
        protected override bool ApplyWorker(XmlDocument xml)
        {

            foreach (object obj in xml.SelectNodes(this.xpath))
            {
                //Calculate prefix for variable
                XmlNode xmlNode = obj as XmlNode;
                string path = xmlNode.GetXPath();
                string prefix = Helpers.getPrefix(path, prefixLength);
                string temp = Helpers.substituteVariable(this.apply.node.OuterXml, storeIn, prefix, brackets);
                XmlContainer newContainer = new XmlContainer() { node =  Helpers.getNodeFromString(temp)};
                for (int i = 0; i < this.apply.node.ChildNodes.Count; i++)
                {
                    PatchOperation patch = Helpers.getPatchFromString(newContainer.node.ChildNodes[i].OuterXml);
                    patch.Apply(xml);
                }
            }
            return true;
        }

    }
  
    public class IfStatement : PatchOperationPathed
    {
        protected PatchOperationBoolean condition = null;
        protected XmlContainer caseTrue =  null;
        protected XmlContainer caseFalse = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (this.condition.evaluate(xml))
            {
                if (this.caseTrue != null)
                {
                    for (int i = 0; i < this.caseTrue.node.ChildNodes.Count; i++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(this.caseTrue.node.ChildNodes[i].OuterXml);
                        patch.Apply(xml);
                    }
                    return true;
                }
            }
            else
            {
                if (this.caseFalse != null)
                {
                    for (int i = 0; i < this.caseFalse.node.ChildNodes.Count; i++)
                    {
                        PatchOperation patch = Helpers.getPatchFromString(this.caseFalse.node.ChildNodes[i].OuterXml);
                        patch.Apply(xml);
                    }
                    return true;
                }
            }
                return false;
        }

    }

    public class And : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            return true;
        }
        protected override bool evaluation(XmlDocument xml)
        {
            return this.condition1.evaluate(xml) && this.condition2.evaluate(xml);
        }
    }

    public class Or : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            return true;
        }
        protected override bool evaluation(XmlDocument xml)
        {
            return this.condition1.evaluate(xml) || this.condition2.evaluate(xml);
        }
    }

    public class Not : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            return true;
        }
        protected override bool evaluation(XmlDocument xml)
        {
            return !this.condition.evaluate(xml);
        }
    }

    public class Comparision : PatchOperationBoolean
    {
        protected string value = "0";
        protected string relation = "=";
        protected string logic = "or";
        protected string fromXml = "false";
        protected string isFloat = "false";

        protected override bool ApplyWorker(XmlDocument xml)
        {
            return true;
        }

        protected override bool evaluation(XmlDocument xml)
        {
            bool isOr = true;
            if (this.logic == "and")
            {
                isOr = false;
            }
            flag = !isOr;
            if (!bool.Parse(this.isFloat))
            {
                string val2 = "";
                if (fromXml == "true")
                {
                    val2 = xml.SelectSingleNode(value).InnerText;
                }
                else
                {
                    val2 = value;
                }

                foreach (object obj in xml.SelectNodes(this.xpath))
                {
                    XmlNode xmlNode = obj as XmlNode;
                    string xval = xmlNode.InnerText;
                    int compare = xval.CompareTo(val2);
                    if (relation == "eq")
                    {
                        if (isOr)
                        {
                            if (compare == 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare == 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "sl")
                    {
                        if (isOr)
                        {
                            if (compare < 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare < 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "leq")
                    {
                        if (isOr)
                        {
                            if (compare <= 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare <= 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "sg")
                    {
                        if (isOr)
                        {
                            if (compare > 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare > 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "geq")
                    {
                        if (isOr)
                        {
                            if (compare >= 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare >= 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "neq")
                    {
                        if (isOr)
                        {
                            if (compare != 0)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (compare != 0)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                float val2 = 0;
                if (fromXml == "true")
                {
                    val2 = float.Parse(xml.SelectSingleNode(value).InnerText);
                }
                else
                {
                    val2 = float.Parse(value);
                }
                foreach (object obj in xml.SelectNodes(this.xpath))
                {
                    XmlNode xmlNode = obj as XmlNode;
                    float xval = float.Parse(xmlNode.InnerText);
                    if (relation == "eq")
                    {
                        if (isOr)
                        {
                            if (xval == val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval == val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "sl")
                    {
                        if (isOr)
                        {
                            if (xval < val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval < val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "leq")
                    {
                        if (isOr)
                        {
                            if (xval <= val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval <= val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "sg")
                    {
                        if (isOr)
                        {
                            if (xval > val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval > val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "geq")
                    {
                        if (isOr)
                        {
                            if (xval >= val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval >= val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else if (relation == "neq")
                    {
                        if (isOr)
                        {
                            if (xval != val2)
                            {
                                flag = true;
                                return flag;
                            }
                        }
                        else
                        {
                            if (xval != val2)
                            {
                            }
                            else
                            {
                                flag = false;
                                return flag;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

                return flag;
        }
        }

    
}

