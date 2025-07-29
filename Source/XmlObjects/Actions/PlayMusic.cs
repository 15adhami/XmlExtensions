using Verse;

namespace XmlExtensions.Action
{
    internal class PlayMusic : ActionContainer
    {
        public string songDefName;

        private SongDef song;

        protected override bool Init()
        {
            if (string.IsNullOrEmpty(songDefName))
            {
                Error("songDefName is null or empty");
                return false;
            }

            song = DefDatabase<SongDef>.GetNamedSilentFail(songDefName);
            if (song == null)
            {
                Error($"SongDef \"{songDefName}\" not found.");
                return false;
            }

            return true;
        }

        protected override bool ApplyAction()
        {
            if (song == null)
            {
                Error("SongDef is null");
                return false;
            }
            Verse.Log.Message(song.defName);
            Find.MusicManagerPlay.ForcePlaySong(song, false);
            return true;
        }
    }
}
