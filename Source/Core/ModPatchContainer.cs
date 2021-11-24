using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    internal class ModPatchContainer
    {
        public List<string> defNames;
        public int patchCount = 0;
        public int elapsedTime = 0;

        public ModPatchContainer()
        {
            defNames = new();
        }
    }
}