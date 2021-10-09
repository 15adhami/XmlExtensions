﻿using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class CumulativeMath : PatchOperationValue
    {
        protected string operation;
        protected string xpath;

        protected override void SetException()
        {
            exceptionVals = new string[] { storeIn, operation, xpath };
            exceptionFields = new string[] { "storeIn", "operation", "xpath" };
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            string value = "";
            float sum = 0;
            XmlNodeList XmlList;
            XmlList = xml.SelectNodes(xpath);
            if (XmlList == null || XmlList.Count == 0)
            {
                XPathError();
                return false;
            }
            int n = XmlList.Count;
            if (operation != "count")
            {
                float m;
                try
                {
                    m = float.Parse(XmlList[0].InnerText);
                }
                catch
                {
                    Error("Failed to get a valid value from the node:" + XmlList[0].OuterXml);
                    return false;
                }
                foreach (object obj in XmlList)
                {
                    XmlNode xmlNode = obj as XmlNode;
                    float val;
                    try
                    {
                        val = float.Parse(xmlNode.InnerText);
                    }
                    catch
                    {
                        Error("Failed to get a valid value from the node:" + XmlList[0].OuterXml);
                        return false;
                    }
                    if (operation == "+")
                    {
                        sum += val;
                    }
                    else if (operation == "-")
                    {
                        sum -= val;
                    }
                    else if (operation == "*")
                    {
                        sum *= val;
                    }
                    else if (operation == "average")
                    {
                        sum += val / n;
                    }
                    else if (operation == "min")
                    {
                        m = Math.Min(m, val);
                    }
                    else if (operation == "max")
                    {
                        m = Math.Max(m, val);
                    }
                }
                if (operation == "min" || operation == "max")
                {
                    sum = m;
                }
            }
            else
            {
                sum = n;
            }
            if (operation != "concat")
            {
                value = sum.ToString();
            }
            vals.Add(value);
            return true;
        }
    }

}