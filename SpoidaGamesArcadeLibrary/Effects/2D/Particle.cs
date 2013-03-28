using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class Particle
    {
        private Texture2D texture;
        public Vector2 position;
        public float rotation;
        public float scale;
        public Color color;
        public int width;
        public int height;
        public Vector2 origin;
        public int depth;
        public int timeToLive;

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        public Particle()
        {
            position = Vector2.Zero;
            rotation = 0;
            scale = 1;
            color = Color.White;
            origin = Vector2.Zero;
            depth = 0;
            timeToLive = 10;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)(width * scale), (int)(height * scale)), new Rectangle(0, 0, texture.Width, texture.Height), color, rotation, origin, SpriteEffects.None, depth);
        }
    }
}

