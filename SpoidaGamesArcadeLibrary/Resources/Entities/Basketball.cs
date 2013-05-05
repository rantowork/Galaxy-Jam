using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class Basketball
    {
        private Texture2D basketballTexture;
        public Texture2D BasketballTexture
        {
            get { return basketballTexture; }
            set { basketballTexture = value; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(Source.Width / 2f, Source.Height / 2f); }
        }

        private List<Rectangle> frames = new List<Rectangle>();
        private int currentFrame;
        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
        }

        public int FrameCount
        {
            get { return frames.Count; }
        }

        private float frameTime = 0.2f;
        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }
        }

        private float timeLeftForCurrentFrame;
        public float TimeLeftForCurrentFrame
        {
            get { return timeLeftForCurrentFrame; }
            set { timeLeftForCurrentFrame = value; }
        }

        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        private bool animate;
        public bool Animate
        {
            get { return animate; }
            set { animate = value; }
        }

        private string basketballName;
        public string BasketballName
        {
            get { return basketballName; }
            set { basketballName = value; }
        }

        public Basketball(Texture2D texture, List<Rectangle> framesList, bool isAnimated, string name)
        {
            BasketballTexture = texture;
            frames = framesList;
            Animate = isAnimated;
            BasketballName = name;
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
