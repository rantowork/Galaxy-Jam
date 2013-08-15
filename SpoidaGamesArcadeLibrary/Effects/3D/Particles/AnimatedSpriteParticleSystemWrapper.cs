using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
    public class AnimatedSpriteParticleSystemWrapper : AnimatedSpriteParticleSystem, IWrapParticleSystem
    {
        public AnimatedSpriteParticleSystemWrapper(Game cGame)
            : base(cGame)
        { }

        public void AfterAutoInitialize()
        { }
    }
}
