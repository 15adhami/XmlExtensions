﻿using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    public class PatchDef : Def
    {
        public List<string> parameters;
        public XmlContainer apply;
        public string brackets = "{}";
    }
}