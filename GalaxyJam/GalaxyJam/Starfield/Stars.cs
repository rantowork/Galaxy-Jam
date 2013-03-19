using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyJam.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyJam.Starfield
{
    class Stars
    {
        private List<BasicSprite> stars = new List<BasicSprite>();
        private int width = 1280;
        private int height = 720;
        private Random rand = new Random();
        private Color[] colors = { Color.White, Color.Orange, Color.OrangeRed, Color.DarkOrange, Color.IndianRed};

        public Stars(int screenWidth, int screenHeight, int starCount, Texture2D texture, Rectangle frameRectangle)
        {
            width = screenWidth;
            height = screenHeight;
            for (int x = 0; x < starCount; x++)
            {
                stars.Add(new BasicSprite(new Vector2(rand.Next(0,width),rand.Next(0,height)),texture,frameRectangle,new Vector2(0, rand.Next(15,40))));
                Color starColor = colors[rand.Next(0, colors.Count())];
                starColor *= rand.Next(30, 80)/100f;
                stars[stars.Count() - 1].TintColor = starColor;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (BasicSprite star in stars)
            {
                star.Update(gameTime);
                if (star.Location.Y > height)
                {
                    star.Location = new Vector2(rand.Next(0, width), 0);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (BasicSprite star in stars)
            {
                star.Draw(spriteBatch);
            }
        }
    }
}
