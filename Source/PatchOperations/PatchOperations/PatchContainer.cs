﻿using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public class PatchContainer
    {
        public List<PatchOperation> patches = new List<PatchOperation>();

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            XmlNodeList nodeList = xmlRoot.ChildNodes;
            foreach(XmlNode node in nodeList)
            {
                patches.Add(Helpers.getPatchFromString(node.OuterXml));
            }
        }
    }
}