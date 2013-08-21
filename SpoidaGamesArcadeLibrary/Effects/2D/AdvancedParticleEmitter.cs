namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class AdvancedParticleEmitter
    {
        public AdvancedParticleEmitter()
        {
            InitialParticleCount = 200;
            ParticlesPerSecond = 100.0f;
            ParticleTextureFileName = null;
            MinimumDirectionAngle = 0.0f;
            MaximumDirectionAngle = 360.0f;
            MinimumSpeed = 50.0f;
            MaximumSpeed = 100.0f;
            MinimumLifeTime = 1.0f;
            MaximumLifeTime = 2.5f;
            MinimumScale = 0.1f;
            MaximumScale = 1.0f;
            IsBurst = false;
            EndBurst = false;
        }

        /// <summary>
        /// Set the minimum and maximum of the velocity angles of a particle
        /// </summary>
        /// <param name="min">minimum angle</param>
        /// <param name="max">maximum angle</param>
        public void SetDirectionAngles(float min, float max)
        {
            MinimumDirectionAngle = min;
            MaximumDirectionAngle = max;
        }

        /// <summary>
        /// Set how fast the particle should travel
        /// </summary>
        /// <param name="min">minimum speed of the particle</param>
        /// <param name="max">maximum speed of the particle</param>
        public void SetSpeeds(float min, float max)
        {
            MinimumSpeed = min;
            MaximumSpeed = max;
        }

        /// <summary>
        /// Set the amount of time the particle should remain active
        /// </summary>
        /// <param name="min">minimum life time of the particle</param>
        /// <param name="max">maximum life time of the particle</param>
        public void SetLifeTimes(float min, float max)
        {
            MinimumLifeTime = min;
            MaximumLifeTime = max;
        }

        /// <summary>
        /// Set the size of the particle
        /// </summary>
        /// <param name="min">minimum size of the particle</param>
        /// <param name="max">maximum size of the particle</param>
        public void SetScales(float min, float max)
        {
            MinimumScale = min;
            MaximumScale = max;
        }

        //*****************
        // Properties
        //*****************

        public int InitialParticleCount { get; set; }
        public float ParticlesPerSecond { get; set; }
        public string ParticleTextureFileName { get; set; }
        public float MinimumDirectionAngle { get; set; }
        public float MaximumDirectionAngle { get; set; }
        public float MinimumSpeed { get; set; }
        public float MaximumSpeed { get; set; }
        public float MinimumLifeTime { get; set; }
        public float MaximumLifeTime { get; set; }
        public float MinimumScale { get; set; }
        public float MaximumScale { get; set; }
        public bool IsBurst { get; set; }
        public bool EndBurst { get; set; }
        public double BurstCooldown { get; set; }
    }
}
