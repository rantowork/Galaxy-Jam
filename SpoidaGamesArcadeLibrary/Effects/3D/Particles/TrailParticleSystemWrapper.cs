using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Effects._3D.Particles
{
#if (WINDOWS)
    [Serializable]
#endif
    public class TrailParticleSystemWrapper : TrailParticleSystem, IWrapParticleSystem
    {
        public TrailParticleSystemWrapper(Game game) : base(game)
        {
        }

        public void AfterAutoInitialize()
        {
        }
    }
}
