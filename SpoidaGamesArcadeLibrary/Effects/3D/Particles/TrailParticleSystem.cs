using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
    public class TrailParticleSystem : DefaultTexturedQuadParticleSystem
    {
        public TrailParticleSystem(Game cGame) : base(cGame)
        {
            m_distanceTravelled = 0;
        }

        //===========================================================
        // Structures and Variables
        //===========================================================
        public Color TrailStartColor = Color.Red;
        public Color TrailEndColor = Color.Yellow;

        public int TrailStartSize;
        public int TrailEndSize;

        /// <summary>
        /// Adjust the scale to produce more or less particles.
        /// 1.0 = second particle will be touching (no overlapping) next particle.
        /// 0.5 = second particle will overlap half of first particle.
        /// 0.25 = second particle will overlap 3/4 of first particle.
        /// </summary>
        public float NumberOfParticlesToEmitScale
        {
            get { return m_numberOfParticlesToEmitScale; }
            set { m_numberOfParticlesToEmitScale = DPSFHelper.ValueInRange(value, 0.05f, 2.0f); }
        }
        private float m_numberOfParticlesToEmitScale;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================

        /// <summary>
        /// Initializes the render properties.
        /// </summary>
        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();

            // Use additive blending
            RenderProperties.BlendState = BlendState.Additive;

            //RenderProperties.RasterizerState.CullMode = CullMode.None;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================
        public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
        {
            InitializeTexturedQuadParticleSystem(graphicsDevice, contentManager, 1000, 50000, UpdateVertexProperties, "Textures/Particle");

            LoadParticleSystem();
            Name = "Trail";
        }

        public void InitializeParticleTrail(DefaultTexturedQuadParticle cParticle)
        {
            cParticle.Lifetime = 2.0f;

            cParticle.Position = Emitter.PositionData.Position;
            cParticle.StartSize = cParticle.Size = TrailStartSize;
            cParticle.EndSize = TrailEndSize;
            cParticle.StartColor = cParticle.Color = TrailStartColor;
            cParticle.EndColor = TrailEndColor;

            cParticle.Velocity = Vector3.Zero;
            cParticle.Acceleration = Vector3.Zero;
            cParticle.Orientation = Emitter.OrientationData.Orientation;
            cParticle.RotationalVelocity = new Vector3(0, 0, (float)Math.PI);
        }

        public void LoadParticleSystem()
        {
            ParticleInitializationFunction = InitializeParticleTrail;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 100);

            ParticleSystemEvents.RemoveAllEvents();
            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed);

            Emitter.PositionData.Position = m_emittersLastPosition = new Vector3(72, 35, 0);
            Emitter.OrientationData.RotationalVelocity = new Vector3(0, 0, (float)Math.PI);
            Emitter.ParticlesPerSecond = 50;

            TrailStartColor = Color.White;
            TrailEndColor = Color.DarkGray;
            TrailStartSize = 5;
            TrailEndSize = 20;
            SetTexture("Textures/Particle");

            NumberOfParticlesToEmitScale = 0.05f;
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        // Persistent variables used by the UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed() function.
        private float m_timeSinceLastUpdate;
        private const float WAIT_TIME_BETWEEN_UPDATES = 0.05f;
        private const float NUMBER_OF_UPDATES_PER_SECOND = (1.0f / WAIT_TIME_BETWEEN_UPDATES);
        private Vector3 m_emittersLastPosition;

        // Temp variables used by UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed() to avoid creating garbage for the collector.
        private float m_distanceTravelled;
        private float m_distancePerSecond;

        /// <summary>
        /// Dynamically update the number of particles to emit based on how fast the emitter is moving, so if it
        /// is moving fast, more particles will be released, and fewer will be released if it is moving slowly.
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        protected void UpdateParticleSystemDynamicallyUpdateParticlesEmittedBasedOnSpeed(float fElapsedTimeInSeconds)
        {
            // If not enough time has elapsed to perform an update yet, then just exit.
            m_timeSinceLastUpdate += fElapsedTimeInSeconds;
            if (m_timeSinceLastUpdate < WAIT_TIME_BETWEEN_UPDATES)
                return;

            // We're doing an update now, so update the amount of time since the last update.
            m_timeSinceLastUpdate -= WAIT_TIME_BETWEEN_UPDATES;

            // Calculate how far the emitter has travelled since the last update.
            m_distanceTravelled = (Emitter.PositionData.Position - m_emittersLastPosition).Length();

            // Calculate how many particles to emit based on the distance the emitter has travelled, and the given Scale.
            m_distancePerSecond = m_distanceTravelled * NUMBER_OF_UPDATES_PER_SECOND;
            Emitter.ParticlesPerSecond = m_distancePerSecond / (TrailStartSize * NumberOfParticlesToEmitScale);

            // Record the emitter's position for the next update.
            m_emittersLastPosition = Emitter.PositionData.Position;
        }
    }
}
