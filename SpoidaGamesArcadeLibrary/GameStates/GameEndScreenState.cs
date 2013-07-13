using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class GameEndScreenState
    {
        private const double NEW_HIGH_SCORE_DISPLAY_TIME = 2000;

        public static void Update(GameTime gameTime)
        {
            if (Unlocks.CurrentBestScore < InterfaceSettings.HighScoreManager.BestScore() && !Unlocks.UnlocksCalculated)
            {
                foreach (Basketball basketball in BasketballManager.basketballList)
                {
                    if (basketball.BasketballUnlockScore <= Unlocks.CurrentBestScore)
                    {
                        Unlocks.OldBasketballs.Add(basketball);
                    }

                    if (basketball.BasketballUnlockScore <= InterfaceSettings.HighScoreManager.BestScore())
                    {
                        Unlocks.NewBasketballs.Add(basketball);
                    }
                }

                var unlocks = Unlocks.NewBasketballs.Where(b => Unlocks.OldBasketballs.All(b2 => b2.BasketballName != b.BasketballName));

                var basketballs = unlocks as Basketball[] ?? unlocks.ToArray();
                if (basketballs.Any())
                {
                    foreach (Basketball ball in basketballs)
                    {
                        Unlocks.UnlockedBalls.Add(ball);
                    }
                    Unlocks.IsNewUnlockedBalls = true;
                }
                else
                {
                    Unlocks.UnlockedBalls.Clear();
                    Unlocks.IsNewUnlockedBalls = false;
                }
                Unlocks.UnlocksCalculated = true;
            }
            else
            {
                Unlocks.UnlocksCalculated = true;
            }

            if (!Unlocks.HighScoresLoaded)
            {
                int count = 0;
                foreach (HighScore highScore in InterfaceSettings.HighScoreManager.HighScores)
                {
                    if (count != 9)
                    {
                        Unlocks.HighScoresPlayers.AppendLine(String.Format(" {0}. {1}", (count + 1),
                                                                     highScore.CurrentPlayerName));
                    }
                    else
                    {
                        Unlocks.HighScoresPlayers.AppendLine(String.Format("{0}. {1}", (count + 1),
                                                                     highScore.CurrentPlayerName));
                    }
                    Unlocks.HighScoresScore.AppendLine(String.Format("{0}", highScore.PlayerScore));
                    Unlocks.HighScoresStreak.AppendLine(String.Format("{0}", highScore.PlayerTopStreak));
                    Unlocks.HighScoresMultiplier.AppendLine(String.Format("{0}", highScore.PlayerMultiplier));
                    count++;
                }

                Unlocks.HighScoresLoaded = true;
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.DrawString(Fonts.SpriteFont, Unlocks.HighScoresPlayers, new Vector2(10, 74), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, Unlocks.HighScoresScore, new Vector2(290, 74), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, Unlocks.HighScoresStreak, new Vector2(440, 74), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, Unlocks.HighScoresMultiplier, new Vector2(625, 74), Color.White);
            GameInterface.DrawGameEndInterface(spriteBatch, Fonts.SpriteFont, Fonts.PixelScoreGlow, InterfaceSettings.GoalManager);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            if (Unlocks.IsNewHighScore)
            {
                Unlocks.NewHighScoreTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                const string newHighScore = "New High Score!";
                Vector2 newHighScoreOrigin = Fonts.PixelScoreGlow.MeasureString(newHighScore) / 2;
                float amountToMove = MathHelper.Clamp((float)Unlocks.NewHighScoreTimer / 2000, 0, 1);
                float amountToMoveValue = MathHelper.Lerp(-1000, 1280 / 2, amountToMove);
                if (Unlocks.NewHighScoreTimer >= NEW_HIGH_SCORE_DISPLAY_TIME)
                {
                    Unlocks.NewHighScoreFadeOutTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    float amountToFade = MathHelper.Clamp((float)Unlocks.NewHighScoreFadeOutTimer / 2000, 0, 1);
                    Unlocks.HighScoreFadeValue = MathHelper.Lerp(1, 0, amountToFade);
                }
                spriteBatch.DrawString(Fonts.PixelScoreGlow, newHighScore, new Vector2(amountToMoveValue, 380), new Color(255, 255, 255, Unlocks.HighScoreFadeValue), 0f, newHighScoreOrigin, 1.0f, SpriteEffects.None, 1.0f);
                if (!Unlocks.IsHighScoreSoundEffectPlayed)
                {
                    Sounds.HighScoreSwooshSoundEffect.Play((float)ComputerSettings.SoundEffectVolumeSetting / 10, 0f, 0f);
                    Unlocks.IsHighScoreSoundEffectPlayed = true;
                }

                if (Unlocks.NewHighScoreTimer >= 4000)
                {
                    Unlocks.ShowNewHighScoreTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    float amountToFadeInHighScore = MathHelper.Clamp((float)Unlocks.ShowNewHighScoreTimer / 1000, 0, 1);
                    float amountToFadeInHighScoreValue = MathHelper.Lerp(0, 1, amountToFadeInHighScore);
                    string finalScore = String.Format("Final Score: {0:n0}!", InterfaceSettings.GoalManager.GameScore);
                    Vector2 finalScoreOrigin = Fonts.PixelScoreGlow.MeasureString(finalScore) / 2;
                    spriteBatch.DrawString(Fonts.PixelScoreGlow, finalScore, new Vector2(1280 / 2, 380), new Color(255, 255, 255, amountToFadeInHighScoreValue), 0, finalScoreOrigin, 1f, SpriteEffects.None, 1.0f);
                    Unlocks.NewHighScoreTimer = 4001;
                }
            }
            else if (gameTime.ElapsedGameTime.TotalMilliseconds < 20 && !Unlocks.IsNewHighScore)
            {
                string finalScore = String.Format("Final Score: {0:n0}!", InterfaceSettings.GoalManager.GameScore);
                Vector2 finalScoreOrigin = Fonts.PixelScoreGlow.MeasureString(finalScore) / 2;
                spriteBatch.DrawString(Fonts.PixelScoreGlow, finalScore, new Vector2(1280 / 2, 380), Color.White, 0, finalScoreOrigin, 1f, SpriteEffects.None, 1.0f);
            }

            if (Unlocks.IsNewUnlockedBalls)
            {
                const string unlockedText = "Basketball Unlocked!";
                Vector2 unlockedTextOrigin = Fonts.SpriteFont.MeasureString(unlockedText) / 2;
                if (Unlocks.UnlockedBallCounter < Unlocks.UnlockedBalls.Count)
                {
                    Unlocks.UnlockDisplayTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    float amountToFadeInBasketball = MathHelper.Clamp((float)Unlocks.UnlockDisplayTimer / 1500, 0, 1);
                    float amountToFadeInBasketballValue = MathHelper.Lerp(0, 1, amountToFadeInBasketball);
                    spriteBatch.DrawString(Fonts.SpriteFont, unlockedText, new Vector2(1280 / 2, 500), new Color(255, 91, 71, amountToFadeInBasketballValue), 0f, unlockedTextOrigin, 1f, SpriteEffects.None, 1.0f);
                    Texture2D basketballTexture = Unlocks.UnlockedBalls[Unlocks.UnlockedBallCounter].BasketballTexture;
                    Vector2 basketballOrigin = Unlocks.UnlockedBalls[Unlocks.UnlockedBallCounter].Origin;
                    spriteBatch.Draw(basketballTexture, new Vector2(1280 / 2, 540), null, new Color(255, 255, 255, amountToFadeInBasketballValue), 0f, basketballOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    if (!Unlocks.IsUnlockSoundEffectPlayed)
                    {
                        Sounds.UnlockedSoundEffect.Play((float)ComputerSettings.SoundEffectVolumeSetting / 10, 0f, 0f);
                        Unlocks.IsUnlockSoundEffectPlayed = true;
                    }
                    if (Unlocks.UnlockDisplayTimer >= 3000)
                    {
                        Unlocks.UnlockDisplayTimer = 0;
                        Unlocks.UnlockedBallCounter++;
                        Unlocks.IsUnlockSoundEffectPlayed = false;
                    }
                }
            }

            spriteBatch.End();
        }
    }
}
