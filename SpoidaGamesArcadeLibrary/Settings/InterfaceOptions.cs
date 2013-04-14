namespace SpoidaGamesArcadeLibrary.Settings
{
    public class InterfaceOptions
    {
        private static bool backgroundMusicMuted;
        public static bool BackgroundMusicMuted
        {
            get { return backgroundMusicMuted; }
            set { backgroundMusicMuted = value; }
        }

        private static bool allSoundsMuted;
        public static bool AllSoundsMuted
        {
            get { return allSoundsMuted; }
            set { allSoundsMuted = value; }
        }

        public InterfaceOptions()
        {
            BackgroundMusicMuted = false;
            AllSoundsMuted = false;
        }
    }
}
