using System;

namespace SpoidaGamesArcadeLibrary.Settings
{
    [Serializable]
    public class GameSettings
    {
        public int DisplayModeWidth { get; set; }
        public int DisplayModeHeight { get; set; }
        public int FullScreenOption { get; set; }
        public int MusicVolume { get; set; }
        public int SoundEffectVolume { get; set; }

        public GameSettings(int displayWidth, int displayHeight, int fullScreen, int musicLevel, int soundLevel)
        {
            DisplayModeWidth = displayWidth;
            DisplayModeHeight = displayHeight;
            FullScreenOption = fullScreen;
            MusicVolume = musicLevel;
            SoundEffectVolume = soundLevel;
        }

        public GameSettings()
        {
        }
    }
}
