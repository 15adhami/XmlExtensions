using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal abstract class PatchOperationExtended : PatchOperation
    {
        public string xmlDoc;
        public bool requiresDelay = false;

        protected List<string> exceptionVals;
        protected List<string> exceptionFields;

        public sealed override bool ApplyWorker(XmlDocument xml)
        {
            try
            {
                Initialize();
                if (PatchManager.applyingPatches && requiresDelay)
                {
                    if (!PatchManager.PatchModDict.ContainsKey(this) && PatchManager.ActiveMod != null)
                        PatchManager.PatchModDict.Add(this, PatchManager.ActiveMod);
                    PatchManager.delayedPatches.Add(this);
                    return true;
                }
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
                if (!PreCheck(doc))
                {
                    return false;
                }
                return Patch(doc);
            }
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }
        }

        protected virtual void Initialize()
        {

        }

        protected virtual bool PreCheck(XmlDocument xml)
        {
            return true;
        }

        protected virtual bool Patch(XmlDocument xml)
        {
            return false;
        }

        protected virtual void SetException()
        {
        }

        protected void CreateExceptions(params string[] values)
        {
            exceptionFields = new List<string>();
            exceptionVals = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                {
                    exceptionVals.Add(values[i]);
                }
                else
                {
                    exceptionFields.Add(values[i]);
                }
            }
        }

        /// <summary>
        /// Throws an error for the patch operation
        /// </summary>
        /// <param name="msg">The message to be displayed</param>
        protected void Error(string msg)
        {
            SetException();
            ErrorManager.AddError(new ErrorContext(
                GetType(),
                exceptionFields,
                exceptionVals,
                msg
            ));
        }

        private protected void Error(string[] vals, string[] fields, string msg)
        {
            ErrorManager.AddError(new ErrorContext(
                GetType(),
                new List<string>(fields),
                new List<string>(vals),
                msg
            ));
        }

        protected bool ErrorIfFalse(bool condition, string message)
        {
            if (!condition)
            {
                Error(message);
                return false;
            }
            return true;
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
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }
        }
    }
}