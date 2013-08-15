using System;
using System.Collections.Generic;
using DPSF;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects._3D.Particles;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Settings;
using SpoidaGamesArcadeLibrary.GameStates;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class ArcadeBasketball
    {
        private readonly Random m_random = new Random();

        public Texture2D BasketballTexture { get; set; }

        public Vector2 Origin
        {
            get { return new Vector2(Source.Width / 2f, Source.Height / 2f); }
        }

        private readonly List<Rectangle> m_frames = new List<Rectangle>();
        private int m_currentFrame;
        public int Frame
        {
            get { return m_currentFrame; }
            set { m_currentFrame = (int)MathHelper.Clamp(value, 0, m_frames.Count - 1); }
        }

        public int FrameCount
        {
            get { return m_frames.Count; }
        }

        private float m_frameTime = 0.2f;
        public float FrameTime
        {
            get { return m_frameTime; }
            set { m_frameTime = MathHelper.Max(0, value); }
        }

        public float TimeLeftForCurrentFrame { get; set; }

        public Rectangle Source
        {
            get { return m_frames[m_currentFrame]; }
        }

        public Emitter BallEmitter { get; set; }
        public ParticleEmitterTypes BallEmitterType { get; set; }
        public Body BasketballBody { get; set; }
        public bool HasBallScored { get; set; }
        public bool HasBallFired { get; set; }
        public AnimatedSpriteParticleSystemWrapper ParticleWrapper { get; set; }

        private int m_scoreModifier = 1000;

        public ArcadeBasketball(Texture2D texture, List<Rectangle> framesList, ParticleEmitterTypes ballEmitter)
        {
            BasketballTexture = texture;
            m_frames = framesList;
            BallEmitter = ParticleEmitters.LoadArcadeEmitter(ballEmitter);
            BallEmitterType = ballEmitter;
            BasketballBody = BodyFactory.CreateCircle(PhysicalWorld.World, 32f / (2f * PhysicalWorld.MetersInPixels), 1.0f, new Vector2((m_random.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (m_random.Next(310, 680)) / PhysicalWorld.MetersInPixels));
            BasketballBody.BodyType = BodyType.Static;
            BasketballBody.Mass = 1f;
            BasketballBody.Restitution = 0.3f;
            BasketballBody.Friction = 0.1f;
        }

        public virtual void Update(GameTime gameTime)
        {
            Vector2 basketballCenter = BasketballBody.WorldCenter*PhysicalWorld.MetersInPixels;
            Rectangle basketballCenterRectangle = new Rectangle((int)basketballCenter.X - 8, (int)basketballCenter.Y - 8, 16, 16);
            if (GoalManager.BasketLocation.Intersects(basketballCenterRectangle) && !HasBallScored)
            {
                SoundManager.PlaySoundEffect(Sounds.BasketScoredSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.Explode();
                HasBallScored = true;
                Screen.Camera.Shaking = true;
                ArcadeGoalManager.Streak++;
                if (ArcadeGoalManager.Streak >= 20)
                {
                    m_scoreModifier = 10000;
                }
                else if (ArcadeGoalManager.Streak >= 50)
                {
                    m_scoreModifier = 50000;
                }
                else
                {
                    m_scoreModifier = 1000;
                }
                if (ArcadeModeScreenState.ShowDoubleScore)
                {
                    ArcadeGoalManager.Score += m_scoreModifier * (ArcadeGoalManager.Multiplier * 2);
                }
                else
                {
                    ArcadeGoalManager.Score += m_scoreModifier * ArcadeGoalManager.Multiplier;                
                }
                ArcadeGoalManager.DrawNumberScrollEffect = true;
                if (ArcadeGoalManager.Streak % 4 == 0 && ArcadeGoalManager.Streak != 0)
                {
                    SoundManager.PlaySoundEffect(Sounds.ArcadeModeStreak, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
                }
            }

            if (BallEmitter.ParticlesCanChange)
            {
                if (ArcadeGoalManager.Streak >= 4 && ArcadeGoalManager.Streak < 8)
                {
                    BallEmitter.Colors = new List<Color> {Color.Purple, Color.Plum, Color.Orchid};
                }
                else if (ArcadeGoalManager.Streak >= 8 && ArcadeGoalManager.Streak < 12)
                {
                    BallEmitter.Colors = new List<Color> {Color.LimeGreen, Color.Teal, Color.Green};
                }
                else if (ArcadeGoalManager.Streak >= 12 && ArcadeGoalManager.Streak < 16)
                {
                    BallEmitter.Colors = new List<Color> {Color.DarkRed, Color.Red, Color.IndianRed};
                }
                else if (ArcadeGoalManager.Streak >= 16)
                {
                    BallEmitter.Colors = new List<Color> {Color.Thistle, Color.BlueViolet, Color.RoyalBlue};
                }
                else
                {
                    BallEmitter.Colors = new List<Color> {Color.DarkRed, Color.DarkOrange};
                }
            }

            if (BallEmitterType == ParticleEmitterTypes.Explosion)
            {
                ParticleSystems.BallParticleSystemManager.SetCameraPositionForAllParticleSystems(ParticleSystems._3DCamera.Position);
                ParticleSystems.BallParticleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(ParticleSystems.WorldMatrix, ParticleSystems.ViewMatrix, ParticleSystems.ProjectionMatrix);
                ParticleSystems.BallParticleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);
                ParticleWrapper.Emitter.PositionData.Position = new Vector3(basketballCenter.X, basketballCenter.Y, 0);
            }
            else
            {
                BallEmitter.EmitterLocation = BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                BallEmitter.Update();
            }
        }

        public virtual void DestroyParticles()
        {
            if (BallEmitterType == ParticleEmitterTypes.Explosion)
            {
                ParticleSystems.ParticleSystemManager.RemoveParticleSystem(ParticleWrapper);
                ParticleWrapper.Destroy();
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BasketballTexture, BasketballBody.Position*PhysicalWorld.MetersInPixels, Source, Color.White, BasketballBody.Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }

        public virtual void DrawEmitter(SpriteBatch spriteBatch)
        {
            BallEmitter.Draw(spriteBatch);
        }
    }
}
