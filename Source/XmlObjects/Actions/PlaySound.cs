using Verse;
using Verse.Sound;

namespace XmlExtensions.Action
{
    internal class PlaySound : ActionContainer
    {
        public string soundDefName;

        private SoundDef cachedSound;

        protected override bool Init()
        {
            if (string.IsNullOrEmpty(soundDefName))
            {
                Error("soundDefName is null or empty");
                return false;
            }

            cachedSound = DefDatabase<SoundDef>.GetNamedSilentFail(soundDefName);
            if (cachedSound == null)
            {
                Error($"SoundDef \"{soundDefName}\" not found.");
                return false;
            }

            return true;
        }

        protected override bool ApplyAction()
        {
            if (cachedSound == null)
            {
                Error("Cached SoundDef is null (this should not happen if Init succeeded)");
                return false;
            }

            cachedSound.PlayOneShotOnCamera();
            return true;
        }
    }
}
