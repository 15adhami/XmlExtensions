using System.Xml;

namespace XmlExtensions.Boolean
{    public abstract class BooleanBase
    {
        public string xmlDoc;

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
}
