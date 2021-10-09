﻿using System;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    public abstract class PatchOperationExtended : PatchOperation
    {
        public string xmlDoc;

        protected string[] exceptionVals;
        protected string[] exceptionFields;

        protected sealed override bool ApplyWorker(XmlDocument xml)
        {
            XmlDocument doc = xml;            
            if (xmlDoc != null)
            {
                if (!PatchManager.XmlDocs.ContainsKey(xmlDoc))
                {
                    Error("No XML document exists with docName=\"" + xmlDoc + "\"");
                    return false;
                }
                else
                {
                    doc = PatchManager.XmlDocs[xmlDoc];
                }
            }            
            try
            {
                if (!PreCheck(doc)) { return false; }
                return Patch(doc);
            }
            catch (Exception e)
            {                
                Error(e.Message);
                return false;
            }
        }

        protected virtual bool PreCheck(XmlDocument xml)
        {
            return true;
        }

        protected virtual bool Patch(XmlDocument xml)
        {
            return false;
        }

        protected virtual void SetException() { }

        /// <summary>
        /// Throws an error for the patch operation
        /// </summary>
        /// <param name="msg">The message to be displayed</param>
        protected void Error(string msg)
        {
            SetException();
            string str = GetType().ToString();
            if (exceptionVals != null)
            {
                str += "(" + exceptionFields[0] + "=" + exceptionVals[0];
                for (int i = 1; i < exceptionVals.Length; i++)
                {
                    str += ", " + exceptionFields[i] + "=" + exceptionVals[i];
                }
                str += ")";
            }
            PatchManager.errors.Add(str + ": " + msg);
        }

        protected void Error(string[] vals, string[] fields, string msg)
        {
            string str = GetType().ToString();
            if (vals != null)
            {
                str += "(" + fields[0] + "=" + vals[0];
                for (int i = 1; i < vals.Length; i++)
                {
                    str += ", " + fields[i] + "=" + vals[i];
                }
                str += ")";
            }
            PatchManager.errors.Add(str + ": " + msg);
        }

        protected void XPathError(string node = "xpath")
        {
            Error("Failed to find a node referenced by <" + node + ">");
        }

        protected void NullError(string node)
        {
            Error("<" + node + "> is null");
        }

        protected bool RunPatchesConditional(bool b, XmlContainer caseTrue, XmlContainer caseFalse, XmlDocument xml)
        {
            if (b)
            {
                if (caseTrue != null)
                {
                    return RunPatches(caseTrue, "caseTrue", xml);
                }
            }
            else
            {
                if (caseFalse != null)
                {
                    return RunPatches(caseFalse, "caseFalse", xml);
                }
            }
            return true;
        }

        protected bool RunPatches(XmlContainer container, XmlDocument xml)
        {
            return RunPatches(container, "apply", xml);
        }

        protected bool RunPatches(XmlContainer container, string name, XmlDocument xml)
        {
            try
            {
                for (int j = 0; j < container.node.ChildNodes.Count; j++)
                {
                    PatchOperation patch;
                    try
                    {
                        patch = Helpers.GetPatchFromString(container.node.ChildNodes[j].OuterXml);
                    }
                    catch (Exception e)
                    {
                        Error("Failed to create patch in <" + name + "> from the operation at position=" + (j + 1).ToString() + ":\n" + e.Message);
                        return false;
                    }
                    if (!patch.Apply(xml))
                    {
                        Error("Error in <" + name + "> in the operation at position=" + (j + 1).ToString());
                        return false;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                Error(e.Message);
                return false;
            }
        }
    }
}