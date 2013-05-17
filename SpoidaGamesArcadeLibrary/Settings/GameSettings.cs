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

        private bool isFullScreen;
        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set { isFullScreen = value; }
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

        public GameSettings(int displayWidth, int displayHeight, bool fullScreenOption, int musicLevel, int soundLevel)
        {
            displayModeWidth = displayWidth;
            displayModeHeight = displayHeight;
            isFullScreen = fullScreenOption;
            musicVolume = musicLevel;
            soundEffectVolume = soundLevel;
        }

        public GameSettings()
        {
        }
    }
}
