using System.Collections.Generic;
using System.Text;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class Unlocks
    {
        public static readonly List<Basketball> OldBasketballs = new List<Basketball>();
        public static readonly List<Basketball> NewBasketballs = new List<Basketball>();
        public static readonly List<Basketball> UnlockedBalls = new List<Basketball>();

        public static bool IsNewUnlockedBalls { get; set; }
        public static bool UnlocksCalculated { get; set; }
        public static bool IsNewHighScore { get; set; }
        public static double CurrentBestScore { get; set; }

        public static double UnlockDisplayTimer { get; set; }
        public static double NewHighScoreFadeOutTimer { get; set; }
        public static double ShowNewHighScoreTimer { get; set; }
        public static double NewHighScoreTimer { get; set; }

        public static float HighScoreFadeValue = 1;
        public static int UnlockedBallCounter { get; set; }
        public static bool IsHighScoreSoundEffectPlayed { get; set; }
        public static bool IsUnlockSoundEffectPlayed { get; set; }

        public static bool HighScoresLoaded { get; set; }
        public static readonly StringBuilder HighScoresPlayers = new StringBuilder();
        public static readonly StringBuilder HighScoresScore = new StringBuilder();
        public static readonly StringBuilder HighScoresStreak = new StringBuilder();
        public static readonly StringBuilder HighScoresMultiplier = new StringBuilder();
    }
}
