using System;
using System.Xml;

namespace XmlExtensions.Boolean
{
    [Obsolete]
    public class Comparision : BooleanBase
    {
        // I can't spell lol
        protected string value1;
        protected string value2;
        protected string relation = "eq";
        protected string logic = "and";
        protected string fromXml1 = "false";
        protected string fromXml2 = "false";
        protected string nonNumeric = "false";      

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool isOr = true;
                if (logic == "and")
                {
                    isOr = false;
                }
                bool flag = !isOr;
                if (bool.Parse(nonNumeric))
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
                        foreach (object obj in xml.SelectNodes(value1))
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
                        foreach (object obj in xml.SelectNodes(value1))
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
