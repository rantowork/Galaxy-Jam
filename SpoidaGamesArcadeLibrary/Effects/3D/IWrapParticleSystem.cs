using DPSF;

namespace SpoidaGamesArcadeLibrary.Effects._3D
{
    public interface IWrapParticleSystem : IDPSFParticleSystem
    {
        /// <summary>
        /// Gets the DPSF Demo Friendly name of the particle system.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Perform any DPSF Demo specific setup after being Auto Initialized.
        /// </summary>
        void AfterAutoInitialize();
    }
}
