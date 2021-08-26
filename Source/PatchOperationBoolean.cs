using System.Xml;
using Verse;

namespace XmlExtensions.Boolean
{
    public abstract class PatchOperationBoolean : PatchOperationPathed
    {
        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), this.xpath);
        }

        public bool evaluate(XmlDocument xml)
        {
            if (!this.valid)
            {
                this.flag = evaluation(xml);
            }
            return this.flag;
        }

        protected abstract bool evaluation(XmlDocument xml);
        private bool valid = false;
        protected bool flag = false;
    }

    public class And : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            PatchManager.errors.Add(this.GetType().ToString()+" was applied like a regular patch operation");
            return false;
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

        protected override bool evaluation(XmlDocument xml)
        {
            return this.condition1.evaluate(xml) || this.condition2.evaluate(xml);
        }
    }

    public class Xor : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool evaluation(XmlDocument xml)
        {
            return ((this.condition1.evaluate(xml) && !this.condition2.evaluate(xml)) || (!this.condition1.evaluate(xml) && this.condition2.evaluate(xml)));
        }
    }

    public class Not : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition = null;

        protected override bool evaluation(XmlDocument xml)
        {
            return !this.condition.evaluate(xml);
        }
    }

    public class Comparision : PatchOperationBoolean
    {
        protected string value1;
        protected string value2;
        protected string relation = "eq";
        protected string logic = "and";
        protected string fromXml1 = "false";
        protected string fromXml2 = "false";
        protected string nonNumeric = "false";      

        protected override bool evaluation(XmlDocument xml)
        {
            bool isOr = true;
            if (this.logic == "and")
            {
                isOr = false;
            }
            flag = !isOr;
            if (bool.Parse(this.nonNumeric))
            {
                string val2 = "";
                if (fromXml2 == "true")
                {
                    val2 = xml.SelectSingleNode(value2).InnerText;
                }
                else
                {
                    val2 = value2;
                }

                if (bool.Parse(fromXml1))
                {
                    foreach (object obj in xml.SelectNodes(this.value1))
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
                    int compare = value1.CompareTo(val2);
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
                if (fromXml2 == "true")
                {
                    val2 = float.Parse(xml.SelectSingleNode(value2).InnerText);
                }
                else
                {
                    val2 = float.Parse(value2);
                }
                if (bool.Parse(fromXml1))
                {
                    foreach (object obj in xml.SelectNodes(this.value1))
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
                else
                {
                    float xval = float.Parse(value1);
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
