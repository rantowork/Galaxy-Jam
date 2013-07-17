using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class Basketball
    {
        public Texture2D BasketballTexture { get; set; }

        public Vector2 Origin
        {
            get { return new Vector2(Source.Width / 2f, Source.Height / 2f); }
        }

        private readonly List<Rectangle> m_frames = new List<Rectangle>();
        
        public List<Rectangle> FrameList
        {
            get { return m_frames; }
        } 

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

        public bool Animate { get; set; }
        public string BasketballName { get; set; }
        public int BasketballUnlockScore { get; set; }
        public ParticleEmitterTypes BasketballEmitter { get; set; }

        public Basketball(Texture2D texture, List<Rectangle> framesList, bool isAnimated, string name, int unlockScore, ParticleEmitterTypes ballEmitter)
        {
            BasketballTexture = texture;
            m_frames = framesList;
            Animate = isAnimated;
            BasketballName = name;
            BasketballUnlockScore = unlockScore;
            BasketballEmitter = ballEmitter;
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            TimeLeftForCurrentFrame += elapsed;

            if (Animate)
            {
                if (TimeLeftForCurrentFrame >= FrameTime)
                {
                    Frame = (Frame + 1) % (FrameCount);
                    TimeLeftForCurrentFrame = 0.0f;
                }
            }
        }
    }
}
