namespace SpoidaGamesArcadeLibrary.Globals
{
    public class GameState
    {
        public enum GameStates
        {
            Spoida,
            Dpsf,
            StartScreen,
            TitleScreen,
            SettingsScreen,
            PracticeScreen,
            TutorialScreen,
            Credits,
            OptionsScreen,
            GetReadyState,
            Playing,
            ArcadeMode,
            GameEnd,
            Paused
        } ;

        public static GameStates States { get; set; }

        public static int SelectedGameMode { get; set; }
    }
}
