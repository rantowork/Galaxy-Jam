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

            ParticleEmitter.Add(ParticleEmitterTypes.SparkleEmitter, new Emitter(new List<Texture2D> {Textures.Twopxsolidstar}, new Vector2(-40, -40), 150, new List<Color> {Color.DarkRed, Color.DarkOrange}, (0.1f*(float)(s_random.NextDouble()*2 - 1))));
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
        HoopEmitter
    }
}
