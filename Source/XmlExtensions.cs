using Verse;
using System.Xml;
using System.Collections.Generic;

namespace XmlExtensions
{ 
    public class XmlExtensions : Mod
    {
        public Mod_Settings settings;
        public Dictionary<string, Mod_Settings> modSettingsDict;

        public XmlExtensions(ModContentPack content) : base(content)
        {
            settings = GetSettings<Mod_Settings>();
        }

        public override string SettingsCategory()
        {
            return "XML Extensions";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoSettingsWindowContents(inRect);
        }
    }


    public abstract class PatchOperationBoolean : PatchOperationPathed
    {
        public override string ToString()
        {
            return string.Format("{0}({1})", base.ToString(), this.xpath);
        }

        public bool evaluate(XmlDocument xml)
        {
            if (!this.valid)
            {
                this.flag = evaluation(xml);
            }
            return this.flag;
        }

        protected abstract bool evaluation(XmlDocument xml);
        private bool valid = false;
        protected bool flag = false;
    }
}