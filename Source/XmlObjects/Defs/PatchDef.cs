using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    /// <summary>
    /// A Def that contains a patch
    /// </summary>
    public class PatchDef : Def
    {
        /// <summary>
        /// The list of parameters
        /// </summary>
        public List<string> parameters;

        /// <summary>
        /// The patch operations to apply
        /// </summary>
        public XmlContainer apply;

        /// <summary>
        /// Brackets used in variable substitution
        /// </summary>
        public string brackets = "{}";

        /// <summary>
        /// Whether or not the PatchDef returns a value
        /// </summary>
        protected bool valueOperation = false;
    }
}