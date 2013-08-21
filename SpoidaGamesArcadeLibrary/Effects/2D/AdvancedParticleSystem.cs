using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class AdvancedParticleSystem
    {
        private readonly Texture2D m_particleSprite;             // 2D texture to draw to the screen
        private readonly Vector2 m_origin;                       // origin of the texture (center of the texture)
        private readonly List<AdvancedParticle> m_particleList;          // List of all the particles
        readonly Random m_rand = new Random();                     // random number generator
        private float m_elapsedTime;             // elapsed time since the last update

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">the current instance of the game</param>
        /// <param name="newSettings">setting for the particle system</param>
        public AdvancedParticleSystem(Game game, AdvancedParticleEmitter newSettings)
        {
            Settings = newSettings;

            m_particleList = new List<AdvancedParticle>(Settings.InitialParticleCount);

            for (int i = 0; i < Settings.InitialParticleCount; ++i)
            {
                m_particleList.Add(new AdvancedParticle());
            }

            // load particle texture
            m_particleSprite = game.Content.Load<Texture2D>(Settings.ParticleTextureFileName);

            // set the origin of the texture
            m_origin = new Vector2((float)m_particleSprite.Width / 2, (float)m_particleSprite.Height / 2);

            // initialize the position to zero
            OriginPosition = new Vector2();
        }

        /// <summary>
        /// calculate a new velocity for a newly initialized particle
        /// </summary>
        /// <returns>Vector2</returns>
        private Vector2 GetNewVelocity()
        {
            // get a random angle between min direction angle and max direction angle
            float angle = RandomMinMax(Settings.MinimumDirectionAngle, Settings.MaximumDirectionAngle);

            // convert the angle to radians
            angle = MathHelper.ToRadians(-angle);

            // return the new velocity based on the angle
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        
        /// <summary>
        /// Update the particles based on the number of particles to generate per second
        /// </summary>
        /// <param name="gameTime">elapsed game time</param>
        private void UpdateParticlesPerSec(GameTime gameTime)
        {
            // grab the elapsed time since the last update
            m_elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // calculate the approximate number of particle to add this iteration
            float particlePerSec = Settings.ParticlesPerSecond * m_elapsedTime;

            foreach (AdvancedParticle p in m_particleList)
            {
                // add a new particle if we still have remaining particles to add
                if (particlePerSec > 0.0f)
                {
                    if (p.Active == false)
                    {
                        InitializeParticle(p);
                        --particlePerSec;
                    }
                }

                if (p.Active)
                    p.Update((float)(gameTime.ElapsedGameTime.TotalSeconds));
            }
        }

        /// <summary>
        /// Update the particle system as just a single explosion
        /// </summary>
        /// <param name="gameTime">elapsed game time</param>
        private void UpdateParticleBurst(GameTime gameTime)
        {
            // here we want to only initialize new particles until all the 
            // particles in the List are active. Once all particles are
            // active, the burst is complete so do not re-initialize any
            // new particles, until m_settings.EndBurst == false, again
            foreach (AdvancedParticle p in m_particleList)
            {
                if (p.Active == false)
                {
                    if (!Settings.EndBurst)
                        InitializeParticle(p);
                }
                else
                {
                    Settings.EndBurst = true;
                }

                // update all active particles
                if (p.Active)
                    p.Update((float)(gameTime.ElapsedGameTime.TotalSeconds));
            }
        }

        /// <summary>
        /// Update the particle system
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            // here we update the particles based on whether the
            // particle system is in Burst mode or not
            if (Settings.IsBurst)
            {
                UpdateParticleBurst(gameTime);
            }
            else
            {
                UpdateParticlesPerSec(gameTime);
            }
        }

        /// <summary>
        /// Initialize a particle to its default values
        /// </summary>
        /// <param name="particle">Particle to initialize</param>
        private void InitializeParticle(AdvancedParticle particle)
        {
            // get new particle velocity (based on min and max direction angles)
            Vector2 velocity = GetNewVelocity();

            // grab a new speed between MinimumSpeed and MaximumSpeed
            float speed = RandomMinMax(Settings.MinimumSpeed, Settings.MaximumSpeed);

            // add the speed to the velocity
            velocity *= speed;

            // grab a new amount of time the particle should remain alive
            float lifeTime = RandomMinMax(Settings.MinimumLifeTime, Settings.MaximumLifeTime);

            // grab a new size for the particle 
            float scale = RandomMinMax(Settings.MinimumScale, Settings.MaximumScale);

            // initialize the particle with the new settings
            particle.Initialize(OriginPosition, velocity, lifeTime, scale, true);
        }

        /// <summary>
        /// Get a random float between min and max
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        private float RandomMinMax(float min, float max)
        {
            return min + (float)m_rand.NextDouble() * (max - min);
        }

        /// <summary>
        /// Draw all active particles
        /// </summary>
        /// <param name="gameTime">elapsed game time</param>
        /// <param name="spriteBuffer">the passed in sprite batch</param>
        public virtual void  Draw(GameTime gameTime, SpriteBatch spriteBuffer)
        {
            spriteBuffer.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            foreach (AdvancedParticle p in m_particleList)
            {
                // draw each active particle in the system
                if (p.Active)
                {
                    // the older the particle, the more transparent it becomes
                    // or basically it fades away the older it gets
                    float remainingLife = p.LifeTimeStart / p.LifeTime;
                    float alpha = 4 * remainingLife * (1 - remainingLife);
                    Color particleColor = ParticleColors[s_random.Next(ParticleColors.Count)];
                    Color color = particleColor * alpha;

                    spriteBuffer.Draw(m_particleSprite, p.Position, null, color,
                        0.0f, m_origin, p.Scale, SpriteEffects.None, 0.0f);
                }
            }
            spriteBuffer.End();
        }

        //****************************
        // Properties
        //****************************
        public Vector2 OriginPosition { get; set; }
        public AdvancedParticleEmitter Settings { get; set; }
        public List<Color> ParticleColors = new List<Color>();
        private static readonly Random s_random = new Random();
    }
}
