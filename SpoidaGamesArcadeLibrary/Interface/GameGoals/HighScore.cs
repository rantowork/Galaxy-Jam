using System;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    [Serializable]
    public class HighScore
    {
        private string currentPlayerName;
        public string CurrentPlayerName
        {
            get { return currentPlayerName; }
            set { currentPlayerName = value; }
        }

        private double playerScore;
        public double PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }

        private int playerTopStreak;
        public int PlayerTopStreak
        {
            get { return playerTopStreak; }
            set { playerTopStreak = value; }
        }

        private int playerMultiplier;
        public int PlayerMultiplier
        {
            get { return playerMultiplier; }
            set { playerMultiplier = value; }
        }

        public HighScore(string playerName, double score, int topStreak, int multiplier)
        {
            currentPlayerName = playerName;
            playerScore = score;
            playerTopStreak = topStreak;
            playerMultiplier = multiplier;
        }

        public HighScore()
        {
        }
    }
}
