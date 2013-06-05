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

        private bool shakeCameraOnGoal;
        public bool ShakeCameraOnGoal
        {
            get { return shakeCameraOnGoal; }
            set { shakeCameraOnGoal = value; }
        }

        private bool goalScored;
        public bool GoalScored
        {
            get { return goalScored; }
            set { goalScored = value; }
        }

        private bool scoredOnShot;
        public bool ScoredOnShot
        {
            get { return scoredOnShot; }
            set { scoredOnShot = value; }
        }

        private int topStreak;
        public int TopStreak
        {
            get { return topStreak; }
            set { topStreak = value; }
        }

        private bool drawSwish;
        public bool DrawSwish
        {
            get { return drawSwish; }
            set { drawSwish = value; }
        }

        private bool drawCleanShot;
        public bool DrawCleanShot
        {
            get { return drawCleanShot; }
            set { drawCleanShot = value; }
        }

        private string drawStreakMessage;
        public string DrawStreakMessage
        {
            get { return drawStreakMessage; }
            set { drawStreakMessage = value; }
        }

        private bool drawNumberScrollEffect;
        public bool DrawNumberScrollEffect
        {
            get { return drawNumberScrollEffect; }
            set { drawNumberScrollEffect = value; }
        }

        private string numberScrollScore;
        public string NumberScrollScoreToDraw
        {
            get { return numberScrollScore; }
            set { numberScrollScore = value; }
        }

        /// <summary>
        /// The base value that the score added to the total game score is multiplied by.
        /// </summary>
        private double baseScoreMultiplier;
        public double BaseScoreMultiplier
        {
            get { return baseScoreMultiplier; }
        }

        private Rectangle basketLocation;
        public Rectangle BasketLocation
        {
            get { return basketLocation; }
            set { basketLocation = value; }
        }

        private bool backboardHit;
        public bool BackboardHit
        {
            get { return backboardHit; }
            set { backboardHit = value; }
        }

        private bool rimHit;
        public bool RimHit
        {
            get { return rimHit; }
            set { rimHit = value; }
        }
        
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
            gameScore = 0;
            streak = 0;
            scoreMulitplier = 1;
            baseScoreMultiplier = baseMultiplier;
            shakeCameraOnGoal = shakeCamera;
            goalScored = false;
            basketLocation = goalRectangleLocation;
        }

        /// <summary>
        /// Adds points to the game score by multiplying the base score multiplier by the current score multiplier
        /// </summary>
        public void AddPointsForScoredGoal()
        {
            gameScore += baseScoreMultiplier*ScoreMulitplier*Streak;
        }

        /// <summary>
        /// Observes the game loop for triggers that indicate a goal has scored and sets the appropriate flags
        /// </summary>
        public void UpdateGoalScored(GameTime gameTime, Camera camera, Rectangle shotCenterRectangle, SoundEffect goalScoredSoundEffect, SoundEffect streakObtained, SoundEffect laserBoom, SparkleEmitter sparkleEmitter, Starfield starfield, GameSettings gameSettings)
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
                    SoundManager.PlaySoundEffect(laserBoom, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
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

                scoredOnShot = true;
                Streak++;
                if (streak > topStreak)
                {
                    TopStreak = streak;
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
                sparkleEmitter.ParticleCount = 150;
                starfield.StarSpeedModifier = 4;
            }
            else if (Streak >= 6 && Streak < 9)
            {
                sparkleEmitter.Colors = new List<Color> {Color.LimeGreen, Color.Teal, Color.Green};
                sparkleEmitter.ParticleCount = 150;
                starfield.StarSpeedModifier = 9;
            }
            else if (Streak >= 9 && Streak < 15)
            {
                sparkleEmitter.Colors = new List<Color> { Color.DarkRed, Color.Red, Color.IndianRed };
                sparkleEmitter.ParticleCount = 150;
                starfield.StarSpeedModifier = 12;
            }
            else if (Streak >= 15)
            {
                sparkleEmitter.Colors = new List<Color> { Color.Thistle, Color.BlueViolet, Color.RoyalBlue };
                sparkleEmitter.ParticleCount = 150;
                starfield.StarSpeedModifier = 12;
            }
            else
            {
                sparkleEmitter.Colors = new List<Color> {Color.DarkRed, Color.DarkOrange};
                sparkleEmitter.ParticleCount = 150;
                starfield.StarSpeedModifier = 1;
            }
        }

        private bool IsGoalScored(Rectangle basketball)
        {
            return basketLocation.Intersects(basketball);
        }

        public void ResetGoalManager()
        {
            gameScore = 0;
            streak = 0;
            topStreak = 0;
            scoreMulitplier = 1;
            goalScored = false;
            scoredOnShot = false;
        }
    }
}
