﻿using System.Xml;

namespace XmlExtensions.Boolean
{
    internal class ModHasBeenUsed : BooleanBase
    {
        public string modId;

        protected override bool Evaluation(ref bool b, XmlDocument xml)
        {
            b = XmlMod.modsWithSettings.Contains(modId);
            return true;
        }
    }
}