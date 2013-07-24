using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Settings;

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

        public ParticleEmitterTypes BasketballEmitter { get; set; }
        public Body BasketballBody { get; set; }
        public bool HasBallScored { get; set; }
        public bool HasBallFired { get; set; }

        public ArcadeBasketball(Texture2D texture, List<Rectangle> framesList, ParticleEmitterTypes ballEmitter)
        {
            BasketballTexture = texture;
            m_frames = framesList;
            BasketballEmitter = ballEmitter;
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
                HasBallScored = true;
                ArcadeGoalManager.Streak++;
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BasketballTexture, BasketballBody.Position*PhysicalWorld.MetersInPixels, Source, Color.White, BasketballBody.Rotation, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
