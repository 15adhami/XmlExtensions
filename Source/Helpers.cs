using System;
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

        public static XmlContainer substituteVariableXmlContainer(XmlContainer container, string var, string val, string brackets)
        {
            string oldXml = container.node.OuterXml;
            string newXml;
            newXml = Helpers.substituteVariable(oldXml, var, val, brackets);
            return new XmlContainer() { node = Helpers.getNodeFromString(newXml) };
        }

        public static PatchOperation getPatchFromString(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNode newNode = doc.DocumentElement;
            PatchOperation patchOperation = DirectXmlToObject.ObjectFromXml<PatchOperation>(newNode, false);
            return patchOperation;
        }

        public static XmlNode getNodeFromString(string str)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(str);
            XmlNode newNode = doc.DocumentElement;
            return newNode;
        }

        public static string fixXml(string str)
        {
            string ans = str.Replace("(", "<");
            return ans.Replace(")", ">");
        }

        public static string operationOnString(string str1, string str2, string operation)
        {
            string result = "";

            if (operation == "negate")
            {
                if(str1 == "true")
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

        public static string relationOnString(string str1, string str2, string operation)
        {
            string ans = "";
            return ans;
        }

        public static void runPatchesInXmlContainer(XmlContainer container, XmlDocument xml)
        {
            for (int j = 0; j < container.node.ChildNodes.Count; j++)
            {
                PatchOperation patch = Helpers.getPatchFromString(container.node.ChildNodes[j].OuterXml);
                patch.Apply(xml);
            }
        }

    }
    
    
}
