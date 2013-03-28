using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class CometTailEmitter
    {
        private static Vector2 emitterPosition;
        public static Vector2 EmitterPosition
        {
            get { return emitterPosition; }
            set { emitterPosition = value; }
        }

        private static Color particleColor;
        public static Color ParticleColor
        {
            get { return particleColor; }
            set { particleColor = value; }
        }

        private static int particleCount;
        public static int ParticleCount
        {
            get { return particleCount; }
            set { particleCount = value; }
        }

        private static Texture2D cometTexture;
        public static Texture2D CometTexture
        {
            get { return cometTexture; }
            set { cometTexture = value; }
        }

        private static Particle[] particles;
        private static int nextParticle;
        private static Vector2 targetPosition;
        private static Vector2 lastPosition;

        public static void Initialize(Texture2D texture, int count)
        {
            CometTexture = texture;
            EmitterPosition = Vector2.Zero;
            ParticleCount = count;
            ParticleColor = Color.White;
            targetPosition = EmitterPosition;
            lastPosition = targetPosition;
            LoadParticles();
        }

        public static void LoadParticles()
        {
            particles = new Particle[particleCount];

            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = new Particle
                                   {
                                       position = EmitterPosition,
                                       Texture = CometTexture
                                   };
                particles[i].color = new Color(particles[i].color.R, particles[i].color.G, particles[i].color.B, 0);
                particles[i].width = 48;
                particles[i].height = 48;
            }
        }

        public static void Update(GameTime gameTime, Vector2 ballPosition)
        {
            EmitterPosition = ballPosition;
            for (int p = 0; p < particles.Length; p++)
            {
                if (p == nextParticle && lastPosition != EmitterPosition)
                {
                    particles[p].position = lastPosition;
                    particles[p].color = particleColor;
                }
                if (particles[p].position != EmitterPosition) // Particle is in use
                {
                    particles[p].scale = 1 - (Vector2.Distance(particles[p].position, targetPosition) / 400);
                    particles[p].color = new Color(particles[p].color.R, particles[p].color.G, particles[p].color.B, (byte)(particles[p].scale * 255));
                }
            }
            nextParticle++;

            if (nextParticle >= particles.Length)
                nextParticle = 0;

            lastPosition = targetPosition;
            targetPosition = EmitterPosition;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                particle.Draw(spriteBatch);
            }
        }
    }
}
