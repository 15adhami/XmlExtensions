using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal static class Helpers
    {
        /// <summary>
        /// Calculates the prefix of the given length.
        /// </summary>
        /// <param name="path">The xpath you want to get the prefix of.</param>
        /// <param name="length">The number of nodes that appear in the xpath.</param>
        /// <returns>The prefix.</returns>
        public static string GetPrefix(string path, int length = 2)
        {
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

        /// <summary>
        /// Substitutes a variable with its value in a given XmlContainer.
        /// </summary>
        /// <param name="container">The XmlContainer that you want to edit.</param>
        /// <param name="var">The name of the variable.</param>
        /// <param name="val">The value of the variable.</param>
        /// <param name="brackets">The left and right brackets that surround the variable.</param>
        /// <returns>The new XmlContainer after the substitution.</returns>
        public static XmlContainer SubstituteVariableXmlContainer(XmlContainer container, string var, string val, string brackets)
        {
            string oldXml = container.node.OuterXml;
            string newXml = oldXml.SubstituteVariable(var, val, brackets); ;
            return new XmlContainer() { node = GetNodeFromString(newXml) };
        }

        /// <summary>
        /// Substitutes a list of variables with their corresponding values in a given string.
        /// </summary>
        /// <param name="container">The string that you want to edit.</param>
        /// <param name="var">The list of variable names.</param>
        /// <param name="val">The list of values for the variables.</param>
        /// <param name="brackets">The left and right brackets that surround the variables.</param>
        /// <returns>The new string after the substitution.</returns>
        public static XmlContainer SubstituteVariablesXmlContainer(XmlContainer container, List<string> var, List<string> val, string brackets = "{}")
        {
            string oldXml = container.node.OuterXml;
            string newXml;
            newXml = oldXml.SubstituteVariables(var, val, brackets);
            return new XmlContainer() { node = Helpers.GetNodeFromString(newXml) };
        }
        
        /// <summary>
        /// Creates a PatchOperation from its OuterXml.
        /// </summary>
        /// <param name="OuterXml">The OuterXml of the PatchOperation.</param>
        /// <returns>A PatchOperation from the given OuterXml.</returns>
        public static PatchOperation GetPatchFromString(string OuterXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(OuterXml);
            XmlNode newNode = doc.DocumentElement;
            //return CustomXmlLoader.ObjectFromXml<PatchOperation>(newNode, false);
            return DirectXmlToObject.ObjectFromXml<PatchOperation>(newNode, false);
        }

        public static XmlNode GetNodeFromString(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNode newNode = doc.DocumentElement;
            return newNode;
        }

        /// <summary>
        /// Performs a mathematical on two strings.
        /// </summary>
        /// <param name="str1">The first string.</param>
        /// <param name="str2">The second string.</param>
        /// <param name="operation">The operation to be performed.</param>
        /// <returns>The resulting string.</returns>
        public static string OperationOnString(string str1, string str2, string operation)
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
                if (IsCData(str1) && IsCData(str2))
                    result = WrapCData(UnwrapCData(str1) + UnwrapCData(str2));
                else if (IsCData(str1))
                    result = WrapCData(UnwrapCData(str1) + str2);
                else if (IsCData(str2))
                    result = WrapCData(str1 + UnwrapCData(str2));
                else
                    result = str1 + str2;
            }
            else if (operation == "concat2")
            {
                if (IsCData(str1) && IsCData(str2))
                    result = WrapCData(UnwrapCData(str2) + UnwrapCData(str1));
                else if (IsCData(str2))
                    result = WrapCData(UnwrapCData(str2) + str1);
                else if (IsCData(str1))
                    result = WrapCData(str2 + UnwrapCData(str1));
                else
                    result = str2 + str1;
            }
            else if (operation == "roundup")
            {
                float xval1 = float.Parse(str1);
                if (str2 == "")
                {
                    str2 = "1";
                }
                float xval2 = float.Parse(str2);
                result = (Math.Ceiling(xval1/xval2)*xval2).ToString();
            }
            else if (operation == "rounddown")
            {
                float xval1 = float.Parse(str1);
                if (str2 == "")
                {
                    str2 = "1";
                }
                float xval2 = float.Parse(str2);
                result = (Math.Floor(xval1 / xval2) * xval2).ToString();
            }
            else if (operation == "roundnearest")
            {
                float xval1 = float.Parse(str1);
                if (str2 == "")
                {
                    str2 = "1";
                }
                float xval2 = float.Parse(str2);
                result = (Math.Round(xval1 / xval2) * xval2).ToString();
            }
            return result;
        }

        /// <summary>
        /// Computes a binary relation between two strings.
        /// </summary>
        /// <param name="str1">The first string.</param>
        /// <param name="str2">The second string.</param>
        /// <param name="relation">The binary relation.</param>
        /// <param name="nonNumeric">If true, then the inputs will be interpreted as strings and dictionary order is used instead.</param>
        /// <returns>Whether or not the relations holds.</returns>
        public static bool RelationOnString(string str1, string str2, string relation, bool nonNumeric)
        {
            if (nonNumeric)
            {
                int compare = str1.ToLower().CompareTo(str2.ToLower());
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

        public static bool RunPatchesInPatchContainer(PatchContainer container, XmlDocument xml, ref int errNum)
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
                ErrorManager.AddError(e.Message);
                return false;
            }
        }

        public static XmlNodeList SelectNodes(string path, XmlDocument xml, PatchOperation operation)
        {
            XmlNodeList list = xml.SelectNodes(path);
            return list;
        }

        public static XmlNode SelectSingleNode(string path, XmlDocument xml, PatchOperation operation)
        {
            XmlNode node = xml.SelectSingleNode(path);
            return node;
        }

        private static bool IsCData(string s) => s != null && s.StartsWith("<![CDATA[") && s.EndsWith("]]>");

        private static string UnwrapCData(string s) => s.Substring(9, s.Length - 12);

        private static string WrapCData(string s) => $"<![CDATA[{s}]]>";
    }
}