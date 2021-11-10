using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace XmlExtensions
{
    internal class DefDatabaseForEach : DefDatabaseOperation
    {
        public string objPath;

        protected override bool DoPatch()
        {
            List<ObjectContainer> list = SelectObjects(objPath);
            if (list.Count == 0)
            {
                XPathError("objPath");
                return false;
            }
            foreach (ObjectContainer objContainer in list)
            {
                XmlContainer newContainer = Helpers.SubstituteVariableXmlContainer(apply, storeIn, objContainer.objPath, brackets);
                if (!RunPatches(newContainer, null))
                {
                    return false;
                }
            }
            return true;
        }
    }
}