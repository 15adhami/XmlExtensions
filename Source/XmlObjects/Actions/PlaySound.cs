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
                Error("Cached SoundDef is null");
                return false;
            }
            if (cachedSound.subSounds.Any((SubSoundDef sub) => sub.onCamera))
            {
                cachedSound.PlayOneShotOnCamera();
            }
            else
            {
                cachedSound.PlayOneShot(SoundInfo.InMap(new TargetInfo(Find.CameraDriver.MapPosition, Find.CurrentMap, true)));
            }
            return true;
        }
    }
}
