using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class Translate : PatchOperationValue
    {
        public string tKey;

        protected override void SetException()
        {
            CreateExceptions(storeIn, "storeIn", tKey, "tKey");
        }

        public override bool getValues(List<string> vals, XmlDocument xml)
        {
            try
            {
                vals.Add(tKey.TranslateSimple());
            }
            catch
            {
                Error("Given tKey not found");
                return false;
            }
            return true;
        }
    }
}
