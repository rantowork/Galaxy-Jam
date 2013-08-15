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

            ParticleEmitter.Add(ParticleEmitterTypes.None, new Emitter(null, new Vector2(0,0), 0, new List<Color>{Color.White}, ParticleEmitterTypes.None, false));
            ParticleEmitter.Add(ParticleEmitterTypes.Explosion, new Emitter(null, new Vector2(0, 0), 0, new List<Color> { Color.White }, ParticleEmitterTypes.None, false));
            ParticleEmitter.Add(ParticleEmitterTypes.SparkleEmitter,
                new Emitter(new List<Texture2D> { Textures.Twopxsolidstar },
                    new Vector2(-40, -40),
                    125,
                    new List<Color> { Color.DarkRed, Color.DarkOrange },
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
                new Emitter(new List<Texture2D> { Textures.Star },
                    new Vector2(-40, -40),
                    15,
                    new List<Color> { Color.Pink, Color.HotPink, Color.LightPink },
                    ParticleEmitterTypes.StarEmitter,
                    false));
        }

        public static Emitter LoadArcadeEmitter(ParticleEmitterTypes emitterType)
        {
            Emitter emitterToReturn;
            switch (emitterType)
            {
                case ParticleEmitterTypes.None:
                    emitterToReturn = new Emitter(null, new Vector2(0, 0), 0, new List<Color> {Color.White},
                                                  ParticleEmitterTypes.None, false);
                    break;
                case ParticleEmitterTypes.CombusionEmitter:
                    emitterToReturn = new Emitter(new List<Texture2D> {Textures.Explosion},
                                                  new Vector2(-40, -40),
                                                  50,
                                                  new List<Color> {Color.DarkRed, Color.OrangeRed},
                                                  ParticleEmitterTypes.CombusionEmitter,
                                                  false);
                    break;
                case ParticleEmitterTypes.SparkleEmitter:
                    emitterToReturn = new Emitter(new List<Texture2D> {Textures.Twopxsolidstar},
                                                  new Vector2(-40, -40),
                                                  75,
                                                  new List<Color> {Color.DarkRed, Color.DarkOrange},
                                                  ParticleEmitterTypes.SparkleEmitter,
                                                  true);
                    break;
                case ParticleEmitterTypes.StarEmitter:
                    emitterToReturn = new Emitter(new List<Texture2D> { Textures.Star },
                                                  new Vector2(-40, -40),
                                                  15,
                                                  new List<Color> { Color.Pink, Color.HotPink, Color.LightPink },
                                                  ParticleEmitterTypes.StarEmitter,
                                                  false);
                    break;
                case ParticleEmitterTypes.Explosion:
                    emitterToReturn = new Emitter(null, new Vector2(0, 0), 0, new List<Color> { Color.White },
                                                  ParticleEmitterTypes.None, false);
                    break;
                default:
                    emitterToReturn = new Emitter(new List<Texture2D> {Textures.Twopxsolidstar},
                                                  new Vector2(-40, -40),
                                                  75,
                                                  new List<Color> {Color.DarkRed, Color.DarkOrange},
                                                  ParticleEmitterTypes.SparkleEmitter,
                                                  true);
                    break;
            }
            return emitterToReturn;
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
        None,
        SparkleEmitter,
        CombusionEmitter,
        StarEmitter,
        HoopEmitter,
        Explosion,
    }
}
