using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Verse;


namespace XmlExtensions
{
    public static class Helpers
    {
        public static string getPrefix(string str, int length)
        {
            string path = str;
            if (path[0] == '/')
            {
                path = path.Substring(1);
            }

            string[] parts = path.Split('/');
            string ans = "";
            for (int i = 0; i < length; i++)
            {
                ans += "/";
                ans += parts[i];
            }
            ans = ans.Substring(1);
            return ans;
        }

        public static string substituteVariable(string str, string var, string val, string brackets)
        {
            string variable = brackets[0]+var+ brackets[1];
            return str.Replace(variable, val);
        }

        public static string substituteVariables(string str, List<string> vars, List<string> vals, string brackets)
        {            
            int i = 0;
            StringBuilder builder = new StringBuilder(str);
            foreach(string var in vars)
            {
                builder.Replace(brackets[0] + var + brackets[1], vals[i]);
                i++;
            }
            return builder.ToString();
        }

        public static XmlContainer substituteVariableXmlContainer(XmlContainer container, string var, string val, string brackets)
        {
            string oldXml = container.node.OuterXml;
            string newXml;
            newXml = Helpers.substituteVariable(oldXml, var, val, brackets);
            return new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
        }

        public static XmlContainer substituteVariablesXmlContainer(XmlContainer container, List<string> var, List<string> val, string brackets)
        {
            string oldXml = container.node.OuterXml;
            string newXml;
            newXml = Helpers.substituteVariables(oldXml, var, val, brackets);
            return new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
        }

        public static PatchOperation getPatchFromString(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNode newNode = doc.DocumentElement;
            return DirectXmlToObject.ObjectFromXml<PatchOperation>(newNode, false);
            /*
            XmlAttribute att = newNode.Attributes["Class"];
            if (att != null && att.InnerText == "XmlExtensions.AggregateValues")
            {
                AggregateValues patch =  (AggregateValues)XmlMod.createPatch.Method.Invoke(null, new Object[] { null});
                if(newNode["apply"]!=null)
                {
                    patch.apply = new XmlContainer() { node = newNode["apply"] };
                }
                if (newNode["valueOperations"] != null)
                {
                    patch.valueOperations = new XmlContainer() { node = newNode["valueOperations"] };
                }
                if (newNode["root"] != null)
                {
                    patch.root = newNode["root"].InnerXml;
                }
                return patch;
            }
            else
            {
                return DirectXmlToObject.ObjectFromXml<PatchOperation>(newNode, false);
            }      
            */
        }

        public static XmlNode getNodeFromString(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNode newNode = doc.DocumentElement;
            return newNode;
        }

        public static string operationOnString(string str1, string str2, string operation)
        {
            string result = "";
            if (operation == "negate")
            {
                if (str1 == "true")
                {
                    result = "false";                    
                }
                else
                {
                    result = "true";
                }
            }
            else if (operation == "+")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval1 + xval2).ToString();
            }
            else if (operation == "*")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval1 * xval2).ToString();
            }
            else if (operation == "/")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval1 / xval2).ToString();
            }
            else if (operation == "/2")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval2 / xval1).ToString();
            }
            else if (operation == "-")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval1 - xval2).ToString();
            }
            else if (operation == "-2")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval2 - xval1).ToString();
            }
            else if (operation == "%")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval1 % xval2).ToString();
            }
            else if (operation == "%2")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = (xval2 % xval1).ToString();
            }
            else if (operation == "^")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Pow(xval1, xval2).ToString();
            }
            else if (operation == "^2")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Pow(xval2, xval1).ToString();
            }
            else if (operation == "log")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Log(xval1, xval2).ToString();
            }
            else if (operation == "log2")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Log(xval2, xval1).ToString();
            }
            else if (operation == "min")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Min(xval1, xval2).ToString();
            }
            else if (operation == "max")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = Math.Max(xval1, xval2).ToString();
            }
            else if (operation == "average")
            {
                float xval1 = float.Parse(str1);
                float xval2 = float.Parse(str2);
                result = ((xval1 + xval2) / 2).ToString();
            }
            else if (operation == "concat")
            {
                result = str1 + str2;
            }
            else if (operation == "concat2")
            {
                result = str2 + str1;
            }
            return result;
        }

        public static bool relationOnString(string str1, string str2, string relation, bool nonNumeric)
        {
            if(nonNumeric)
            {
                int compare = str1.CompareTo(str2);
                if (relation == "eq")
                {
                    if (compare == 0)
                        return true;
                    else
                        return false;
                }
                else if (relation == "sl")
                {
                    if (compare < 0)
                        return true;
                    else
                        return false;
                }
                else if (relation == "leq")
                {
                    if (compare <= 0)
                        return true;
                    else
                        return false;
                }
                else if (relation == "sg")
                {
                    if (compare > 0)
                        return true;
                    else
                        return false;
                }
                else if (relation == "geq")
                {
                    if (compare >= 0)
                        return true;
                    else
                        return false;
                }
                else if (relation == "neq")
                {
                    if (compare != 0)
                        return true;
                    else
                        return false;
                }
                else
                    return compare == 0;
            }
            else
            {
                float val1 = float.Parse(str1);
                float val2 = float.Parse(str2);
                if (relation == "eq")
                    return val1 == val2;
                else if (relation == "sl")
                    return val1 < val2;
                else if (relation == "leq")
                    return val1 <= val2;
                else if (relation == "sg")
                    return val1 > val2;
                else if (relation == "geq")
                    return val1 >= val2;
                else
                    return val1 == val2;
            }
        }

        public static bool runPatchesInXmlContainer(XmlContainer container, XmlDocument xml, ref int errNum)
        {
            try
            {
                for (int j = 0; j < container.node.ChildNodes.Count; j++)
                {
                    PatchOperation patch = new PatchOperation();
                    try
                    {
                        patch = getPatchFromString(container.node.ChildNodes[j].OuterXml);
                    }
                    catch(Exception e)
                    {
                        PatchManager.errors.Add("Could not create patch from:\n" + container.node.ChildNodes[j].OuterXml+"\n"+e.Message);
                        errNum = j + 1;
                        return false;
                    }
                    if (!patch.Apply(xml))
                    {
                        errNum = j + 1;
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool runPatchesInPatchContainer(PatchContainer container, XmlDocument xml, ref int errNum)
        {
            try
            {
                for (int j = 0; j < container.patches.Count; j++)
                {
                    if (!container.patches[j].Apply(xml))
                    {
                        errNum = j + 1;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add(e.Message);
                return false;
            }
        }

        public static bool containsNode(XmlNode node, string nodeName)
        {
            foreach(XmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == nodeName) { return true; }
            }
            return false;
        }

        public static string tryTranslate(string str, string tKey)
        {            
            if (tKey != null)
            {
                TaggedString temp = new TaggedString();
                if (tKey.TryTranslate(out temp))
                    return temp.RawText;
                else
                    return str;
            }
            else
            {
                return str;
            }
        }

    }

    
    
    
}
