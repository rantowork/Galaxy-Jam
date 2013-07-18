using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects.Environment;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class GoalManager
    {
        public double GameScore { get; private set; }
        public int ScoreMulitplier { get; set; }
        public int Streak { get; set; }
        public bool ShakeCameraOnGoal { get; set; }
        public bool GoalScored { get; set; }
        public bool ScoredOnShot { get; set; }
        public int TopStreak { get; set; }
        public bool DrawSwish { get; set; }
        public bool DrawCleanShot { get; set; }
        public string DrawStreakMessage { get; set; }
        public bool DrawNumberScrollEffect { get; set; }
        public string NumberScrollScoreToDraw { get; set; }
        public double BaseScoreMultiplier { get; private set; }
        public static Rectangle BasketLocation { get; set; }
        public bool BackboardHit { get; set; }
        public bool RimHit { get; set; }

        /// <summary>
        /// Initializes the games score, multiplier and streak manager.
        /// </summary>
        /// <param name="baseMultiplier">
        /// The base multiplier is the base value that all score additions will be multiplied by.
        /// For example, setting this to 100 will cause a multiplier of 3 to add 300 points to the total score on the next goal.
        /// </param>
        /// <param name="shakeCamera">
        /// Determines wether the camera will shake when a goal is scored.
        /// The shake camera property does have a setter so it can be modified mid game if it is no longer needed.
        /// </param>
        /// <param name="goalRectangleLocation">
        /// The rectangle where the goal will be scored when intersecting
        /// </param>
        public GoalManager(double baseMultiplier, bool shakeCamera, Rectangle goalRectangleLocation)
        {
            GameScore = 0;
            Streak = 0;
            ScoreMulitplier = 1;
            BaseScoreMultiplier = baseMultiplier;
            ShakeCameraOnGoal = shakeCamera;
            GoalScored = false;
            BasketLocation = goalRectangleLocation;
        }

        /// <summary>
        /// Adds points to the game score by multiplying the base score multiplier by the current score multiplier
        /// </summary>
        public void AddPointsForScoredGoal()
        {
            GameScore += BaseScoreMultiplier*ScoreMulitplier*Streak;
        }

        /// <summary>
        /// Observes the game loop for triggers that indicate a goal has scored and sets the appropriate flags
        /// </summary>
        public void UpdateGoalScored(GameTime gameTime, Camera camera, Rectangle shotCenterRectangle, SoundEffect goalScoredSoundEffect, SoundEffect streakObtained, Emitter sparkleEmitter, Starfield starfield, GameSettings gameSettings)
        {
            if (IsGoalScored(shotCenterRectangle) && !GoalScored)
            {
                GoalScored = true;

                if (!BackboardHit && RimHit)
                {
                    ScoreMulitplier++;
                    DrawCleanShot = true;
                    SoundManager.PlaySoundEffect(goalScoredSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }
                else if (!BackboardHit && !RimHit)
                {
                    ScoreMulitplier += 2;
                    DrawSwish = true;
                    SoundManager.PlaySoundEffect(goalScoredSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }
                else
                {
                    SoundManager.PlaySoundEffect(goalScoredSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }

                //Adds bonus multiplier based on streak
                if (Streak >= 3 && Streak < 6)
                {
                    ScoreMulitplier++;
                }

                //Adds bonus multiplier based on awesome streak!
                if (Streak >= 6 && Streak < 9)
                {
                    ScoreMulitplier += 2;
                }

                //Adds bonus multiplier based on godly streak!
                if (Streak >= 9)
                {
                    ScoreMulitplier += 3;
                }

                ScoredOnShot = true;
                Streak++;
                if (Streak > TopStreak)
                {
                    TopStreak = Streak;
                }
                if (Streak == 3 || Streak == 6 || Streak == 9 || Streak == 15)
                {
                    SoundManager.PlaySoundEffect(streakObtained, (float)gameSettings.SoundEffectVolume / 10, 0f, 0f);
                }
                AddPointsForScoredGoal();

                camera.Shaking = true;
                DrawNumberScrollEffect = true;
            }

            if (Streak >= 3 && Streak < 6)
            {
                DrawStreakMessage = "Good streak!";
            }

            if (Streak >= 6 && Streak < 9)
            {
                DrawStreakMessage = "Mega streak!";
            }

            if (Streak >= 9)
            {
                DrawStreakMessage = "ULTRA Streak!";
            }

            if (Streak >= 15)
            {
                DrawStreakMessage = "INHUMAN STREAK!";
            }

            if (Streak == 0)
            {
                DrawStreakMessage = string.Empty;
            }

            if (camera.Shaking)
            {
                camera.ShakeCamera(gameTime);
            }
            else
            {
                camera.Position = Vector2.Zero;
            }

            if (Streak >= 3 && Streak < 6)
            {
                sparkleEmitter.Colors = new List<Color> { Color.Purple, Color.Plum, Color.Orchid};
                starfield.StarSpeedModifier = 4;
            }
            else if (Streak >= 6 && Streak < 9)
            {
                sparkleEmitter.Colors = new List<Color> {Color.LimeGreen, Color.Teal, Color.Green};
                starfield.StarSpeedModifier = 9;
            }
            else if (Streak >= 9 && Streak < 15)
            {
                sparkleEmitter.Colors = new List<Color> { Color.DarkRed, Color.Red, Color.IndianRed };
                starfield.StarSpeedModifier = 12;
            }
            else if (Streak >= 15)
            {
                sparkleEmitter.Colors = new List<Color> { Color.Thistle, Color.BlueViolet, Color.RoyalBlue };
                starfield.StarSpeedModifier = 12;
            }
            else
            {
                sparkleEmitter.Colors = new List<Color> {Color.DarkRed, Color.DarkOrange};
                starfield.StarSpeedModifier = 1;
            }
        }

        private bool IsGoalScored(Rectangle basketball)
        {
            return BasketLocation.Intersects(basketball);
        }

        public void ResetGoalManager()
        {
            GameScore = 0;
            Streak = 0;
            TopStreak = 0;
            ScoreMulitplier = 1;
            GoalScored = false;
            ScoredOnShot = false;
        }
    }
}
