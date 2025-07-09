using System.Diagnostics;

namespace XmlExtensions.Source.Core
{
    internal class PatchProfiler
    {
        private Stopwatch globalWatch = new();
        private Stopwatch patchWatch = new();
        public int TotalPatches { get; private set; }
        public int FailedPatches { get; private set; }

        public void Start() { patchWatch.Start(); }
        public void Stop() { patchWatch.Stop(); }
        public void Reset() { patchWatch.Reset(); }
        public void Resume() { patchWatch.Resume(); }
        public void Pause() { patchWatch.Pause(); }
        public void CountPatch(bool success) { /* ... */ }
    }
}
