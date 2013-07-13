using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ComputerSettings
    {
        public static DisplayMode PreviousDisplayMode { get; set; }
        public static DisplayMode DefaultDisplayMode { get; set; }

        public static int CurrentResolution { get; set; }
        public static int CurrentSettingSelection { get; set; }
        public static int FullScreenSetting { get; set; }
        public static int MusicVolumeSetting { get; set; }
        public static int SoundEffectVolumeSetting { get; set; }
        public static int PreviousFullScreenSetting { get; set; }
        public static int PreviousMusicSetting { get; set; }
        public static int PreviousSoundEffectSetting { get; set; }
    }
}
