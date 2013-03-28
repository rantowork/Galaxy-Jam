using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyJam.Particles
{
    public class Basket
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particlesList;
        private List<Texture2D> texturesList;

        private bool toTheLeft;

        private List<Color> colors = new List<Color>();
        public List<Color> Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        public Basket(List<Texture2D> textures, Vector2 location, List<Color> colorList, bool flowToTheLeft)
        {
            EmitterLocation = location;
            texturesList = textures;
            particlesList = new List<Particle>();
            random = new Random();
            Colors = colorList;
            toTheLeft = flowToTheLeft;
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = texturesList[random.Next(texturesList.Count)];

            Vector2 position = EmitterLocation;

            //Vector2 velocity = new Vector2(1f * (float)(random.NextDouble() * 2 - 1), 1f * (float)(random.NextDouble() * 2 - 1));
            //Vector2 velocity = new Vector2(0,0);
            Vector2 velocity = new Vector2(1, 0);

            const float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            //float size = (float)random.NextDouble();

            Color particleColor = colors[random.Next(0, colors.Count)];

            float size = 1f;

            int ttl = 10;// +random.Next(20);

            return new Particle(texture, position, velocity, angle, angularVelocity, particleColor, size, ttl);
        }

        public void Update(GameTime gameTime)
        {
            int total = 8;

            for (int i = 0; i < total; i++)
            {
                particlesList.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particlesList.Count; particle++)
            {
                particlesList[particle].Update();
                if (particlesList[particle].Ttl <= 0)
                {
                    particlesList.RemoveAt(particle);
                    particle--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particlesList)
            {
                particle.Draw(spriteBatch);
            }
        }
    }
}
