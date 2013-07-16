using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class Emitter
    {
        private readonly Random m_random;
        private readonly List<Particle> m_particles;
        
        public List<Texture2D> Textures; 
        public int ParticleCount { get; set; }
        public List<Color> Colors { get; set; }
        public Vector2 EmitterLocation { get; set; }
        public float EmitterAngle { get; set; }

        public Emitter(List<Texture2D> textures, Vector2 location, int particleCount, List<Color> initialColors, float initialAngle)
        {
            EmitterLocation = location;
            Textures = textures;
            ParticleCount = particleCount;
            Colors = initialColors;
            EmitterAngle = initialAngle;

            m_random = new Random();
            m_particles = new List<Particle>();
        }
 
        public void Update()
        {
            int total = ParticleCount;
 
            for (int i = 0; i < total; i++)
            {
                m_particles.Add(GenerateNewParticle());
            }
 
            for (int particle = 0; particle < m_particles.Count; particle++)
            {
                m_particles[particle].Update();
                if (m_particles[particle].Ttl <= 0)
                {
                    m_particles.RemoveAt(particle);
                    particle--;
                }
            }
        }
 
        private Particle GenerateNewParticle()
        {
            Texture2D texture = Textures[m_random.Next(Textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(m_random.NextDouble() * 2 - 1),
                                    1f * (float)(m_random.NextDouble() * 2 - 1));

            float angle = EmitterAngle;
            float angularVelocity = 0.1f * (float)(m_random.NextDouble() * 2 - 1);

            Color color = Colors[m_random.Next(Colors.Count)];
            float size = (float)m_random.NextDouble();
            int ttl = 5 + m_random.Next(30);
 
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void CleanUpParticles()
        {
            m_particles.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle t in m_particles)
            {
                t.Draw(spriteBatch);
            }
        }
    }
}
