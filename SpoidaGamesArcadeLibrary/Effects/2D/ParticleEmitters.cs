using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class ParticleEmitters
    {
        public static Dictionary<ParticleEmitterTypes, Emitter> ParticleEmitter { get; set; }
        private static readonly Random s_random = new Random();

        public static void LoadEmitters(ContentManager content)
        {
            ParticleEmitter = new Dictionary<ParticleEmitterTypes, Emitter>();

            ParticleEmitter.Add(ParticleEmitterTypes.SparkleEmitter, 
                new Emitter(new List<Texture2D> {Textures.Twopxsolidstar}, 
                    new Vector2(-40, -40), 
                    150, 
                    new List<Color> {Color.DarkRed, Color.DarkOrange},
                    ParticleEmitterTypes.SparkleEmitter,
                    true));

            ParticleEmitter.Add(ParticleEmitterTypes.CombusionEmitter, 
                new Emitter(new List<Texture2D> { Textures.Explosion }, 
                    new Vector2(-40, -40), 
                    50,
                    new List<Color> { Color.DarkRed, Color.DarkOrange },
                    ParticleEmitterTypes.CombusionEmitter,
                    true));

            ParticleEmitter.Add(ParticleEmitterTypes.StarEmitter, 
                new Emitter(new List<Texture2D> {Textures.Star},
                    new Vector2(-40,-40),
                    15,
                    new List<Color>{ Color.Pink, Color.HotPink, Color.LightPink },
                    ParticleEmitterTypes.StarEmitter,
                    false));
        }

        public static Emitter GetEmitter(ParticleEmitterTypes emitterType)
        {
            Emitter emitterToUpdate;
            if (ParticleEmitter.TryGetValue(emitterType, out emitterToUpdate))
            {
                return emitterToUpdate;
            }
            return ParticleEmitter[0];
        }
    }

    public enum ParticleEmitterTypes
    {
        SparkleEmitter,
        CombusionEmitter,
        StarEmitter,
        HoopEmitter
    }
}
