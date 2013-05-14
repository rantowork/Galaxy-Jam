namespace SpoidaGamesArcadeLibrary.Settings
{
    public class SettingsManager
    {
        public static int GetResolutionWidth(int resolutionSetting)
        {
            if (resolutionSetting == 0)
            {
                return 1280;
            }

            return 1280;
        }

        public static int GetResolutionHeight(int resolutionSetting)
        {
            if (resolutionSetting == 0)
            {
                return 720;
            }
            return 720;
        }
    }
}
