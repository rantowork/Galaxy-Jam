using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class SparkleEmitter
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;
        
        private List<Color> colors = new List<Color> {Color.DarkRed, Color.DarkOrange};
        public List<Color> Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        private int particleCount = 50;
        public int ParticleCount
        {
            get { return particleCount; }
            set { particleCount = value; }
        }

        public SparkleEmitter(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            particles = new List<Particle>();
            random = new Random();
        }
 
        public void Update()
        {
            int total = particleCount;
 
            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }
 
            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Ttl <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }
 
        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);

            Color color = colors[random.Next(colors.Count)];
            float size = (float)random.NextDouble();
            int ttl = 5 + random.Next(30);
 
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }
 
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle t in particles)
            {
                t.Draw(spriteBatch);
            }
        }
    }
}
