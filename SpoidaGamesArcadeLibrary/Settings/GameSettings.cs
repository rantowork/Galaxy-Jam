using System;

namespace SpoidaGamesArcadeLibrary.Settings
{
    [Serializable]
    public class GameSettings
    {
        private int displayModeWidth;
        public int DisplayModeWidth
        {
            get { return displayModeWidth; }
            set { displayModeWidth = value; }
        }

        private int displayModeHeight;
        public int DisplayModeHeight
        {
            get { return displayModeHeight; }
            set { displayModeHeight = value; }
        }

        private int fullScreenOption;
        public int FullScreenOption
        {
            get { return fullScreenOption; }
            set { fullScreenOption = value; }
        }

        private int musicVolume;
        public int MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = value; }
        }

        private int soundEffectVolume;
        public int SoundEffectVolume
        {
            get { return soundEffectVolume; }
            set { soundEffectVolume = value; }
        }

        public GameSettings(int displayWidth, int displayHeight, int fullScreen, int musicLevel, int soundLevel)
        {
            displayModeWidth = displayWidth;
            displayModeHeight = displayHeight;
            fullScreenOption = fullScreen;
            musicVolume = musicLevel;
            soundEffectVolume = soundLevel;
        }

        public GameSettings()
        {
        }
    }
}
