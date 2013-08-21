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
        public static Dictionary<ParticleEmitterTypes, AdvancedParticleSystem> AdvancedEmitterDictionary { get; set; }
        private static Game s_game;

        public static void InitializeAdvancedParticleSystems(Game game)
        {
            s_game = game;
            AdvancedEmitterDictionary = new Dictionary<ParticleEmitterTypes, AdvancedParticleSystem>();

            AdvancedParticleEmitter advancedStar = new AdvancedParticleEmitter();
            advancedStar.ParticleTextureFileName = "Textures/Star5";
            advancedStar.IsBurst = false;
            advancedStar.SetLifeTimes(1.0f, 1.5f);
            advancedStar.SetScales(0.1f, 0.4f);
            advancedStar.ParticlesPerSecond = 100.0f;
            advancedStar.InitialParticleCount = (int)(advancedStar.ParticlesPerSecond * advancedStar.MaximumLifeTime);
            advancedStar.SetDirectionAngles(0, 360);

            AdvancedParticleSystem advancedStarEmitter = new AdvancedParticleSystem(s_game, advancedStar);
            advancedStarEmitter.OriginPosition = new Vector2(-400, -400);
            advancedStarEmitter.ParticleColors.Add(new Color(4, 126, 207));
            
            AdvancedParticleEmitter yinYang = new AdvancedParticleEmitter();
            yinYang.ParticleTextureFileName = "Textures/Spark";
            yinYang.IsBurst = false;
            yinYang.SetLifeTimes(1.0f, 1.5f);
            yinYang.SetScales(0.3f, 0.7f);
            yinYang.ParticlesPerSecond = 100.0f;
            yinYang.InitialParticleCount = (int)(yinYang.ParticlesPerSecond * yinYang.MaximumLifeTime);
            yinYang.SetDirectionAngles(0, 360);

            AdvancedParticleSystem yinYangEmitter = new AdvancedParticleSystem(s_game, yinYang);
            yinYangEmitter.OriginPosition = new Vector2(-400, -400);
            yinYangEmitter.ParticleColors.Add(new Color(182, 22, 42));
            yinYangEmitter.ParticleColors.Add(new Color(18, 40, 255));

            AdvancedParticleEmitter theEye = new AdvancedParticleEmitter();
            theEye.ParticleTextureFileName = "Textures/Spark";
            theEye.IsBurst = false;
            theEye.SetLifeTimes(.3f, .7f);
            theEye.SetScales(.8f, 1.5f);
            theEye.ParticlesPerSecond = 100.0f;
            theEye.InitialParticleCount = (int)(theEye.ParticlesPerSecond * theEye.MaximumLifeTime);
            theEye.SetDirectionAngles(0, 360);

            AdvancedParticleSystem theEyeEmitter = new AdvancedParticleSystem(s_game, theEye);
            theEyeEmitter.OriginPosition = new Vector2(-400, -400);
            theEyeEmitter.ParticleColors.Add(Color.DarkGreen);

            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.AdvancedStar, advancedStarEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.TheYinYang, advancedStarEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.TheEye, advancedStarEmitter);
        }

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

        public static AdvancedParticleSystem LoadAdvancedArcadeEmitter(ParticleEmitterTypes emitterType)
        {
            AdvancedParticleSystem emitterToReturn;
            switch (emitterType)
            {
                case ParticleEmitterTypes.AdvancedStar:

                    AdvancedParticleEmitter advancedStar = new AdvancedParticleEmitter();
                    advancedStar.ParticleTextureFileName = "Textures/Star5";
                    advancedStar.IsBurst = false;
                    advancedStar.SetLifeTimes(1.0f, 1.5f);
                    advancedStar.SetScales(0.1f, 0.4f);
                    advancedStar.ParticlesPerSecond = 100.0f;
                    advancedStar.InitialParticleCount = (int)(advancedStar.ParticlesPerSecond * advancedStar.MaximumLifeTime);
                    advancedStar.SetDirectionAngles(0, 360);

                    AdvancedParticleSystem advancedStarEmitter = new AdvancedParticleSystem(s_game, advancedStar);
                    advancedStarEmitter.OriginPosition = new Vector2(-400, -400);
                    advancedStarEmitter.ParticleColors.Add(new Color(4, 126, 207));
                    emitterToReturn = advancedStarEmitter;

                break;

                case ParticleEmitterTypes.TheYinYang:

                    AdvancedParticleEmitter yinYang = new AdvancedParticleEmitter();
                    yinYang.ParticleTextureFileName = "Textures/Spark";
                    yinYang.IsBurst = false;
                    yinYang.SetLifeTimes(1.0f, 1.5f);
                    yinYang.SetScales(0.3f, 0.7f);
                    yinYang.ParticlesPerSecond = 100.0f;
                    yinYang.InitialParticleCount = (int)(yinYang.ParticlesPerSecond * yinYang.MaximumLifeTime);
                    yinYang.SetDirectionAngles(0, 360);

                    AdvancedParticleSystem yinYangEmitter = new AdvancedParticleSystem(s_game, yinYang);
                    yinYangEmitter.OriginPosition = new Vector2(-400, -400);
                    yinYangEmitter.ParticleColors.Add(new Color(182, 22, 42));
                    yinYangEmitter.ParticleColors.Add(new Color(18, 40, 255));
                    emitterToReturn = yinYangEmitter;

                break;


                case ParticleEmitterTypes.TheEye:

                AdvancedParticleEmitter theEye = new AdvancedParticleEmitter();
                theEye.ParticleTextureFileName = "Textures/Smoke";
                theEye.IsBurst = false;
                theEye.SetLifeTimes(.3f, 1.5f);
                theEye.SetScales(.6f, 1.2f);
                theEye.ParticlesPerSecond = 100.0f;
                theEye.InitialParticleCount = (int)(theEye.ParticlesPerSecond * theEye.MaximumLifeTime);
                theEye.SetDirectionAngles(0, 360);

                AdvancedParticleSystem theEyeEmitter = new AdvancedParticleSystem(s_game, theEye);
                theEyeEmitter.OriginPosition = new Vector2(-400, -400);
                theEyeEmitter.ParticleColors.Add(Color.DarkGreen);
                emitterToReturn = theEyeEmitter;

                break;

                default:
                    AdvancedParticleEmitter defaultEmitter = new AdvancedParticleEmitter();
                    defaultEmitter.ParticleTextureFileName = "Textures/Star5";
                    defaultEmitter.IsBurst = false;
                    defaultEmitter.SetLifeTimes(1.0f, 1.5f);
                    defaultEmitter.SetScales(0.1f, 0.4f);
                    defaultEmitter.ParticlesPerSecond = 100.0f;
                    defaultEmitter.InitialParticleCount = (int)(defaultEmitter.ParticlesPerSecond * defaultEmitter.MaximumLifeTime);
                    defaultEmitter.SetDirectionAngles(0, 360);
                    
                    AdvancedParticleSystem defaultStarEmitter = new AdvancedParticleSystem(s_game, defaultEmitter);
                    defaultStarEmitter.OriginPosition = new Vector2(-400, -400);
                    defaultStarEmitter.ParticleColors.Add(Color.White);
                    emitterToReturn = defaultStarEmitter;

                break;
            }
            return emitterToReturn;
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

        public static bool IsEmitterClassic(ParticleEmitterTypes emitterType)
        {
            if (emitterType == ParticleEmitterTypes.AdvancedStar || emitterType == ParticleEmitterTypes.TheYinYang || emitterType == ParticleEmitterTypes.TheEye)
            {
                return false;
            }
            return true;
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
        AdvancedStar,
        TheYinYang,
        TheEye
    }
}
