using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
    public class DpsfSplashScreenWrapper : DpsfSplashParticleSystem, IWrapParticleSystem
    {
        public DpsfSplashScreenWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }

	    public void ProcessInput()
	    { }
    }
}
