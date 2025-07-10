using System.Diagnostics;

namespace XmlExtensions.Source.Core
{
    internal class PatchProfiler
    {
        public Stopwatch globalWatch = new();
        public Stopwatch patchWatch = new();
        public int TotalPatches { get; set; } = 0;
        public int FailedPatches { get; set; } = 0;

        public void StartWatch() { patchWatch.Start(); }
        public void StopWatch() { patchWatch.Stop(); }
        public void ResetWatch() { patchWatch.Reset(); }
        public bool IsRunning() { return patchWatch.IsRunning; }
        public long ElapsedMilliseconds() { return patchWatch.ElapsedMilliseconds; }
        public void CountPatch(bool success) { /* ... */ }
    }
}
