using System.Collections.Generic;
using System;
using System.Xml;
using Verse;
using System.Linq;

namespace XmlExtensions
{
    public class PatchOperationSortList : PatchOperationExtendedPathed
    {
        public string xpathLocal = "li";
        public bool reverse = false;
        public bool nonNumeric = false;

        protected override bool Patch(XmlDocument xml)
        {
            foreach (XmlNode list in nodes)
            {
                List<XmlNode> nodeList = list.SelectNodes(list.GetXPath() + "/li").Cast<XmlNode>().ToList<XmlNode>();
                if (reverse)
                {
                    if (nonNumeric)
                    {
                        nodeList.Sort(delegate (XmlNode node1, XmlNode node2)
                        {
                            XmlNode node1c = node1.SelectSingleNode(xpathLocal);
                            XmlNode node2c = node2.SelectSingleNode(xpathLocal);
                            if (node1c == null && node2c == null)
                            {
                                return 0;
                            }
                            if (node1c == null)
                            {
                                return -1;
                            }
                            if (node2c == null)
                            {
                                return 1;
                            }
                            string val1 = node1c.InnerText;
                            string val2 = node2c.InnerText;
                            return (val2.CompareTo(val1));
                        });
                    }
                    else
                    {
                        nodeList.Sort(delegate (XmlNode node1, XmlNode node2)
                        {
                            XmlNode node1c = node1.SelectSingleNode(xpathLocal);
                            XmlNode node2c = node2.SelectSingleNode(xpathLocal);
                            if (node1c == null && node2c == null)
                            {
                                return 0;
                            }
                            if (node1c == null)
                            {
                                return -1;
                            }
                            if (node2c == null)
                            {
                                return 1;
                            }
                            float val1 = float.Parse(node1c.InnerText);
                            float val2 = float.Parse(node2c.InnerText);
                            return (val1 > val2 ? -1 : (val1 < val2 ? 1 : 0));
                        });
                    }

                }
                else
                {
                    if (nonNumeric)
                    {
                        nodeList.Sort(delegate (XmlNode node1, XmlNode node2)
                        {
                            XmlNode node1c = node1.SelectSingleNode(xpathLocal);
                            XmlNode node2c = node2.SelectSingleNode(xpathLocal);
                            if (node1c == null && node2c == null)
                            {
                                return 0;
                            }
                            if (node1c == null)
                            {
                                return -1;
                            }
                            if (node2c == null)
                            {
                                return 1;
                            }
                            string val1 = node1c.InnerText;
                            string val2 = node2c.InnerText;
                            return (val1.CompareTo(val2));
                        });
                    }
                    else
                    {
                        nodeList.Sort(delegate (XmlNode node1, XmlNode node2)
                        {
                            XmlNode node1c = node1.SelectSingleNode(xpathLocal);
                            XmlNode node2c = node2.SelectSingleNode(xpathLocal);
                            if (node1c == null && node2c == null)
                            {
                                return 0;
                            }
                            if (node1c == null)
                            {
                                return -1;
                            }
                            if (node2c == null)
                            {
                                return 1;
                            }
                            float val1 = float.Parse(node1c.InnerText);
                            float val2 = float.Parse(node2c.InnerText);
                            return (val1 > val2 ? 1 : (val1 < val2 ? -1 : 0));
                        });
                    }
                }
                foreach (XmlNode child in list.ChildNodes)
                {
                    list.RemoveChild(child);
                }
                foreach (XmlNode node in nodeList)
                {
                    list.AppendChild(node);
                }
            }
            return true;
        }

        protected override void SetException()
        {
            exceptionVals = new string[] { xpath, xpathLocal };
            exceptionFields = new string[] { "xpath", "xpathLocal" };
        }
    }
}