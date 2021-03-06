﻿using System;
using System.Globalization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class PlayingScreenState
    {
        private const double NUMBER_SCROLL_EFFECT_TIME = 500;
        private static double s_numberScrollEffectTimer;
        private static readonly StringBuilder s_numberScrollStringEffects = new StringBuilder();
        private static readonly Random s_rand = new Random();
        private const double EFFECT_TIME = 1500;
        private static double s_effectTimer;

        private static double s_hoopParticleTimer;
        private static int s_hoopDirection;

        public static void Update(GameTime gameTime)
        {
            if (InterfaceSettings.HighScoreManager.BestScore() <= InterfaceSettings.ArcadeHighScoreManager.BestScore())
            {
                Unlocks.CurrentBestScore = InterfaceSettings.ArcadeHighScoreManager.BestScore();
            }
            else
            {
                Unlocks.CurrentBestScore = InterfaceSettings.HighScoreManager.BestScore();
            }
            Unlocks.UnlocksCalculated = false;
            Unlocks.IsNewUnlockedBalls = false;
            Unlocks.IsNewHighScore = false;
            Unlocks.IsHighScoreSoundEffectPlayed = false;

            Unlocks.UnlockDisplayTimer = 0;
            Unlocks.NewHighScoreFadeOutTimer = 0;
            Unlocks.ShowNewHighScoreTimer = 0;
            Unlocks.NewHighScoreTimer = 0;

            Unlocks.UnlockedBallCounter = 0;
            Unlocks.HighScoreFadeValue = 1;

            Unlocks.UnlockedBalls.Clear();

            BasketballManager.SelectedBasketball.Update(gameTime);
            
            Screen.HandlePlayerInput();
            Screen.HandleBasketballPosition();
            
            Vector2 basketballCenter = InterfaceSettings.BasketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
            Rectangle basketballCenterRectangle = new Rectangle((int)basketballCenter.X - 8, (int)basketballCenter.Y - 8, 16, 16);
            InterfaceSettings.GoalManager.UpdateGoalScored(gameTime, Screen.Camera, basketballCenterRectangle, Sounds.BasketScoredSoundEffect, Sounds.StreakWubSoundEffect, BasketballManager.SelectedBasketball.BallEmitter, InterfaceSettings.StarField, InterfaceSettings.GameSettings);

            if (PhysicalWorld.BackboardCollisionHappened)
            {
                PhysicalWorld.GlowBackboard(gameTime);
            }

            if (PhysicalWorld.LeftRimCollisionHappened)
            {
                PhysicalWorld.GlowLeftRim(gameTime);
            }

            if (PhysicalWorld.RightRimCollisionHappened)
            {
                PhysicalWorld.GlowRightRim(gameTime);
            }

            if (InterfaceSettings.GoalManager.DrawNumberScrollEffect)
            {
                s_numberScrollStringEffects.Clear();
                for (int i = 0; i < InterfaceSettings.GoalManager.GameScore.ToString(CultureInfo.InvariantCulture).Length; i++)
                {
                    int number = s_rand.Next(0, 9);
                    s_numberScrollStringEffects.Append(number);
                }
                InterfaceSettings.GoalManager.NumberScrollScoreToDraw = s_numberScrollStringEffects.ToString();
                s_numberScrollEffectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (NUMBER_SCROLL_EFFECT_TIME < s_numberScrollEffectTimer)
                {
                    InterfaceSettings.GoalManager.DrawNumberScrollEffect = false;
                    s_numberScrollEffectTimer = 0;
                }
            }

            if (InterfaceSettings.GoalManager.Streak < 3)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(new Color(255, 120, 0));
            }
            else if (InterfaceSettings.GoalManager.Streak >= 3 && InterfaceSettings.GoalManager.Streak < 6)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.Plum);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 6 && InterfaceSettings.GoalManager.Streak < 9)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.Lime);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 9 && InterfaceSettings.GoalManager.Streak < 15)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.DarkRed);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 15)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.BlueViolet);
            }
            else
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(new Color(255, 120, 0));
            }

            s_hoopParticleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (s_hoopParticleTimer <= 1500)
            {
                if (s_hoopDirection == 0)
                {
                    float amount = MathHelper.Clamp((float)s_hoopParticleTimer / 1500, 0, 1);
                    Vector3 start = new Vector3(72, 35, 0);
                    Vector3 end = new Vector3(62, 35, 0);
                    Vector3 result = Vector3.Lerp(start, end, amount);
                    ParticleSystems.TrailParticleSystemWrapper.Emitter.PositionData.Position = result;
                }
                else
                {
                    float amount = MathHelper.Clamp((float)s_hoopParticleTimer / 1500, 0, 1);
                    Vector3 start = new Vector3(62, 35, 0);
                    Vector3 end = new Vector3(72, 35, 0);
                    Vector3 result = Vector3.Lerp(start, end, amount);
                    ParticleSystems.TrailParticleSystemWrapper.Emitter.PositionData.Position = result;
                }
            }
            else
            {
                s_hoopParticleTimer = 0;
                if (s_hoopDirection == 0)
                {
                    s_hoopDirection = 1;
                }
                else
                {
                    s_hoopDirection = 0;
                }
            }

            ParticleSystems.ParticleSystemManager.SetCameraPositionForAllParticleSystems(ParticleSystems._3DCamera.Position);
            ParticleSystems.ParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(ParticleSystems.WorldMatrix, ParticleSystems.ViewMatrix, ParticleSystems.ProjectionMatrix);
            ParticleSystems.ParticleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (GameTimer.GetElapsedTimeSpan() >= new TimeSpan(0, 0, 2, 0))
            {
                GameTimer.StopGameTimer();
                if (InterfaceSettings.BasketballManager.BasketballBody.Awake == false)
                {
                    InterfaceSettings.HighScoreManager.SaveHighScore(InterfaceSettings.PlayerOptions.PlayerName, InterfaceSettings.GoalManager.GameScore, InterfaceSettings.GoalManager.TopStreak, InterfaceSettings.GoalManager.ScoreMulitplier);
                    ParticleSystems.BallParticleSystemManager.DestroyAndRemoveAllParticleSystems();
                    Unlocks.HighScoresPlayers.Clear();
                    Unlocks.HighScoresScore.Clear();
                    Unlocks.HighScoresStreak.Clear();
                    Unlocks.HighScoresMultiplier.Clear();
                    if (BasketballManager.SelectedBasketball.BallEmitter != null)
                    {
                        BasketballManager.SelectedBasketball.BallEmitter.CleanUpParticles();
                    }
                    if (Unlocks.CurrentBestScore < InterfaceSettings.HighScoreManager.BestScore() && !Unlocks.UnlocksCalculated)
                    {
                        Unlocks.IsNewHighScore = true;
                    }
                    GameState.States = GameState.GameStates.GameEnd;
                }
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 backboardPosition = PhysicalWorld.BackboardBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 backboardOrigin = new Vector2(Textures.Backboard1.Width, Textures.Backboard1.Height) / 2;

            Vector2 leftRimOrigin = new Vector2(Textures.LeftRim1.Width, Textures.LeftRim1.Height) / 2;
            Vector2 rightRimOrigin2 = new Vector2(Textures.RightRim1.Width, Textures.RightRim1.Height) / 2;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            if (InterfaceSettings.BasketballManager.BasketballBody.Awake == false)
            {
                for (float t = 0; t < 5f; t += .01f)
                {
                    const float steps = 1 / 60f;
                    Vector2 stepVelocity = (InterfaceSettings.PointingAt * InterfaceSettings.Force * steps);
                    Vector2 gravity = (ConvertUnits.ToDisplayUnits(new Vector2(0, 25f))) * steps * steps;
                    Vector2 position = InterfaceSettings.BasketballLocation + t * stepVelocity + .5f * (t * t + t) * gravity;
                    spriteBatch.Draw(Textures.Twopxsolidstar, position, Color.MediumPurple);
                }
            }

            spriteBatch.End();
            
            if (BasketballManager.SelectedBasketball.BallEmitterType == ParticleEmitterTypes.Explosion)
            {
                ParticleSystems.BallParticleSystemManager.DrawAllParticleSystems();
            }
            else
            {
                BasketballManager.SelectedBasketball.DrawEmitter(gameTime, spriteBatch);
            }

            //draw objects which contain a body that can have forces applied to it
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            //draw basketball
            spriteBatch.Draw(BasketballManager.SelectedBasketball.BasketballTexture, (InterfaceSettings.BasketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.SelectedBasketball.Source, Color.White, InterfaceSettings.BasketballManager.BasketballBody.Rotation, BasketballManager.SelectedBasketball.Origin, 1f, SpriteEffects.None, 0f);
            //draw backboard
            if (InterfaceSettings.GoalManager.Streak < 3)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 3 && InterfaceSettings.GoalManager.Streak < 6)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard2Glow : Textures.Backboard2, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim2Glow : Textures.LeftRim2, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim2Glow : Textures.RightRim2, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 6 && InterfaceSettings.GoalManager.Streak < 9)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard3Glow : Textures.Backboard3, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim3Glow : Textures.LeftRim3, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim3Glow : Textures.RightRim3, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 9 && InterfaceSettings.GoalManager.Streak < 15)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard4Glow : Textures.Backboard4, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim4Glow : Textures.LeftRim4, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim4Glow : Textures.RightRim4, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 15)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard5Glow : Textures.Backboard5, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim5Glow : Textures.LeftRim5, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim5Glow : Textures.RightRim5, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }

            GameInterface.DrawPlayingInterface(spriteBatch, Fonts.SpriteFont, Fonts.PixelScoreGlow, InterfaceSettings.GoalManager);

            if (InterfaceSettings.GoalManager.DrawSwish)
            {
                s_effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                const string swishText = "SWISH!";
                Vector2 swishOrigin = Fonts.PixelScoreGlow.MeasureString(swishText) / 2;
                spriteBatch.DrawString(Fonts.PixelScoreGlow, swishText, new Vector2(1280 / 2, 70), Color.White, 0f, swishOrigin, 1.0f, SpriteEffects.None, 1.0f);
                if (EFFECT_TIME < s_effectTimer)
                {
                    s_effectTimer = 0;
                    InterfaceSettings.GoalManager.DrawSwish = false;
                }
            }

            if (InterfaceSettings.GoalManager.DrawCleanShot)
            {
                s_effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                const string niceShotText = "Nice Shot!";
                Vector2 niceShotOrigin = Fonts.PixelScoreGlow.MeasureString(niceShotText) / 2;
                spriteBatch.DrawString(Fonts.PixelScoreGlow, niceShotText, new Vector2(1280 / 2, 70), Color.White, 0f, niceShotOrigin, 1.0f, SpriteEffects.None, 1.0f);
                if (EFFECT_TIME < s_effectTimer)
                {
                    s_effectTimer = 0;
                    InterfaceSettings.GoalManager.DrawCleanShot = false;
                }
            }

            if (!String.IsNullOrEmpty(InterfaceSettings.GoalManager.DrawStreakMessage))
            {
                Vector2 streakCenter = Fonts.StreakText.MeasureString(InterfaceSettings.GoalManager.DrawStreakMessage) / 2;
                spriteBatch.DrawString(Fonts.StreakText, InterfaceSettings.GoalManager.DrawStreakMessage, new Vector2(1280 / 2, 696), Color.White, 0f, streakCenter, 1.0f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            ParticleSystems.ParticleSystemManager.DrawAllParticleSystems();
        }
    }
}
