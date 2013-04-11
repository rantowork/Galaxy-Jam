using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;

namespace SpoidaGamesArcadeLibrary.Interface.Screen
{
    public class GameInterface
    {
        //Title Screen Interface
        public static void DrawTitleScreen(SpriteBatch spriteBatch, Texture2D startScreenLogo)
        {
            spriteBatch.Draw(startScreenLogo, new Rectangle(0, 0, 1280, 720), Color.White);
        }

        //Options Interface
        const string INSTRUCTIONS = "Input your name and hit enter to begin";
        const string NAME_ERROR = "Name must be between 3 and 12 characters!";

        public static void DrawOptionsInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, bool nameToShort)
        {
            Vector2 instructionsOrigin = pixelFont.MeasureString(INSTRUCTIONS) / 2;
            Vector2 nameErrorOrigin = pixelFont.MeasureString(NAME_ERROR) / 2;
            
            spriteBatch.DrawString(pixelFont, INSTRUCTIONS, new Vector2(1280 / 2, 180), Color.White, 0.0f, instructionsOrigin, 1f, SpriteEffects.None, 1.0f);

            if (nameToShort)
            {
                spriteBatch.DrawString(pixelFont, NAME_ERROR, new Vector2(1280 / 2, 675), Color.Red, 0.0f, nameErrorOrigin, 1f, SpriteEffects.None, 1.0f);
            }

            spriteBatch.DrawString(pixelFont, "Select a Basketball", new Vector2(1000, 100), Color.White);
        }

        //Playing Interface
        public static void DrawPlayingInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, GoalManager goalManager)
        {
            string currentScore = String.Format("{0}", goalManager.GameScore);
            string currentMultiplier = String.Format("Score Multiplier: {0}", goalManager.ScoreMulitplier);
            string currentStreak = String.Format("Streak: {0}", goalManager.Streak);
            string timeRemaining = String.Format("{0}", GameTimer.GetElapsedGameTime());

            Vector2 currentScoreOrigin = pixelGlowFont.MeasureString(currentScore) / 2;

            spriteBatch.DrawString(pixelGlowFont, currentScore, new Vector2(1280 / 2, 30), Color.White, 0f, currentScoreOrigin, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(pixelFont, currentMultiplier, new Vector2(1020, 694), Color.White);
            spriteBatch.DrawString(pixelFont, currentStreak, new Vector2(1100, 22), Color.White);
            spriteBatch.DrawString(pixelGlowFont, timeRemaining, new Vector2(10, 664), Color.White);
        }

        //Game Over Interface
        private const string GAME_OVER = "Game Over!";
        private static string gameOverTimer = String.Format("Time Remaining: {0}", String.Format("{0:00}:{1:00}", new TimeSpan(0, 0, 0, 0).Minutes, new TimeSpan(0, 0, 0, 0).Seconds));

        public static void DrawGameEndInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, GoalManager goalManager)
        {
            string finalScore = String.Format("Final Score: {0}!", goalManager.GameScore);
            Vector2 finalScoreOrigin = pixelFont.MeasureString(finalScore) / 2;
            Vector2 gameOverOrigin = pixelFont.MeasureString(GAME_OVER) / 2;

            spriteBatch.DrawString(pixelFont, GAME_OVER, new Vector2(1280 / 2, 340), Color.White, 0, gameOverOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(pixelFont, finalScore, new Vector2(1280 / 2, 370), Color.White, 0, finalScoreOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(pixelFont, gameOverTimer, new Vector2(10, 694), Color.White);
            spriteBatch.DrawString(pixelFont, "High Scores", new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(pixelFont, "Player", new Vector2(10, 50), Color.White);
            spriteBatch.DrawString(pixelFont, "Top Streak", new Vector2(170, 50), Color.White);
            spriteBatch.DrawString(pixelFont, "Score", new Vector2(340, 50), Color.White);
        }

        //Paused Interface
        const string PAUSED = "Paused!";
        public static void DrawPausedInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont)
        {
            Vector2 pausedOrigin = pixelFont.MeasureString(PAUSED) / 2;
            spriteBatch.DrawString(pixelFont, PAUSED, new Vector2(1280 / 2, 720 / 2), Color.White, 0, pausedOrigin, 1f, SpriteEffects.None, 0);
        }
    }
}
