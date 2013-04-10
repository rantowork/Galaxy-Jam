using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class Basketball
    {
        private Random random = new Random();

        private Texture2D basketballTexture;
        public Texture2D BasketballTexture
        {
            get { return basketballTexture; }
            set { basketballTexture = value; }
        }

        private Body basketballBody;
        public Body BasketballBody
        {
            get { return basketballBody; }
        }

        public Basketball()
        {
            basketballBody = BodyFactory.CreateCircle(PhysicalWorld.World, 32f / (2f * PhysicalWorld.MetersInPixels), 1.0f, new Vector2((random.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (random.Next(310, 680)) / PhysicalWorld.MetersInPixels));
            basketballBody.BodyType = BodyType.Dynamic;
            basketballBody.Restitution = 0.3f;
            basketballBody.Friction = 0.1f;
        }
    }
}
