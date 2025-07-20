using System.Collections.Generic;
using System.Xml;
using Verse;

namespace XmlExtensions
{
    internal class PatchCoordinator
    {
        public bool IsApplyingPatches { get; set; } = false;
        public ModContentPack ActiveMod { get; set; }

        public Dictionary<PatchOperation, ModContentPack> PatchModDict { get; } = [];

        public Dictionary<string, PatchDef> PatchDefs = [];

        // For DefDatabase Operations
        public List<PatchOperationExtended> DelayedPatches { get; } = [];

        public void SetActiveMod(ModContentPack mod) { ActiveMod = mod; }

        public Dictionary<XmlNode, LoadableXmlAsset> assetlookup;
    }
}
