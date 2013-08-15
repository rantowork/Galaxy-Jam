using System;
using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
    public class AnimatedSpriteParticleSystem : DefaultAnimatedSpriteParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AnimatedSpriteParticleSystem(Game cGame) : base(cGame) { }

        //===========================================================
        // Structures and Variables
        //===========================================================
        Animations m_explosionAnimation;
        private const int NUMBER_OF_PICTURES_IN_ANIMATION = 16;
        private const float TIME_BETWEEN_ANIMATION_IMAGES = 0.08f; // Default Animation speed

        readonly Vector3 m_mousePosition = new Vector3();

        int m_screenWidth;
        int m_screenHeight;

        public ParticleEmitterTypes EmitterType { get; set; }

#if (WINDOWS)
        [Serializable]
#endif
        enum EColorAmounts
        {
            None = 0,
            Some = 1,
            All = 2,
            LastInList = 2
        }
        EColorAmounts m_colorAmount = EColorAmounts.None;

        //===========================================================
        // Overridden Particle System Functions
        //===========================================================
        protected override void AfterInitialize()
        {
            base.AfterInitialize();

            // Setup the Animation
            m_explosionAnimation = new Animations();

            // The Order of the Picture IDs to make up the Animation
            int[] iaAnimationOrder = new int[NUMBER_OF_PICTURES_IN_ANIMATION];
            for (int iIndex = 0; iIndex < NUMBER_OF_PICTURES_IN_ANIMATION; iIndex++)
            {
                iaAnimationOrder[iIndex] = iIndex;
            }

            // Create the Pictures and Animation and Set the Animation to use for Explosions
            m_explosionAnimation.CreatePicturesFromTileSet(NUMBER_OF_PICTURES_IN_ANIMATION, 16, new Rectangle(0, 0, 64, 64));
            int animationId = m_explosionAnimation.CreateAnimation(iaAnimationOrder, TIME_BETWEEN_ANIMATION_IMAGES, 1);
            m_explosionAnimation.CurrentAnimationID = animationId;
        }

        protected override void AfterDestroy()
        {
            base.AfterDestroy();
            m_explosionAnimation = null;
        }

        protected override void SetEffectParameters()
        {
            base.SetEffectParameters();

            float fColorAmount;

            switch (m_colorAmount)
            {
                default:
                    fColorAmount = 0.0f;
                    break;

                case EColorAmounts.Some:
                    fColorAmount = 0.5f;
                    break;

                case EColorAmounts.All:
                    fColorAmount = 1.0f;
                    break;
            }

            // Show only the Textures Color (do not blend with Particle Color)
            Effect.Parameters["xColorBlendAmount"].SetValue(fColorAmount);
        }

        /// <summary>
        /// Function to setup the Render Properties (i.e. BlendState, DepthStencilState, RasterizerState, and SamplerState)
        /// which will be applied to the Graphics Device before drawing the Particle System's Particles.
        /// <para>This function is called when initializing the particle system.</para>
        /// </summary>
        protected override void InitializeRenderProperties()
        {
            base.InitializeRenderProperties();

            // Use the old custom DPSF effect (instead of the Windows-Phone-friendly default SpriteBatch effect) to support the m_colorAmount property.
            SetEffectAndTechnique(DPSFDefaultEffect, DPSFDefaultEffectTechniques.Sprites.ToString());

            // Use DepthRead to eliminate nasty black box artefacts
            RenderProperties.DepthStencilState = DepthStencilState.DepthRead;
            RenderProperties.BlendState = BlendState.Additive;
        }

        //===========================================================
        // Initialization Functions
        //===========================================================

        public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
        {

            InitializeSpriteParticleSystem(graphicsDevice, contentManager, 1000, 50000, "Textures/AnimatedExplosion", spriteBatch);

            m_screenWidth = GraphicsDevice.Viewport.Width;
            m_screenHeight = GraphicsDevice.Viewport.Height;

            LoadExplosionEvents();
        }

        public void LoadExplosionEvents()
        {
            Name = "Animated Sprite Explosion";
            if (EmitterType == ParticleEmitterTypes.Explosion)
            {
                SetTexture("Textures/AnimatedExplosion");
            }

            RemoveAllParticles();
            ParticleInitializationFunction = InitializeParticleAnimatedExplosion;

            ParticleEvents.RemoveAllEvents();
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleDepthFromFrontToBackUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleAnimationAndTextureCoordinates, 500);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToDieOnceAnimationFinishesPlaying, 1000);

            ParticleSystemEvents.RemoveAllEvents();
            ParticleSystemEvents.AddEveryTimeEvent(UpdateParticleSystemToSortParticlesByDepth, 50);

            Emitter.ParticlesPerSecond = 50;
            Emitter.PositionData.Position = new Vector3(-100, -100, 0);

            InitialProperties.LifetimeMin = m_explosionAnimation.TimeRequiredToPlayCurrentAnimation;
            InitialProperties.LifetimeMax = m_explosionAnimation.TimeRequiredToPlayCurrentAnimation;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = Vector3.Zero;
            InitialProperties.VelocityMax = Vector3.Zero;
            InitialProperties.AccelerationMin = Vector3.Zero;
            InitialProperties.AccelerationMax = Vector3.Zero;
            InitialProperties.FrictionMin = 0.0f;
            InitialProperties.FrictionMax = 0.0f;
            InitialProperties.StartColorMin = Color.Red;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.Blue;
            InitialProperties.EndColorMax = Color.White;
            InitialProperties.InterpolateBetweenMinAndMaxColors = false;
            InitialProperties.StartWidthMin = 80.0f;
            InitialProperties.StartWidthMax = 100.0f;
            InitialProperties.StartHeightMin = InitialProperties.StartWidthMin;
            InitialProperties.StartHeightMax = InitialProperties.StartWidthMax;
            InitialProperties.EndWidthMin = 130.0f;
            InitialProperties.EndWidthMax = 150.0f;
            InitialProperties.EndHeightMin = InitialProperties.EndWidthMin;
            InitialProperties.EndHeightMax = InitialProperties.EndWidthMax;
            InitialProperties.RotationMin = 0.0f;
            InitialProperties.RotationMax = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin = 0.0f;
            InitialProperties.RotationalVelocityMax = 0.0f;
            InitialProperties.RotationalAccelerationMin = 0.0f;
            InitialProperties.RotationalAccelerationMax = 0.0f;
        }

        public void InitializeParticleAnimatedExplosion(DefaultAnimatedSpriteParticle particle)
        {
            InitializeParticleUsingInitialProperties(particle);
            particle.Animation.CopyFrom(m_explosionAnimation);
        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        protected void BounceOffScreenEdges(DefaultAnimatedSpriteParticle particle, float elapsedTimeInSeconds)
        {
            int iLeft = (int)(particle.Position.X - (particle.Width / 4));
            int iRight = (int)(particle.Position.X + (particle.Width / 4));
            int iTop = (int)(particle.Position.Y - (particle.Height / 4));
            int iBottom = (int)(particle.Position.Y + (particle.Height / 4));

            // If the Particle is heading off the left or right side of the screen
            if ((iLeft < 0 && particle.Velocity.X < 0) ||
                (iRight > m_screenWidth && particle.Velocity.X > 0))
            {
                // Reverse it's horizontal direction
                particle.Velocity.X *= -1;
            }

            // If the Particle is heading off the top or bottom of the screen
            if ((iTop < 0 && particle.Velocity.Y < 0) ||
                (iBottom > m_screenHeight && particle.Velocity.Y > 0))
            {
                // Reverse it's vertical direction
                particle.Velocity.Y *= -1;
            }
        }


        //===========================================================
        // Particle System Update Functions
        //===========================================================

        //===========================================================
        // Other Particle System Functions
        //===========================================================
        public void ToggleColorAmount()
        {
            m_colorAmount++;

            // If we've gone past the End of the List
            if (m_colorAmount > EColorAmounts.LastInList)
            {
                m_colorAmount = 0;
            }
        }

        public Vector3 MousePosition
        {
            get { return m_mousePosition; }
            set
            {
                Emitter.PositionData.Position = value;

                if (ActiveParticles.Last != null)
                {
                    ActiveParticles.Last.Value.Position = value;
                }
            }
        }
    }
}
