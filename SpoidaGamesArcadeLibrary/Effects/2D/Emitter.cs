using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class Emitter
    {
        private readonly Random m_random;
        
        private readonly List<Particle> m_particles;
        public List<Texture2D> Textures;
        public List<Color> Colors { get; set; }

        public Vector2 EmitterLocation { get; set; }

        private ParticleEmitterTypes EmitterType { get; set; }

        public int ParticleCount { get; set; }

        public bool ParticlesCanChange { get; set; }

        public Emitter(List<Texture2D> textures, Vector2 location, int particleCount, List<Color> initialColors, ParticleEmitterTypes type, bool particlesCanChange)
        {
            EmitterLocation = location;
            Textures = textures;
            ParticleCount = particleCount;
            Colors = initialColors;
            EmitterType = type;
            ParticlesCanChange = particlesCanChange;

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
            
            Color color = Colors[m_random.Next(Colors.Count)];

            Vector2 position = EmitterLocation;
            Vector2 velocity = GenerateParticleVector();

            float angle = GenerateParticleAngle();
            float angularVelocity = GenerateParticleAngleVelocity();
            float size = (float)m_random.NextDouble();

            int ttl = GenerateTimeToLive();
 
            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        private Vector2 GenerateParticleVector()
        {
            switch (EmitterType)
            {
                case ParticleEmitterTypes.SparkleEmitter:
                    return new Vector2((1f * (float)(m_random.NextDouble() * 2 - 1)), 1f * (float)(m_random.NextDouble() * 2 - 1));
                case ParticleEmitterTypes.CombusionEmitter:
                    return new Vector2((1f * (float)(m_random.NextDouble() * 2 - 1)), 1f * (float)(m_random.NextDouble() * 2 - 1));
                default:
                    return new Vector2((1f*(float) (m_random.NextDouble()*2 - 1)/2), 1f*(float) (m_random.NextDouble()*2 - 1)/2);
            }
        }

        private float GenerateParticleAngle()
        {
            switch (EmitterType)
            {
                case ParticleEmitterTypes.SparkleEmitter:
                    return (0.1f*(float) (m_random.NextDouble()*2 - 1));
                case ParticleEmitterTypes.CombusionEmitter:
                    return 0;
                default:
                    return (0.1f * (float)(m_random.NextDouble() * 2 - 1));
            }
        }

        private int GenerateTimeToLive()
        {
            switch (EmitterType)
            {
                case ParticleEmitterTypes.SparkleEmitter:
                    return 5 + m_random.Next(30);
                case ParticleEmitterTypes.CombusionEmitter:
                    return 5 + m_random.Next(30);
                case ParticleEmitterTypes.StarEmitter:
                    return 25 + m_random.Next(10);
                default:
                    return 5 + m_random.Next(30);
            }
        }

        private float GenerateParticleAngleVelocity()
        {
            switch (EmitterType)
            {
                case ParticleEmitterTypes.SparkleEmitter:
                    return (0.1f * (float)(m_random.NextDouble() * 2 - 1));
                case ParticleEmitterTypes.CombusionEmitter:
                    return (0.1f * (float)(m_random.NextDouble() * 2 - 1));
                default:
                    return (0.1f * (float)(m_random.NextDouble() * 2 - 1));
            }
        }

        public void CleanUpParticles()
        {
            m_particles.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (EmitterType)
            {
                case ParticleEmitterTypes.SparkleEmitter:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    foreach (Particle t in m_particles)
                    {
                        t.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    break;
                case ParticleEmitterTypes.CombusionEmitter:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    foreach (Particle t in m_particles)
                    {
                        t.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    break;
                case ParticleEmitterTypes.StarEmitter:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    foreach (Particle t in m_particles)
                    {
                        t.Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    break;
            }
        }
    }
}
