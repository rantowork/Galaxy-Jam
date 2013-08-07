namespace SpoidaGamesArcadeLibrary.Globals
{
    public class GameState
    {
        public enum GameStates
        {
            StartScreen,
            TitleScreen,
            SettingsScreen,
            PracticeScreen,
            TutorialScreen,
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
