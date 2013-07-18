using System;
using System.Collections.Generic;
using System.Linq;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Globals;
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
        private const int BASKETBALL_SPAWN_TIMER = 660;
        private static int s_basketballTimer = 0;
        private static bool s_readyToFire = true;
        private static bool s_lastShotMade;

        private static readonly Vector2 s_backboardPosition = PhysicalWorld.BackboardBody.Position * PhysicalWorld.MetersInPixels;
        private static readonly Vector2 s_backboardOrigin = new Vector2(Textures.Backboard1.Width, Textures.Backboard1.Height) / 2;

        private static readonly Vector2 s_leftRimOrigin = new Vector2(Textures.LeftRim1.Width, Textures.LeftRim1.Height) / 2;
        private static readonly Vector2 s_rightRimOrigin2 = new Vector2(Textures.RightRim1.Width, Textures.RightRim1.Height) / 2;

        public static Basketball PlayerSelectedBall { get; set; }

        public static void Update(GameTime gameTime)
        {
            PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            if (s_readyToFire)
            {
                s_readyToFire = false;
                SpawnNewBasketball();
                s_lastShotMade = false;
            }

            if (!s_readyToFire && s_lastShotMade)
            {
                s_basketballTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (s_basketballTimer >= BASKETBALL_SPAWN_TIMER)
                {
                    s_basketballTimer = 0;
                    s_readyToFire = true;
                }
            }

            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.Update(gameTime);
                if (basketball.BasketballBody.Position.Y > 720/PhysicalWorld.MetersInPixels)
                {
                    s_activeBasketballsToRemove.Add(basketball);
                }
            }

            s_activeBasketballs.RemoveAll(x => s_activeBasketballsToRemove.Contains(x));

            HandlePlayerInput();

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
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.Draw(gameTime, spriteBatch);
            }

            if (InterfaceSettings.GoalManager.Streak < 3)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 3 && InterfaceSettings.GoalManager.Streak < 6)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard2Glow : Textures.Backboard2, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim2Glow : Textures.LeftRim2, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim2Glow : Textures.RightRim2, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 6 && InterfaceSettings.GoalManager.Streak < 9)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard3Glow : Textures.Backboard3, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim3Glow : Textures.LeftRim3, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim3Glow : Textures.RightRim3, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 9 && InterfaceSettings.GoalManager.Streak < 15)
            {
                spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard4Glow : Textures.Backboard4, s_backboardPosition, null, Color.White, 0f, s_backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim4Glow : Textures.LeftRim4, new Vector2(57, 208), null, Color.White, 0f, s_leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim4Glow : Textures.RightRim4, new Vector2(188, 208), null, Color.White, 0f, s_rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.GoalManager.Streak >= 15)
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

            spriteBatch.End();
        }

        private static void SpawnNewBasketball()
        {
            ArcadeBasketball newBall = new ArcadeBasketball(PlayerSelectedBall.BasketballTexture, PlayerSelectedBall.FrameList, PlayerSelectedBall.BasketballEmitter);
            newBall.BasketballBody.Position = new Vector2((s_random.Next(400, 1200)) / PhysicalWorld.MetersInPixels, (s_random.Next(310, 650)) / PhysicalWorld.MetersInPixels);
            newBall.BasketballBody.Awake = false;
            newBall.HasBallScored = false;
            s_activeBasketballs.Add(newBall);
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
                            HandleShotAngle(ball.BasketballBody, InterfaceSettings.Force);
                            SoundManager.PlaySoundEffect(Sounds.BasketBallShotSoundEffect,
                                                         (float) InterfaceSettings.GameSettings.SoundEffectVolume/10,
                                                         0.0f, 0.0f);
                        }
                    }
                }
            }
        }

        private static void HandleShotAngle(Body ball, float shotForce)
        {
            ball.ApplyLinearImpulse(ConvertUnits.ToSimUnits(InterfaceSettings.PointingAt) * shotForce);
            ball.ApplyAngularImpulse(.2f);
            s_lastShotMade = true;
        }

        private static double MouseAngle(Vector2 spriteLocation, Vector2 mouseLocation)
        {
            return Math.Atan2(mouseLocation.Y - (spriteLocation.Y), mouseLocation.X - (spriteLocation.X)); //this will return the angle(in radians) from sprite to mouse.
        }
    }
}
