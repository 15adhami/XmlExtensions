using System.Collections.Generic;
using Verse;

namespace XmlExtensions
{
    internal class PatchCoordinator
    {
        public bool IsApplyingPatches { get; set; } = false;
        public ModContentPack ActiveMod { get; set; }

        public Dictionary<PatchOperation, ModContentPack> PatchModDict { get; } = [];

        // For DefDatabase Operations
        public List<PatchOperationExtended> DelayedPatches { get; } = [];

        public void SetActiveMod(ModContentPack mod) { ActiveMod = mod; }
    }
}
