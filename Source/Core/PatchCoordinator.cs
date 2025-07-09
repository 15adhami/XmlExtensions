using System.Collections.Generic;
using Verse;

namespace XmlExtensions.Source.Core
{
    internal class PatchCoordinator
    {
        public bool IsApplyingPatches { get; set; }
        public ModContentPack ActiveMod { get; set; }
        public Dictionary<PatchOperation, ModContentPack> PatchModDict { get; } = [];
        public List<PatchOperationExtended> DelayedPatches { get; } = [];

        public void SetActiveMod(ModContentPack mod) { ActiveMod = mod; }
    }
}
