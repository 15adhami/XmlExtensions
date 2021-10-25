using System.Collections.Generic;
using System.Xml;
using XmlExtensions.Action;

namespace XmlExtensions
{
    public class ApplyAction : PatchOperationValue
    {
        public ActionContainer action;

        protected override void SetException()
        {
            if (storeIn != null)
            {
                CreateExceptions(storeIn, "storeIn");
            }
        }

        protected override bool PreCheck(XmlDocument xml)
        {
            return true;
        }

        protected override bool Patch(XmlDocument xml)
        {
            if (storeIn != null)
            {
                return base.Patch(xml);
            }
            else
            {
                return ErrorIfFalse(action.DoAction(), "Error while applying action");
            }
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            if (!ErrorIfFalse(action.DoAction(), "Error while applying action"))
            {
                return false;
            }
            vals.Add(action.output.ToString());
            return true;
        }
    }
}