using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects.Environment
{
    public class Stars
    {
        public Texture2D texture;

        protected List<Rectangle> frames = new List<Rectangle>();
        private int frameWidth;
        private int frameHeight;
        private int currentFrame;

        private Color tintColor = Color.White;
        private float rotation;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;

        public Stars(Vector2 spriteLocation, Texture2D spriteTexture, Vector2 spriteVelocity)
        {
            location = spriteLocation;
            texture = spriteTexture;
            velocity = spriteVelocity;

            frames.Add(new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height));
            frameWidth = spriteTexture.Width;
            frameHeight = spriteTexture.Height;
        }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
        }

        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        public Rectangle Destination
        {
            get { return new Rectangle((int)location.X, (int)location.Y, frameWidth, frameHeight); }
        }

        public Vector2 Center
        {
            get { return location + new Vector2(frameWidth / 2, frameHeight / 2); }
        }

        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            location += (velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Center, Source, tintColor, rotation, new Vector2(frameWidth / 2, frameHeight / 2),
                             1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
