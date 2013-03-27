using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class ParticleEmitter : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        public Particle[] particles;
        public Vector2 position;
        public int particleCount;
        public Color particleColor;

        int nextParticle;
        Vector2 targetPos;
        Vector2 myLastpos;

        public ParticleEmitter(Game game, int count) : base(game)
        {
            position = Vector2.Zero;
            particleCount = count;
            particleColor = Color.White;
        }
        protected override void LoadContent()
        {
            position = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            Texture2D tmpTexture = Game.Content.Load<Texture2D>("Textures/fire");

            LoadParticles(tmpTexture);

            targetPos = position;
            myLastpos = targetPos;

            base.LoadContent();
        }
        public void LoadParticles(Texture2D tmpTexture)
        {
            particles = new Particle[particleCount];

            for (int p = 0; p < particles.Length; p++)
            {
                particles[p] = new Particle
                                   {
                                       position = position, 
                                       Texture = tmpTexture
                                   };
                particles[p].color = new Color(particles[p].color.R, particles[p].color.G, particles[p].color.B, 0);
                particles[p].width = 48;
                particles[p].height = 48;
            }
        }
        public override void Update(GameTime gameTime)
        {
            for (int p = 0; p < particles.Length; p++)
            {
                if (p == nextParticle && myLastpos != position)
                {
                    particles[p].position = myLastpos;
                    particles[p].color = particleColor;
                }
                if (particles[p].position != position) // Particle is in use
                {
                    particles[p].scale = 1 - (Vector2.Distance(particles[p].position, targetPos) / 400);
                    particles[p].color = new Color(particles[p].color.R, particles[p].color.G, particles[p].color.B, (byte)(particles[p].scale * 255));
                }
            }
            nextParticle++;

            if (nextParticle >= particles.Length)
                nextParticle = 0;

            myLastpos = targetPos;
            targetPos = new Vector2((float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) * 200, (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 200) + position;

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null);
            
            foreach (Particle t in particles)
            {
                t.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
