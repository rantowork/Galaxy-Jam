using System;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Settings
{
    [Serializable]
    public class GameSettings
    {
        private DisplayMode resolution;
        public DisplayMode Resolution
        {
            get { return resolution; }
            set { resolution = value; }
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

        public GameSettings(DisplayMode videoResolution, bool fullScreenOption, int musicLevel, int soundLevel)
        {
            resolution = videoResolution;
            isFullScreen = fullScreenOption;
            musicVolume = musicLevel;
            soundEffectVolume = soundLevel;
        }
    }
}
