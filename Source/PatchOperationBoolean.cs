using System;
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

        protected override bool ApplyWorker(XmlDocument xml)
        {
            PatchManager.errors.Add(this.GetType().ToString() + " was applied like a regular patch operation");
            return false;
        }

        public bool evaluate(ref bool b, XmlDocument xml)
        {
            if (!this.valid)
            {
                this.flag = evaluation(ref b, xml);
            }
            return this.flag;
        }

        protected abstract bool evaluation(ref bool b, XmlDocument xml);
        private bool valid = false;
        protected bool flag = false;
    }

    public class And : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        
        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition1.evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition1>");
                    return false;
                }
                bool b2 = false;
                if (!condition2.evaluate(ref b2, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition2>");
                    return false;
                }
                b = b1 && b2;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }

    public class Or : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition1.evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition1>");
                    return false;
                }
                bool b2 = false;
                if (!condition2.evaluate(ref b2, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition2>");
                    return false;
                }
                b = b1 || b2;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }

    public class Xor : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition1 = null;
        protected PatchOperationBoolean condition2 = null;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition1.evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition1>");
                    return false;
                }
                bool b2 = false;
                if (!condition2.evaluate(ref b2, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition2>");
                    return false;
                }
                b = (b1 && !b2) || (!b1 && b2);
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
        }
    }

    public class Not : PatchOperationBoolean
    {
        protected PatchOperationBoolean condition = null;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool b1 = false;
                if (!condition.evaluate(ref b1, xml))
                {
                    PatchManager.errors.Add(this.GetType().ToString() + ": Failed to evaluate <condition>");
                    return false;
                }
                b = !b1;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(this.GetType().ToString() + ": " + e.Message);
                return false;
            }
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

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
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
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare == 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "sl")
                            {
                                if (isOr)
                                {
                                    if (compare < 0)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare < 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "leq")
                            {
                                if (isOr)
                                {
                                    if (compare <= 0)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare <= 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "sg")
                            {
                                if (isOr)
                                {
                                    if (compare > 0)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare > 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "geq")
                            {
                                if (isOr)
                                {
                                    if (compare >= 0)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare >= 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "neq")
                            {
                                if (isOr)
                                {
                                    if (compare != 0)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (compare != 0)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                b = false;
                                return true;
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
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare == 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "sl")
                        {
                            if (isOr)
                            {
                                if (compare < 0)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare < 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "leq")
                        {
                            if (isOr)
                            {
                                if (compare <= 0)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare <= 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "sg")
                        {
                            if (isOr)
                            {
                                if (compare > 0)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare > 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "geq")
                        {
                            if (isOr)
                            {
                                if (compare >= 0)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare >= 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "neq")
                        {
                            if (isOr)
                            {
                                if (compare != 0)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (compare != 0)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            b = false;
                            return true;
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
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval == val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "sl")
                            {
                                if (isOr)
                                {
                                    if (xval < val2)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval < val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "leq")
                            {
                                if (isOr)
                                {
                                    if (xval <= val2)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval <= val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "sg")
                            {
                                if (isOr)
                                {
                                    if (xval > val2)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval > val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "geq")
                            {
                                if (isOr)
                                {
                                    if (xval >= val2)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval >= val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else if (relation == "neq")
                            {
                                if (isOr)
                                {
                                    if (xval != val2)
                                    {
                                        b = true;
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (xval != val2)
                                    {
                                    }
                                    else
                                    {
                                        b = false;
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                b = false;
                                return true;
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
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval == val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "sl")
                        {
                            if (isOr)
                            {
                                if (xval < val2)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval < val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "leq")
                        {
                            if (isOr)
                            {
                                if (xval <= val2)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval <= val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "sg")
                        {
                            if (isOr)
                            {
                                if (xval > val2)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval > val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "geq")
                        {
                            if (isOr)
                            {
                                if (xval >= val2)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval >= val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else if (relation == "neq")
                        {
                            if (isOr)
                            {
                                if (xval != val2)
                                {
                                    b = true;
                                    return true;
                                }
                            }
                            else
                            {
                                if (xval != val2)
                                {
                                }
                                else
                                {
                                    b = false;
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            b = false;
                            return true;
                        }


                    }
                }
                b = flag;
                return true;
            }
            catch(Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Boolean.Comparision(value1=" + value1 + ", value2=" + value2 + "): " + e.Message);
                return false;
            }
        }           
    }
}
