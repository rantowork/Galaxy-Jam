using DPSF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects._3D.Particles;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ParticleSystems
    {
        private const int PARTICLE_SYSTEM_UPDATES_PER_SECOND = 60;
        public static _3DCamera _3DCamera { get; set; }

        public static ParticleSystemManager ParticleSystemManager { get; set; }
        public static ParticleSystemManager BallParticleSystemManager { get; set; }

        public static TrailParticleSystemWrapper TrailParticleSystemWrapper { get; set; }
        public static ExplosionFlyingSparksParticleSystemWrapper ExplosionFlyingSparksParticleSystemWrapper { get; set; }
        public static DpsfSplashScreenWrapper DpsfSplashScreenWrapper { get; set; }
        
        public static Matrix WorldMatrix { get; set; }
        public static Matrix ViewMatrix { get; set; }
        public static Matrix ProjectionMatrix { get; set; }

        private static GraphicsDevice s_graphicsDevice;
        private static ContentManager s_contentManager;
        private static Game s_game;

        public static void InitializeParticleSystems(GraphicsDevice graphicsDevice, ContentManager contentManager, Game game)
        {
            s_graphicsDevice = graphicsDevice;
            s_contentManager = contentManager;
            s_game = game;

            ParticleSystemManager = new ParticleSystemManager();
            BallParticleSystemManager = new ParticleSystemManager();

            WorldMatrix = Matrix.Identity;

            _3DCamera = new _3DCamera(true);

            ParticleSystemManager.AddParticleSystem(DpsfSplashScreenWrapper);
            ParticleSystemManager.UpdatesPerSecond = PARTICLE_SYSTEM_UPDATES_PER_SECOND;
            BallParticleSystemManager.UpdatesPerSecond = PARTICLE_SYSTEM_UPDATES_PER_SECOND;

            const float aspectRatio = 1280 / 720;
            ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -200), new Vector3(0, 0, 0), Vector3.Up);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, .01f, 10000f);
        }

        public static void AddStatic3DSystemsToManager()
        {
            ParticleSystemManager.AddParticleSystem(TrailParticleSystemWrapper);
            ParticleSystemManager.AddParticleSystem(ExplosionFlyingSparksParticleSystemWrapper);
        }

        public static AnimatedSpriteParticleSystemWrapper AddSpriteSystemToManager(ParticleEmitterTypes type)
        {
            AnimatedSpriteParticleSystemWrapper wrapper = new AnimatedSpriteParticleSystemWrapper(s_game);
            wrapper.EmitterType = type;
            wrapper.AutoInitialize(s_graphicsDevice, s_contentManager, null);
            wrapper.AfterAutoInitialize();
            BallParticleSystemManager.AddParticleSystem(wrapper);
            return wrapper;
        }
    }
}
