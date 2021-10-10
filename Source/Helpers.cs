using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public static class Helpers
    {
        /// <summary>
        /// Calculates the prefix the given length.
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
        /// Substitutes a variable with its value in a given a string.
        /// </summary>
        /// <param name="str">The string that you want to edit.</param>
        /// <param name="var">The name of the variable.</param>
        /// <param name="val">The value of the variable.</param>
        /// <param name="brackets">The left and right brackets that surround the variable.</param>
        /// <returns>The new string after the substitution.</returns>
        public static string SubstituteVariable(string str, string var, string val, string brackets = "{}")
        {
            string variable = brackets[0] + var + brackets[1];
            return str.Replace(variable, val);
        }

        /// <summary>
        /// Substitutes a list of variables with their corresponding values in a given string.
        /// </summary>
        /// <param name="str">The string that you want to edit.</param>
        /// <param name="vars">The list of variable names.</param>
        /// <param name="vals">The list of values for the variables.</param>
        /// <param name="brackets">The left and right brackets that surround the variables.</param>
        /// <returns>The new string after the substitution.</returns>
        public static string SubstituteVariables(string str, List<string> vars, List<string> vals, string brackets)
        {
            int i = 0;
            StringBuilder builder = new StringBuilder(str);
            foreach (string var in vars)
            {
                builder.Replace(brackets[0] + var + brackets[1], vals[i]);
                i++;
            }
            return builder.ToString();
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
            string newXml;
            newXml = Helpers.SubstituteVariable(oldXml, var, val, brackets);
            return new XmlContainer() { node = Helpers.GetNodeFromString(newXml) };
        }

        /// <summary>
        /// Substitutes a list of variables with their corresponding values in a given string.
        /// </summary>
        /// <param name="container">The string that you want to edit.</param>
        /// <param name="var">The list of variable names.</param>
        /// <param name="val">The list of values for the variables.</param>
        /// <param name="brackets">The left and right brackets that surround the variables.</param>
        /// <returns>The new string after the substitution.</returns>
        public static XmlContainer SubstituteVariablesXmlContainer(XmlContainer container, List<string> var, List<string> val, string brackets)
        {
            string oldXml = container.node.OuterXml;
            string newXml;
            newXml = Helpers.SubstituteVariables(oldXml, var, val, brackets);
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
                result = str1 + str2;
            }
            else if (operation == "concat2")
            {
                result = str2 + str1;
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
                ErrorManager.Add(e.Message);
                return false;
            }
        }

        public static string TryTranslate(string str, string tKey)
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