﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class ArcadeModeScreenState
    {
        private static readonly Random s_random = new Random();
        private static readonly List<ArcadeBasketball> s_activeBasketballs = new List<ArcadeBasketball>();
        private static readonly List<ArcadeBasketball> s_activeBasketballsToRemove = new List<ArcadeBasketball>();
        private static int s_basketballSpawnTimer = 660;
        private static int s_basketballTimer;
        public static bool ReadyToFire = true;
        private static bool s_lastShotMade;
        private static readonly StringBuilder s_powerUpsToDraw = new StringBuilder();

        private const double NUMBER_SCROLL_EFFECT_TIME = 500;
        private static double s_numberScrollEffectTimer;
        private static readonly StringBuilder s_numberScrollStringEffects = new StringBuilder();

        private static double s_hoopParticleTimer;
        private static int s_hoopDirection;
        
        private static readonly Vector2 s_backboardPosition = PhysicalWorld.BackboardBody.Position * PhysicalWorld.MetersInPixels;
        private static readonly Vector2 s_backboardOrigin = new Vector2(Textures.Backboard1.Width, Textures.Backboard1.Height) / 2;

        private static readonly Vector2 s_leftRimOrigin = new Vector2(Textures.LeftRim1.Width, Textures.LeftRim1.Height) / 2;
        private static readonly Vector2 s_rightRimOrigin2 = new Vector2(Textures.RightRim1.Width, Textures.RightRim1.Height) / 2;

        public static Basketball PlayerSelectedBall { get; set; }

        private static float s_laserSightLength;
        private static bool s_showLaserSight;
        private static string s_laserSightText;
        private static double s_laserSightRemaining;

        public static bool ShowDoubleScore {get;set;}
        private static string s_doubleScoreText;
        private static double s_doubleScoreRemaining;

        private static bool s_showRapidFire;
        private static string s_rapidFireText;
        private static double s_rapidFireRemaining;

        private static bool s_showHomingBall;
        private static string s_homingBallText;
        private static int s_homingBallInventoryRemaining;

        private static bool s_isHomingBallEngaged;
        private static bool s_readyToReduceHomingBallInventory;

        private static Vector2 s_randomRapidFireVector = Vector2.Zero;
        private static bool s_randomRapidFireVectorLoaded;

        public static double LaserSightTextTimer { get; set; }
        public static double MulitplierTextTimer { get; set; }
        public static double RapidFireTextTimer { get; set; }
        public static double HomingBallTextTimer { get; set; }

        public static bool DrawLaserSightText { get; set; }
        public static bool DrawMultiplierText { get; set; }
        public static bool DrawRapidFireText { get; set; }
        public static bool DrawHomingBallText { get; set; }

        private const double TEXT_POWER_UP_TIME = 2000;

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

            if (ReadyToFire)
            {
                ReadyToFire = false;
                SpawnNewBasketball();
                s_lastShotMade = false;
            }

            if (!ReadyToFire && s_lastShotMade)
            {
                s_basketballTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (s_basketballTimer >= s_basketballSpawnTimer)
                {
                    s_basketballTimer = 0;
                    ReadyToFire = true;
                }
            }

            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.Update(gameTime);
                if (basketball.BasketballBody.Position.Y > 720/PhysicalWorld.MetersInPixels)
                {
                    PhysicalWorld.World.RemoveBody(basketball.BasketballBody);
                    s_activeBasketballsToRemove.Add(basketball);
                    basketball.DestroyParticles();
                    if (basketball.HasBallScored == false)
                    {
                        ArcadeGoalManager.Streak = 0;
                    }
                }
            }

            s_activeBasketballs.RemoveAll(x => s_activeBasketballsToRemove.Contains(x));

            HandlePlayerInput();

            if (!s_isHomingBallEngaged && s_homingBallInventoryRemaining > 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F))
                {
                    s_isHomingBallEngaged = true;
                    s_readyToReduceHomingBallInventory = true;
                    SoundManager.PlaySoundEffect(Sounds.EngageHomingBall, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }
            }

            ArcadeGoalManager.Update(gameTime);
            s_powerUpsToDraw.Clear();
            foreach (KeyValuePair<int, PowerUp> powerUp in ArcadeGoalManager.ActivePowerUps)
            {
                powerUp.Value.TimeRemaining -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (powerUp.Value.TimeRemaining <= 0)
                {
                    if (powerUp.Value.PowerUpName != "Homing Ball")
                    {
                        powerUp.Value.IsActive = false;
                    }
                }

                switch (powerUp.Value.PowerUpName)
                {
                    case "Laser Sight":
                        if (powerUp.Value.IsActive)
                        {
                            if (s_isHomingBallEngaged)
                            {
                                s_laserSightLength = 120f;
                            }
                            else
                            {
                                s_laserSightLength = 80f;
                            }
                            s_showLaserSight = true;
                            s_laserSightRemaining = powerUp.Value.TimeRemaining/1000;
                            s_laserSightText = powerUp.Value.PowerUpName;
                        }
                        else
                        {
                            if (s_isHomingBallEngaged)
                            {
                                s_laserSightLength = 120f;
                            }
                            else
                            {
                                s_laserSightLength = 5f;
                            }
                            s_showLaserSight = false;
                        }
                        break;
                    case "Homing Ball":
                        if (powerUp.Value.IsActive)
                        {
                            if (s_readyToReduceHomingBallInventory)
                            {
                                powerUp.Value.AvailableInventory--;
                                s_readyToReduceHomingBallInventory = false;
                            }
                            s_showHomingBall = true;
                            s_homingBallInventoryRemaining = powerUp.Value.AvailableInventory;
                            s_homingBallText = powerUp.Value.PowerUpName;
                        }
                        break;
                    case "2x Multiplier":
                        if (powerUp.Value.IsActive)
                        {
                            ShowDoubleScore = true;
                            s_doubleScoreRemaining = powerUp.Value.TimeRemaining/1000;
                            s_doubleScoreText = powerUp.Value.PowerUpName;
                        }
                        else
                        {
                            ShowDoubleScore = false;
                        }
                        break;
                    case "Rapid Fire":
                        if (powerUp.Value.IsActive)
                        {
                            s_basketballSpawnTimer = 250;
                            s_showRapidFire = true;
                            s_rapidFireRemaining = powerUp.Value.TimeRemaining/1000;
                            s_rapidFireText = powerUp.Value.PowerUpName;
                            if (!s_randomRapidFireVectorLoaded)
                            {
                                if (s_activeBasketballs.Count != 0)
                                {
                                    ArcadeBasketball ball = s_activeBasketballs.LastOrDefault();
                                    if (ball != null)
                                    {
                                        if (ball.BasketballBody.Position.Y*PhysicalWorld.MetersInPixels < 310)
                                        {
                                            s_randomRapidFireVector = new Vector2((s_random.Next(400, 1200)) / PhysicalWorld.MetersInPixels,
                                                              (s_random.Next(310, 650)) / PhysicalWorld.MetersInPixels);
                                        }
                                        else
                                        {
                                            s_randomRapidFireVector = ball.BasketballBody.Position;
                                        }
                                        s_randomRapidFireVectorLoaded = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            s_randomRapidFireVector = Vector2.Zero;
                            s_randomRapidFireVectorLoaded = false;
                            s_showRapidFire = false;
                            s_basketballSpawnTimer = 660;
                        }
                        break;
                }

            }

            if (ArcadeGoalManager.DrawNumberScrollEffect)
            {
                s_numberScrollStringEffects.Clear();
                for (int i = 0; i < ArcadeGoalManager.Score.ToString(CultureInfo.InvariantCulture).Length; i++)
                {
                    int number = s_random.Next(0, 9);
                    s_numberScrollStringEffects.Append(number);
                }
                ArcadeGoalManager.NumberScrollScoreToDraw = s_numberScrollStringEffects.ToString();
                s_numberScrollEffectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (NUMBER_SCROLL_EFFECT_TIME < s_numberScrollEffectTimer)
                {
                    ArcadeGoalManager.DrawNumberScrollEffect = false;
                    s_numberScrollEffectTimer = 0;
                }
            }

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

            s_hoopParticleTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (s_hoopParticleTimer <= 1500)
            {
                if (s_hoopDirection == 0)
                {
                    float amount = MathHelper.Clamp((float) s_hoopParticleTimer/1500, 0, 1);
                    Vector3 start = new Vector3(72, 35, 0);
                    Vector3 end = new Vector3(62, 35, 0);
                    Vector3 result = Vector3.Lerp(start, end, amount);
                    ParticleSystems.TrailParticleSystemWrapper.Emitter.PositionData.Position = result;
                }
                else
                {
                    float amount = MathHelper.Clamp((float) s_hoopParticleTimer/1500, 0, 1);
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

            ParticleSystems.BallParticleSystemManager.SetCameraPositionForAllParticleSystems(ParticleSystems._3DCamera.Position);
            ParticleSystems.BallParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(ParticleSystems.WorldMatrix, ParticleSystems.ViewMatrix, ParticleSystems.ProjectionMatrix);
            ParticleSystems.BallParticleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (Screen.Camera.Shaking)
            {
                Screen.Camera.ShakeCamera(gameTime);
            }
            else
            {
                Screen.Camera.Position = Vector2.Zero;
            }

            if (GameTimer.GetElapsedTimeSpan() >= new TimeSpan(0, 0, 3, 0))
            {
                GameTimer.StopGameTimer();
                InterfaceSettings.ArcadeHighScoreManager.SaveHighScore(InterfaceSettings.PlayerOptions.PlayerName, ArcadeGoalManager.Score, 0, 0);
                ParticleSystems.BallParticleSystemManager.DestroyAndRemoveAllParticleSystems();
                Unlocks.HighScoresPlayers.Clear();
                Unlocks.HighScoresScore.Clear();
                Unlocks.HighScoresStreak.Clear();
                Unlocks.HighScoresMultiplier.Clear();
                CleanUpGameState();
                if (Unlocks.CurrentBestScore < InterfaceSettings.ArcadeHighScoreManager.BestScore() && !Unlocks.UnlocksCalculated)
                {
                    Unlocks.IsNewHighScore = true;
                }
                GameState.States = GameState.GameStates.GameEnd;
            }
        }

        private static bool s_drawEngageHomingTip = true;
        private static double s_drawTimeRemaining;
        private static double s_drawFadeTimer;
        private const string ENGAGE_TEXT = "Press F to engage homing ball!";
        private static readonly Vector2 s_engageTextOrigin = Fonts.PixelScoreGlow.MeasureString(ENGAGE_TEXT)/2;
        private static float s_engageFade = 1;

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            
            if (s_drawEngageHomingTip)
            {
                s_drawTimeRemaining += gameTime.ElapsedGameTime.TotalMilliseconds;
                
                if (s_drawTimeRemaining >= 1500)
                {
                    s_drawFadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    float amount = MathHelper.Clamp((float)s_drawFadeTimer / 2500, 0, 1);
                    s_engageFade = MathHelper.Lerp(1, 0, amount);
                }
                spriteBatch.DrawString(Fonts.PixelScoreGlow, ENGAGE_TEXT, new Vector2(1280 / 2, 500), new Color(255, 255, 255, s_engageFade), 0f, s_engageTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                if (s_drawTimeRemaining >= 4000)
                {
                    s_drawEngageHomingTip = false;
                }
            }

            if (DrawLaserSightText)
            {
                const string laserSightText = "Laser Sight Activated!";
                Vector2 laserSightTextOrign = Fonts.PixelScoreGlow.MeasureString(laserSightText) / 2;

                LaserSightTextTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (LaserSightTextTimer <= TEXT_POWER_UP_TIME)
                {
                    float amount = MathHelper.Clamp((float)LaserSightTextTimer / 2000, 0, 1);
                    float fade = MathHelper.Lerp(1, 0, amount);
                    spriteBatch.DrawString(Fonts.PixelScoreGlow, laserSightText, new Vector2(1280 / 2, 500), new Color(255, 255, 255, fade), 0f, laserSightTextOrign, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    DrawLaserSightText = false;
                }
            }

            if (DrawMultiplierText)
            {
                const string multiplierText = "2x Multiplier Activated!";
                Vector2 multiplierTextOrigin = Fonts.PixelScoreGlow.MeasureString(multiplierText) / 2;

                MulitplierTextTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (MulitplierTextTimer <= TEXT_POWER_UP_TIME)
                {
                    float amount = MathHelper.Clamp((float)MulitplierTextTimer / 2000, 0, 1);
                    float fade = MathHelper.Lerp(1, 0, amount);
                    spriteBatch.DrawString(Fonts.PixelScoreGlow, multiplierText, new Vector2(1280 / 2, 500), new Color(255, 255, 255, fade), 0f, multiplierTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    DrawMultiplierText = false;
                }
            }

            if (DrawRapidFireText)
            {
                const string rapidFireText = "Rapid Fire Activated!";
                Vector2 rapidFireTextOrigin = Fonts.PixelScoreGlow.MeasureString(rapidFireText) / 2;

                RapidFireTextTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (RapidFireTextTimer <= TEXT_POWER_UP_TIME)
                {
                    float amount = MathHelper.Clamp((float)RapidFireTextTimer / 2000, 0, 1);
                    float fade = MathHelper.Lerp(1, 0, amount);
                    spriteBatch.DrawString(Fonts.PixelScoreGlow, rapidFireText, new Vector2(1280 / 2, 500), new Color(255, 255, 255, fade), 0f, rapidFireTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    DrawRapidFireText = false;
                }
            }

            if (DrawHomingBallText)
            {
                const string homingBallAdded = "+1 Homing Ball!";
                Vector2 homingBallAddedOrigin = Fonts.PixelScoreGlow.MeasureString(homingBallAdded) / 2;

                HomingBallTextTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (HomingBallTextTimer <= TEXT_POWER_UP_TIME)
                {
                    float amount = MathHelper.Clamp((float)HomingBallTextTimer / 2000, 0, 1);
                    float fade = MathHelper.Lerp(1, 0, amount);
                    spriteBatch.DrawString(Fonts.PixelScoreGlow, homingBallAdded, new Vector2(1280 / 2, 500), new Color(255, 255, 255, fade), 0f, homingBallAddedOrigin, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    DrawHomingBallText = false;
                }
            }
            spriteBatch.End();

            DrawLaserSight(spriteBatch);

            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.DrawEmitter(gameTime, spriteBatch);
            }

            ParticleSystems.BallParticleSystemManager.DrawAllParticleSystems();

            string currentMultiplier = String.Format("x{0}", ArcadeGoalManager.Multiplier);
            string currentStreak = String.Format("+{0}", ArcadeGoalManager.Streak);

            Vector2 multiplierOrigin = Fonts.PixelScoreGlow.MeasureString(currentMultiplier);
            Vector2 currentStreakOrigin = Fonts.PixelScoreGlow.MeasureString(currentStreak);

            const string homingBallEngaged = "Homing Ball Engaged!";
            Vector2 homingBallEngagedOrigin = Fonts.SpriteFontGlow.MeasureString(homingBallEngaged) / 2;

            string timeRemaining = String.Format("{0}", GameTimer.GetElapsedGameTime());
            Vector2 timeRemainingOrigin = Fonts.PixelScoreGlow.MeasureString(timeRemaining);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.DrawString(Fonts.PixelScoreGlow, currentMultiplier, new Vector2(1080, 52), Color.White, 0f, multiplierOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.PixelScoreGlow, currentStreak, new Vector2(1260, 52), Color.White, 0f, currentStreakOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.PixelScoreGlow, timeRemaining, new Vector2(170, 52), Color.White, 0f, timeRemainingOrigin, 1.0f, SpriteEffects.None, 1.0f);

            if (s_isHomingBallEngaged)
            {
                spriteBatch.DrawString(Fonts.SpriteFontGlow, homingBallEngaged, new Vector2(1280 / 2, 60), Color.Red, 0f, homingBallEngagedOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }

            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.Draw(gameTime, spriteBatch);
            }

            if (ArcadeGoalManager.Streak < 4)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (ArcadeGoalManager.Streak >= 4 && ArcadeGoalManager.Streak < 8)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard2Glow : Textures.Backboard2, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim2Glow : Textures.LeftRim2, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim2Glow : Textures.RightRim2, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (ArcadeGoalManager.Streak >= 8 && ArcadeGoalManager.Streak < 12)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard3Glow : Textures.Backboard3, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim3Glow : Textures.LeftRim3, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim3Glow : Textures.RightRim3, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (ArcadeGoalManager.Streak >= 12 && ArcadeGoalManager.Streak < 16)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard4Glow : Textures.Backboard4, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim4Glow : Textures.LeftRim4, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim4Glow : Textures.RightRim4, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (ArcadeGoalManager.Streak >= 16)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard5Glow : Textures.Backboard5, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim5Glow : Textures.LeftRim5, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim5Glow : Textures.RightRim5, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            
            spriteBatch.DrawString(Fonts.SpriteFontGlow, s_powerUpsToDraw.ToString(), new Vector2(600, 10), Color.White);
            if (s_showLaserSight)
            {
                float amount = MathHelper.Clamp((float)s_laserSightRemaining/10, 0, 1);
                float width = MathHelper.Lerp(0, 150, amount);

                spriteBatch.Draw(Textures.Onepxsolidstar, new Rectangle(85, 700, (int)width, 16), Color.OrangeRed);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, s_laserSightText + " " + Convert.ToInt16(s_laserSightRemaining).ToString(CultureInfo.InvariantCulture), new Vector2(85, 680), Color.White);
            }

            if (ShowDoubleScore)
            {
                float amount = MathHelper.Clamp((float)s_doubleScoreRemaining / 10, 0, 1);
                float width = MathHelper.Lerp(0, 150, amount);

                spriteBatch.Draw(Textures.Onepxsolidstar, new Rectangle(405, 700, (int)width, 16), Color.OrangeRed);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, s_doubleScoreText + " " + Convert.ToInt16(s_doubleScoreRemaining).ToString(CultureInfo.InvariantCulture), new Vector2(405, 680), Color.White);
            }

            if (s_showRapidFire)
            {
                float amount = MathHelper.Clamp((float)s_rapidFireRemaining / 10, 0, 1);
                float width = MathHelper.Lerp(0, 150, amount);

                spriteBatch.Draw(Textures.Onepxsolidstar, new Rectangle(725, 700, (int)width, 16), Color.OrangeRed);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, s_rapidFireText + " " + Convert.ToInt16(s_rapidFireRemaining).ToString(CultureInfo.InvariantCulture), new Vector2(725, 680), Color.White);
            }

            if (s_showHomingBall)
            {
                for (int x = 0; x < s_homingBallInventoryRemaining; x++)
                {
                    spriteBatch.Draw(Textures.Onepxsolidstar, new Rectangle(1045 + x * 10, 700, 6, 16), Color.OrangeRed);
                }
                spriteBatch.DrawString(Fonts.SpriteFontGlow, s_homingBallText + " - " + s_homingBallInventoryRemaining.ToString(CultureInfo.InvariantCulture), new Vector2(1045, 680), Color.White);
            }

            string currentScore;
            if (ArcadeGoalManager.DrawNumberScrollEffect)
            {
                currentScore = ArcadeGoalManager.NumberScrollScoreToDraw;
            }
            else
            {
                currentScore = String.Format("{0}", ArcadeGoalManager.Score);
            }
            Vector2 currentScoreOrigin = Fonts.PixelScoreGlow.MeasureString(currentScore) / 2;
            spriteBatch.DrawString(Fonts.PixelScoreGlow, currentScore, new Vector2(1280 / 2, 22), Color.White, 0f, currentScoreOrigin, 1.0f, SpriteEffects.None, 1.0f);

            spriteBatch.End();

            ParticleSystems.ParticleSystemManager.DrawAllParticleSystems();
        }
        
        public static void SpawnNewBasketball()
        {
            ArcadeBasketball newBall = new ArcadeBasketball(PlayerSelectedBall.BasketballTexture, PlayerSelectedBall.FrameList, PlayerSelectedBall.BallEmitterType);
            if (newBall.BallEmitterType == ParticleEmitterTypes.Explosion)
            {
                newBall.ParticleWrapper = ParticleSystems.AddSpriteSystemToManager(ParticleEmitterTypes.Explosion);
            }
            if (s_showRapidFire)
            {
                newBall.BasketballBody.Position = s_randomRapidFireVector;
                newBall.BasketballBody.Awake = false;
                newBall.HasBallScored = false;
            }
            else
            {
                newBall.BasketballBody.Position = new Vector2((s_random.Next(400, 1200)) / PhysicalWorld.MetersInPixels,
                                                              (s_random.Next(310, 650)) / PhysicalWorld.MetersInPixels);
                newBall.BasketballBody.Awake = false;
                newBall.HasBallScored = false;
            }

            s_activeBasketballs.Add(newBall);
        }

        private static double CalculateProjectileFiring()
        {
            if (s_activeBasketballs.Count != 0)
            {
                ArcadeBasketball ball = s_activeBasketballs.LastOrDefault();
                if (ball != null)
                {
                    Vector2 ballLocation = ball.BasketballBody.Position*PhysicalWorld.MetersInPixels;
                    Vector2 hoopLocation = new Vector2(95, 246);

                    double x = -(ballLocation.X - hoopLocation.X);
                    double y = (ballLocation.Y - hoopLocation.Y);
                    const double v = (6/10f)*2800;
                    double g = ConvertUnits.ToDisplayUnits(new Vector2(0, 25)).Y;
                    double sqrt = (v*v*v*v) - (g*(g*(x*x) + 2*y*(v*v)));
                    sqrt = Math.Sqrt(sqrt);
                    return Math.Atan(((v*v) + sqrt)/(g*x));
                }
            }
            return 0;
        }

        private static void HandlePlayerInput()
        {
            MouseState state = Screen.Input.GetMouse().GetState();

            if (s_activeBasketballs.Count != 0)
            {
                ArcadeBasketball ball = s_activeBasketballs.LastOrDefault();
                if (ball != null)
                {
                    Vector2 ballLocation = ball.BasketballBody.Position*PhysicalWorld.MetersInPixels;
                    Vector2 mouseLocation = Vector2.Transform( new Vector2(state.X, state.Y) - new Vector2(ResolutionManager.GetViewportX, ResolutionManager.GetViewportY), Matrix.Invert(ResolutionManager.GetTransformationMatrix()));
                    double radians = MouseAngle(ballLocation, mouseLocation);
                    InterfaceSettings.PointingAt = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
                    float distance = Vector2.Distance(ballLocation, mouseLocation);
                    float modifier = MathHelper.Clamp(distance, 0, 1200);
                    InterfaceSettings.Force = (6 / 10f) * modifier + 1200;

                    if (!ball.HasBallFired)
                    {
                        if (state.LeftButton == ButtonState.Pressed)
                        {
                            ball.HasBallFired = true;
                            ball.BasketballBody.Awake = true;
                            ball.BasketballBody.BodyType = BodyType.Dynamic;
                            ball.BasketballBody.Mass = 1f;
                            if (s_isHomingBallEngaged)
                            {
                                SoundManager.PlaySoundEffect(Sounds.HomingMissleBallShot, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                            }
                            else
                            {
                                SoundManager.PlaySoundEffect(Sounds.BasketBallShotSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                            }
                            HandleShotAngle(ball.BasketballBody, InterfaceSettings.Force);
                        }
                    }
                }
            }
        }

        private static void DrawLaserSight(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            if (s_activeBasketballs.Count != 0)
            {
                ArcadeBasketball ball = s_activeBasketballs.LastOrDefault();
                if (ball != null)
                {
                    if (!ball.HasBallFired)
                    {
                        for (float t = 0; t < s_laserSightLength; t += .01f)
                        {
                            const float steps = 1/60f;
                            if (!s_isHomingBallEngaged)
                            {
                                Vector2 ballLocation = ball.BasketballBody.Position * PhysicalWorld.MetersInPixels;
                                Vector2 stepVelocity = (InterfaceSettings.PointingAt * InterfaceSettings.Force * steps);
                                Vector2 gravity = (ConvertUnits.ToDisplayUnits(new Vector2(0, 25f))) * steps * steps;
                                Vector2 position = ballLocation + t * stepVelocity + .5f * (t * t + t) * gravity;
                                spriteBatch.Draw(Textures.Twopxsolidstar, position, Color.MediumPurple);
                            }
                            else
                            {
                                const float shotForce = (6 / 10f) * 2800;
                                Vector2 ballLocation = ball.BasketballBody.Position * PhysicalWorld.MetersInPixels;
                                double angle = CalculateProjectileFiring();
                                Vector2 angleVector = new Vector2(-(float)Math.Cos(angle), (float)Math.Sin(angle));
                                Vector2 stepVelocity = angleVector*shotForce*steps;
                                Vector2 gravity = (ConvertUnits.ToDisplayUnits(new Vector2(0, 25f))) * steps * steps;
                                Vector2 position = ballLocation + t * stepVelocity + .5f * (t * t + t) * gravity;
                                spriteBatch.Draw(Textures.Twopxsolidstar, position, Color.MediumPurple);
                            }
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private static void HandleShotAngle(Body ball, float shotForce)
        {
            if (s_isHomingBallEngaged)
            {
                shotForce = (6 / 10f) * 2800;
                double angle = CalculateProjectileFiring();
                Vector2 angleVector = new Vector2(-(float)Math.Cos(angle), (float)Math.Sin(angle));
                ball.ApplyLinearImpulse(ConvertUnits.ToSimUnits(angleVector) * shotForce);
                s_isHomingBallEngaged = false;
            }
            else
            {
                ball.ApplyLinearImpulse(ConvertUnits.ToSimUnits(InterfaceSettings.PointingAt) * shotForce);
            }
            ball.ApplyAngularImpulse(.2f);
            s_lastShotMade = true;
        }

        private static double MouseAngle(Vector2 spriteLocation, Vector2 mouseLocation)
        {
            return Math.Atan2(mouseLocation.Y - (spriteLocation.Y), mouseLocation.X - (spriteLocation.X)); //this will return the angle(in radians) from sprite to mouse.
        }

        public static void CleanUpGameState()
        {
            foreach (ArcadeBasketball ball in s_activeBasketballs)
            {
                PhysicalWorld.World.RemoveBody(ball.BasketballBody);
            }
            s_activeBasketballs.Clear();
        }
    }
}
