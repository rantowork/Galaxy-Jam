using DPSF;
using Microsoft.Xna.Framework;
using SpoidaGamesArcadeLibrary.Effects._3D;
using SpoidaGamesArcadeLibrary.Effects._3D.Particles;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ParticleSystems
    {
        private const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 60;
        public static _3DCamera _3DCamera { get; set; }

        public static ParticleSystemManager ParticleSystemManager { get; set; }
        public static TrailParticleSystemWrapper TrailParticleSystemWrapper { get; set; }
        public static IWrapParticleSystem CurrentParticleSystemWrapper { get; set; }

        public static Matrix WorldMatrix { get; set; }
        public static Matrix ViewMatrix { get; set; }
        public static Matrix ProjectionMatrix { get; set; }

        public static void InitializeParticleSystems()
        {
            ParticleSystemManager = new ParticleSystemManager();

            WorldMatrix = Matrix.Identity;

            _3DCamera = new _3DCamera(true);

            ParticleSystemManager.AddParticleSystem(TrailParticleSystemWrapper);
            ParticleSystemManager.UpdatesPerSecond = PARTICLE_SYSTEM_UPDATES_PER_SECOND;
            CurrentParticleSystemWrapper = TrailParticleSystemWrapper;
            const float aspectRatio = 1280 / 720;
            ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -200), new Vector3(0, 0, 0), Vector3.Up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, .01f, 10000f);
        }
    }
}
