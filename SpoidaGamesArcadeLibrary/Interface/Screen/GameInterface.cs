using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

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
        private static Cue previousCue;

        public static void DrawOptionsInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, bool nameToShort, int currentBasketballSelection, int currentSongSelection)
        {
            Vector2 instructionsOrigin = pixelFont.MeasureString(INSTRUCTIONS) / 2;
            Vector2 nameErrorOrigin = pixelFont.MeasureString(NAME_ERROR) / 2;
            
            spriteBatch.DrawString(pixelFont, INSTRUCTIONS, new Vector2(1280 / 2, 180), Color.White, 0.0f, instructionsOrigin, 1f, SpriteEffects.None, 1.0f);

            if (nameToShort)
            {
                spriteBatch.DrawString(pixelFont, NAME_ERROR, new Vector2(1280 / 2, 675), Color.Red, 0.0f, nameErrorOrigin, 1f, SpriteEffects.None, 1.0f);
            }

            if (BasketballManager.basketballSelection.Count == 0)
            {
                int count = 0;
                var values = Enum.GetValues(typeof(BasketballTypes));
                foreach (BasketballTypes type in values)
                {
                    BasketballManager.basketballSelection.Add(count, type);
                    count++;
                }
            }

            if (SoundManager.musicSelection.Count == 0)
            {
                int count = 0;
                var values = Enum.GetValues(typeof (SongTypes));
                foreach (SongTypes type in values)
                {
                    SoundManager.musicSelection.Add(count, type);
                    count++;
                }
            }

            SongTypes songType;
            if (SoundManager.musicSelection.TryGetValue(currentSongSelection, out songType))
            {
                spriteBatch.DrawString(pixelFont, GetSongTypeString(songType), new Vector2(500, 600), Color.White);
            }

            Cue cue;
            if (SoundManager.music.TryGetValue(songType, out cue))
            {
                if (previousCue == null)
                {
                    previousCue = cue;
                    SoundManager.PlayBackgroundMusic();
                }
                if (cue != previousCue)
                {
                    previousCue = cue;
                    SoundManager.SelectedMusic.Stop(AudioStopOptions.Immediate);
                    SoundManager.SelectedMusic.Dispose();
                    Cue newCue = SoundManager.soundBank.GetCue(cue.Name);
                    SoundManager.SelectedMusic = newCue;
                    SoundManager.SelectedMusic.Play();
                }
            }

            BasketballTypes basketballTypes;
            if (BasketballManager.basketballSelection.TryGetValue(currentBasketballSelection, out basketballTypes))
            {
                spriteBatch.DrawString(pixelFont, GetBasketballTypeString(basketballTypes), new Vector2(500, 500), Color.White);
            }

            Basketball basketball;
            if (BasketballManager.basketballs.TryGetValue(basketballTypes, out basketball))
            {
                spriteBatch.Draw(basketball.BasketballTexture, new Vector2(740, 508), basketball.Source, Color.White, 0f, basketball.Origin, 1.0f, SpriteEffects.None, 0f);
                BasketballManager.SelectedBasketball = basketball;
            }
        }
        
        private static string GetBasketballTypeString(BasketballTypes type)
        {
            if (type == BasketballTypes.RedGlowBall)
            {
                return "Red Glow Ball";
            }
            if (type == BasketballTypes.GreenGlowBall)
            {
                return "Green Glow Ball";
            }
            if (type == BasketballTypes.YellowGlowBall)
            {
                return "Yellow Glow Ball";
            }
            return "Puple Skull Ball";
        }

        private static string GetSongTypeString(SongTypes type)
        {
            if (type == SongTypes.BouncyLoop1)
            {
                return "Bouncy Loop";
            }
            if (type == SongTypes.SpaceLoop1)
            {
                return "Spacey Loop 1";
            }
            return "Spacey Loop 2";
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
