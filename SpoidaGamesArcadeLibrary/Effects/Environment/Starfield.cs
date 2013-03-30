using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects.Environment
{
    public class Starfield
    {
        private List<Stars> stars = new List<Stars>();
        private int width = 1280;
        private int height = 720;
        private Random rand = new Random();
        private Color[] colors = { Color.White, Color.GhostWhite, Color.LightGray, Color.LightSteelBlue, Color.LightBlue};

        private int starSpeedModifier;
        public int StarSpeedModifier
        {
            get { return starSpeedModifier; }
            set { starSpeedModifier = value; }
        }

        public Starfield(int screenWidth, int screenHeight, int starCount, List<Texture2D> textures)
        {
            width = screenWidth;
            height = screenHeight;
            for (int x = 0; x < starCount; x++)
            {
                Texture2D texture = textures[rand.Next(textures.Count)];
                stars.Add(new Stars(new Vector2(rand.Next(0,width),rand.Next(0,height)),texture,new Vector2(0, rand.Next(8,22))));
                Color starColor = colors[rand.Next(0, colors.Count())];
                starColor *= rand.Next(10, 80)/100f;
                stars[stars.Count() - 1].TintColor = starColor;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Stars star in stars)
            {
                star.Update(gameTime, starSpeedModifier);
                if (star.Location.Y > height)
                {
                    star.Location = new Vector2(rand.Next(0, width), 0);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Stars star in stars)
            {
                star.Draw(spriteBatch);
            }
        }
    }
}
