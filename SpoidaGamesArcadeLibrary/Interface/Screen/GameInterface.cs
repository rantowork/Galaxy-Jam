using System;
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
        const string NAME_ERROR = "Name must be between 3 and 12 characters!";
        private const string GO_BACK_TEXT = "(Esc) Back";
        private static Cue previousCue;
        private static int previousBasketballSelection;
        private const double WEAPON_SWITCH_TIMER = 300;
        private static double weaponSwitchElapsedTimer;

        public static void DrawOptionsInterface(SpriteBatch spriteBatch, GameTime gameTimer, SpriteFont pixelFont, HighScoreManager highScoreManager, bool nameToShort, int currentBasketballSelection, int currentSongSelection)
        {
            spriteBatch.DrawString(pixelFont, GO_BACK_TEXT, new Vector2(10, 10), Color.White);

            if (nameToShort)
            {
                spriteBatch.DrawString(pixelFont, NAME_ERROR, new Vector2(30, 440), Color.Red);
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
                string songName = GetSongTypeString(songType);
                Vector2 songOrigin = pixelFont.MeasureString(songName);
                spriteBatch.DrawString(pixelFont, songName, new Vector2(1003, 576), Color.White, 0f, songOrigin / 2, 1.0f, SpriteEffects.None, 1f);
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

            if (previousBasketballSelection == currentBasketballSelection)
            {
                BasketballTypes basketballTypes;
                if (BasketballManager.basketballSelection.TryGetValue(currentBasketballSelection, out basketballTypes))
                {
                    Basketball basketball;
                    if (BasketballManager.basketballs.TryGetValue(basketballTypes, out basketball))
                    {
                        if (IsBasketballLocked(basketball, highScoreManager))
                        {
                            string lockedText = String.Format("Unlock With {0} Points", basketball.BasketballUnlockScore);
                            Vector2 lockedCenter = pixelFont.MeasureString(lockedText) / 2;
                            spriteBatch.DrawString(pixelFont, lockedText, new Vector2(1003, 140), Color.White, 0f, lockedCenter, 1f, SpriteEffects.None, 1f);
                            Texture2D lockedTexture = BasketballManager.lockedBasketballTextures[0];
                            spriteBatch.Draw(lockedTexture, new Vector2(1003, 230), null, Color.White, 0f, new Vector2((float)lockedTexture.Width / 2, (float)lockedTexture.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
                        }
                        else
                        {
                            Vector2 basketballTextCenter = pixelFont.MeasureString(basketball.BasketballName)/2;
                            spriteBatch.DrawString(pixelFont, basketball.BasketballName, new Vector2(1003, 140), Color.White, 0f, basketballTextCenter, 1f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(basketball.BasketballTexture, new Vector2(1003, 230), null, Color.White, 0f, basketball.Origin, 1.0f, SpriteEffects.None, 1.0f);
                            BasketballManager.SelectedBasketball = basketball;
                        }
                    }
                }
            }
            else
            {
                highScoreManager.CanChangeBasketballSelection = false;

                BasketballTypes previousBasketballType = BasketballManager.basketballSelection[previousBasketballSelection];
                BasketballTypes nextBasketballType = BasketballManager.basketballSelection[currentBasketballSelection];
                Basketball previousBasketball = BasketballManager.basketballs[previousBasketballType];
                Basketball nextBasketball = BasketballManager.basketballs[nextBasketballType];

                Texture2D previousTexture;
                Texture2D nextTexture;
                Vector2 previousCenter;
                Vector2 nextCenter;

                if (IsBasketballLocked(previousBasketball, highScoreManager))
                {
                    previousTexture = BasketballManager.lockedBasketballTextures[0];
                    highScoreManager.LockedBasketballSelection = true;
                    previousCenter = new Vector2((float) previousTexture.Width/2, (float) previousTexture.Height/2);
                }
                else
                {
                    previousTexture = previousBasketball.BasketballTexture;
                    highScoreManager.LockedBasketballSelection = false;
                    previousCenter = previousBasketball.Origin;
                }

                if (IsBasketballLocked(nextBasketball, highScoreManager))
                {
                    nextTexture = BasketballManager.lockedBasketballTextures[0];
                    highScoreManager.LockedBasketballSelection = true;
                    nextCenter = new Vector2((float) nextTexture.Width/2, (float) nextTexture.Height/2);
                }
                else
                {
                    nextTexture = nextBasketball.BasketballTexture;
                    highScoreManager.LockedBasketballSelection = false;
                    nextCenter = nextBasketball.Origin;
                }

                weaponSwitchElapsedTimer += gameTimer.ElapsedGameTime.TotalMilliseconds;

                float amountToFade = MathHelper.Clamp((float)weaponSwitchElapsedTimer / 300, 0, 1);
                float fadeOutValue = MathHelper.Lerp(255, 0, amountToFade);
                float fadeInValue = MathHelper.Lerp(0, 255, amountToFade);
                float moveUpValue1 = MathHelper.Lerp(230, 170, amountToFade);
                float moveUpValue2 = MathHelper.Lerp(290, 230, amountToFade);
                float moveDownValue1 = MathHelper.Lerp(230, 290, amountToFade);
                float moveDownValue2 = MathHelper.Lerp(170, 230, amountToFade);
                float moveScaleValue1 = MathHelper.Lerp(1, .4f, amountToFade);
                float moveScaleValue2 = MathHelper.Lerp(.4f, 1, amountToFade);

                if (previousBasketballSelection < currentBasketballSelection)
                {
                    //up
                    spriteBatch.Draw(previousTexture, new Vector2(1003, moveUpValue1), null, new Color(255, 255, 255, (byte)fadeOutValue), 0f, previousCenter, moveScaleValue1, SpriteEffects.None, 1.0f);
                    spriteBatch.Draw(nextTexture, new Vector2(1003, moveUpValue2), null, new Color(255, 255, 255, (byte)fadeInValue), 0f, nextCenter, moveScaleValue2, SpriteEffects.None, 1.0f);
                }
                else
                {
                    //down
                    spriteBatch.Draw(previousTexture, new Vector2(1003, moveDownValue1), null, new Color(255, 255, 255, (byte)fadeOutValue), 0f, previousCenter, moveScaleValue1, SpriteEffects.None, 1.0f);
                    spriteBatch.Draw(nextTexture, new Vector2(1003, moveDownValue2), null, new Color(255, 255, 255, (byte)fadeInValue), 0f, nextCenter, moveScaleValue2, SpriteEffects.None, 1.0f);
                }

                if (weaponSwitchElapsedTimer >= WEAPON_SWITCH_TIMER)
                {
                    weaponSwitchElapsedTimer = 0;
                    previousBasketballSelection = currentBasketballSelection;
                    highScoreManager.CanChangeBasketballSelection = true;
                }
            }
        }

        private static bool IsBasketballLocked(Basketball basketball, HighScoreManager highScoreManager)
        {
            if (basketball.BasketballUnlockScore <= highScoreManager.BestScore())
            {
                return false;
            }
            return true;
        }

        private static string GetSongTypeString(SongTypes type)
        {
            if (type == SongTypes.BouncyLoop1)
            {
                return "Bouncy";
            }
            if (type == SongTypes.BouncyLoop2)
            {
                return "Grunge";
            }
            if (type == SongTypes.DeepSpace1)
            {
                return "Space";
            }
            if (type == SongTypes.DeepSpace2)
            {
                return "Funky";
            }
            if (type == SongTypes.SpaceLoop1)
            {
                return "Chilly";
            }
            return "Smooth";
        }

        //Playing Interface
        public static void DrawPlayingInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, GoalManager goalManager)
        {
            string currentScore;
            if (goalManager.DrawNumberScrollEffect)
            {
                currentScore = goalManager.NumberScrollScoreToDraw;
            }
            else
            {
                currentScore = String.Format("{0}", goalManager.GameScore);
            }
            string currentMultiplier = String.Format("x{0}", goalManager.ScoreMulitplier);
            string currentStreak = String.Format("+{0}", goalManager.Streak);
            string timeRemaining = String.Format("{0}", GameTimer.GetElapsedGameTime());

            Vector2 currentScoreOrigin = pixelGlowFont.MeasureString(currentScore) / 2;
            Vector2 multiplierOrigin = pixelGlowFont.MeasureString(currentMultiplier);
            Vector2 currentStreakOrigin = pixelGlowFont.MeasureString(currentStreak);

            spriteBatch.DrawString(pixelGlowFont, currentScore, new Vector2(1280 / 2, 30), Color.White, 0f, currentScoreOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(pixelGlowFont, currentMultiplier, new Vector2(1260, 720), Color.White, 0f, multiplierOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(pixelGlowFont, currentStreak, new Vector2(1260, 60), Color.White, 0f, currentStreakOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(pixelGlowFont, timeRemaining, new Vector2(10, 664), Color.White);
        }

        //Game Over Interface
        private const string GAME_OVER = "Game Over!";
        private const string QUIT_RESTART_TEXT = "(Q)uit | (R)etry | (M)enu";
        private static string gameOverTimer = String.Format("{0}", String.Format("{0:00}:{1:00}", new TimeSpan(0, 0, 0, 0).Minutes, new TimeSpan(0, 0, 0, 0).Seconds));

        public static void DrawGameEndInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont, GoalManager goalManager)
        {
            Vector2 gameOverOrigin = pixelFont.MeasureString(GAME_OVER) / 2;
            Vector2 quitRestartOrigin = pixelFont.MeasureString(QUIT_RESTART_TEXT) / 2;

            spriteBatch.DrawString(pixelFont, GAME_OVER, new Vector2(1280 / 2, 340), Color.OrangeRed, 0, gameOverOrigin, 1f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(pixelFont, QUIT_RESTART_TEXT, new Vector2(1280/2, 420), Color.White, 0, quitRestartOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(pixelGlowFont, gameOverTimer, new Vector2(10, 664), Color.White);
            spriteBatch.DrawString(pixelFont, "High Scores", new Vector2(10, 30), Color.Gold);
            spriteBatch.DrawString(pixelFont, "Player", new Vector2(76, 50), Color.DarkOrange);
            spriteBatch.DrawString(pixelFont, "Score", new Vector2(290, 50), Color.DarkOrange);
            spriteBatch.DrawString(pixelFont, "Top Streak", new Vector2(440, 50), Color.DarkOrange);
            spriteBatch.DrawString(pixelFont, "Multiplier", new Vector2(625, 50), Color.DarkOrange);
        }

        //Paused Interface
        const string PAUSED = "Paused!";
        public static void DrawPausedInterface(SpriteBatch spriteBatch, SpriteFont pixelFont, SpriteFont pixelGlowFont)
        {
            Vector2 pausedOrigin = pixelFont.MeasureString(PAUSED) / 2;
            Vector2 quitRestartOrigin = pixelFont.MeasureString(QUIT_RESTART_TEXT)/2;
            spriteBatch.DrawString(pixelFont, PAUSED, new Vector2(1280 / 2, 720 / 2), Color.DarkOrange, 0, pausedOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(pixelFont, QUIT_RESTART_TEXT, new Vector2(1280/2, 360 + 40), Color.White, 0f, quitRestartOrigin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}
