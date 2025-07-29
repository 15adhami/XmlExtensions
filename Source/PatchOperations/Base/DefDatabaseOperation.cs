using System;
using System.Xml;

namespace XmlExtensions
{
    internal abstract class DefDatabaseOperation : PatchOperationValue
    {
        protected bool isValue = false;

        protected override void Initialize()
        {
            requiresDelay = true;
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            return true;
        }

        protected sealed override bool Patch(XmlDocument xml)
        {
            if (!isValue)
            {
                try
                {
                    if (!PreCheck(xml))
                    {
                        return false;
                    }
                    return DoPatch();
                }
                catch (Exception e)
                {
                    Error(e.Message);
                    return false;
                }
            }
            else
            {
                return base.Patch(xml);
            }
        }

        protected virtual bool DoPatch()
        {
            return true;
        }
    }
}