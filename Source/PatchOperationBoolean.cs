using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Verse;

namespace XmlExtensions.Boolean
{
    public abstract class PatchOperationBoolean : PatchOperationExtendedPathed
    {
        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), this.xpath);
        }

        protected override bool applyWorker(XmlDocument xml)
        {
            PatchManager.errors.Add(this.GetType().ToString() + " was applied like a regular patch operation");
            return false;
        }

        public bool evaluate(ref bool b, XmlDocument xml)
        {
            XmlDocument doc = (xmlDoc == null ? xml : PatchManager.XmlDocs[xmlDoc]);
            if (!this.valid)
            {
                // cache the result
                this.flag = evaluation(ref b, doc);
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

    public class Comparison : PatchOperationBoolean
    {
        protected string value1;
        protected string value2;
        protected string relation = "eq";
        protected string logic = "and";
        protected bool fromXml1 = false;
        protected bool fromXml2 = false;
        protected bool nonNumeric = false;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                string str1 = value1;
                string str2 = value2;
                if (fromXml1)
                {
                    XmlNodeList nodes1 = xml.SelectNodes(value1);
                    if (nodes1 == null || nodes1.Count == 0)
                    {
                        PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value1=" + value1 + "): Failed to find a node with the given xpath");
                        return false;
                    }
                    if (fromXml2)
                    {
                        XmlNodeList nodes2 = xml.SelectNodes(value2);
                        if (nodes2 == null || nodes2.Count == 0)
                        {
                            PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value2=" + value2 + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        foreach (XmlNode node1 in nodes1)
                        {
                            foreach (XmlNode node2 in nodes2)
                            {
                                str1 = node1.InnerText;
                                str2 = node2.InnerText;
                                if (Helpers.relationOnString(str1, str2, relation, nonNumeric))
                                {
                                    b = true;
                                    if (logic == "or")
                                        return true;
                                }
                                else
                                {
                                    b = false;
                                    if (logic == "and")
                                        return true;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (XmlNode node1 in nodes1)
                        {
                            str1 = node1.InnerText;
                            if (Helpers.relationOnString(str1, str2, relation, nonNumeric))
                            {
                                b = true;
                                if (logic == "or")
                                    return true;
                            }
                            else
                            {
                                b = false;
                                if (logic == "and")
                                    return true;
                            }
                        }
                    }
                }
                else
                {
                    if (fromXml2)
                    {
                        XmlNodeList nodes2 = xml.SelectNodes(value2);
                        if (nodes2 == null || nodes2.Count == 0)
                        {
                            PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value2=" + value2 + "): Failed to find a node with the given xpath");
                            return false;
                        }
                        foreach (XmlNode node2 in nodes2)
                        {
                            str2 = node2.InnerText;
                            if (Helpers.relationOnString(str1, str2, relation, nonNumeric))
                            {
                                b = true;
                                if (logic == "or")
                                    return true;
                            }
                            else
                            {
                                b = false;
                                if (logic == "and")
                                    return true;
                            }
                        }                        
                    }
                    else
                    {
                        b =  Helpers.relationOnString(str1, str2, relation, nonNumeric);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Boolean.Comparison(value1=" + value1 + ", value2=" + value2 + "): " + e.Message);
                return false;
            }
        }
    }

    [Obsolete]
    public class Comparision : PatchOperationBoolean
    {
        // I can't spell lol
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

    public class FindMod : PatchOperationBoolean
    {
        public List<string> mods;
        public bool packageId = false;
        public string logic = "or";

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                bool flag = false;
                if (mods == null)
                {
                    PatchManager.errors.Add("XmlExtensions.Boolean.FindMod: <mods> is null");
                    return false;
                }
                if (logic == "or")
                {
                    if (!packageId)
                        flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                    else
                        flag = mods.Any(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
                }
                else
                {
                    if (!packageId)
                        flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.Name == x));
                    else
                        flag = mods.All(x => LoadedModManager.RunningMods.Any(y => y.PackageId.ToLower() == x.ToLower()));
                }
                b = flag;
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Boolean.FindMod: " + e.Message);
                return false;
            }
        }
    }

    public class Conditional : PatchOperationBoolean
    {
        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                XmlNode node = xml.SelectSingleNode(xpath);
                b = (node != null);
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.Conditional(xpath=" + xpath + "): " + e.Message);
                return false;
            }
        }
    }

    public class ConditionalInherited : PatchOperationBoolean
    {
        public string xpathDef;
        public string xpathLocal;

        protected override bool evaluation(ref bool b, XmlDocument xml)
        {
            try
            {
                if (xpathDef == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited: <xpathDef> is null");
                    return false;
                }
                if (xpathLocal == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + "): <xpathLocal> is null");
                    return false;
                }
                XmlNode defNode = xml.SelectSingleNode(xpathDef);
                if (defNode == null)
                {
                    PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + "): Failed to find a node with the given xpath");
                    return false;
                }
                b = findNode(defNode, xpathLocal, xml);
                if (!b)
                {
                    if (xml == PatchManager.XmlDocs["Defs"])
                    {
                        return true;
                    }
                    else
                    {
                        b = findNode(defNode, xpathLocal, PatchManager.XmlDocs["Defs"]);
                        return true;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                PatchManager.errors.Add("XmlExtensions.ConditionalInherited(xpathDef=" + xpathDef + ", xpathLocal=" + xpathLocal + "): " + e.Message);
                return false;
            }
        }

        private bool findNode(XmlNode defNode, string path, XmlDocument xml)
        {
            XmlNode node = defNode.SelectSingleNode(path);
            if (node != null)
            {
                return true;
            }
            else
            {
                XmlAttribute att = defNode.Attributes["ParentName"];
                if (att == null)
                {
                    return false;
                }
                string parent = att.InnerText;
                if (parent == null)
                {
                    return false;
                }
                else
                {
                    return findNode(xml.SelectSingleNode("/Defs/" + defNode.Name + "[@Name=\"" + parent + "\"]"), path, xml);
                }
            }
        }
    }

}
