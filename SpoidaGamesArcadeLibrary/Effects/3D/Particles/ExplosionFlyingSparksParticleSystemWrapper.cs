using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
    public class ExplosionFlyingSparksParticleSystemWrapper : ExplosionFlyingSparksParticleSystem, IWrapParticleSystem
    {
        public ExplosionFlyingSparksParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        {
            SetupToAutoExplodeEveryInterval(2);
        }
    }
}
