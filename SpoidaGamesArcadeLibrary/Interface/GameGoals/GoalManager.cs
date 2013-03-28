namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class GoalManager
    {
        private double gameScore;
        public double GameScore
        {
            get { return gameScore; }
        }

        private int scoreMulitplier;
        public int ScoreMulitplier
        {
            get { return scoreMulitplier; }
            set { scoreMulitplier = value; }
        }

        private int streak;
        public int Streak
        {
            get { return streak; }
            set { streak = value; }
        }

        /// <summary>
        /// The base value that the score added to the total game score is multiplied by.
        /// </summary>
        private double baseScoreMultiplier;
        public double BaseScoreMultiplier
        {
            get { return baseScoreMultiplier; }
        }

        /// <summary>
        /// Initializes the games score, multiplier and streak manager.
        /// </summary>
        /// <param name="baseMultiplier">
        /// The base multiplier is the base value that all score additions will be multiplied by.
        /// For example, setting this to 100 will cause a multiplier of 3 to add 300 points to the total score on the next goal.
        /// </param>
        public GoalManager(double baseMultiplier)
        {
            gameScore = 0;
            streak = 0;
            scoreMulitplier = 1;
            baseScoreMultiplier = baseMultiplier;
        }

        public void GoalScored()
        {
            gameScore += baseScoreMultiplier*scoreMulitplier;
        }
    }
}
