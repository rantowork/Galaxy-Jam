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
            theEye.ParticleTextureFileName = "Textures/Smoke";
            theEye.IsBurst = false;
            theEye.SetLifeTimes(.3f, .7f);
            theEye.SetScales(.8f, 1.5f);
            theEye.ParticlesPerSecond = 100.0f;
            theEye.InitialParticleCount = (int)(theEye.ParticlesPerSecond * theEye.MaximumLifeTime);
            theEye.SetDirectionAngles(0, 360);

            AdvancedParticleSystem theEyeEmitter = new AdvancedParticleSystem(s_game, theEye);
            theEyeEmitter.OriginPosition = new Vector2(-400, -400);
            theEyeEmitter.ParticleColors.Add(Color.DarkGreen);

            AdvancedParticleEmitter star = new AdvancedParticleEmitter();
            star.ParticleTextureFileName = "Textures/Spark";
            star.IsBurst = true;
            star.SetLifeTimes(.4f, .4f);
            star.SetScales(.6f, .6f);
            star.ParticlesPerSecond = 200.0f;
            star.InitialParticleCount = (int)(star.ParticlesPerSecond * star.MaximumLifeTime);
            star.SetDirectionAngles(0, 360);
            star.BurstCooldown = 200f;

            AdvancedParticleSystem starEmitter = new AdvancedParticleSystem(s_game, star);
            starEmitter.OriginPosition = new Vector2(-400, -400);
            starEmitter.ParticleColors.Add(Color.DeepPink);

            AdvancedParticleEmitter theFire = new AdvancedParticleEmitter();
            theFire.ParticleTextureFileName = "Textures/Splat";
            theFire.IsBurst = false;
            theFire.SetLifeTimes(.8f, 1.3f);
            theFire.SetScales(0.3f, .5f);
            theFire.ParticlesPerSecond = 50.0f;
            theFire.InitialParticleCount = (int)(theFire.ParticlesPerSecond * theFire.MaximumLifeTime);
            theFire.SetDirectionAngles(250, 285);

            AdvancedParticleSystem theFireEmitter = new AdvancedParticleSystem(s_game, theFire);
            theFireEmitter.OriginPosition = new Vector2(-400, -400);
            theFireEmitter.ParticleColors.Add(Color.DarkBlue);

            AdvancedParticleEmitter greenSlime = new AdvancedParticleEmitter();
            greenSlime.ParticleTextureFileName = "Textures/Splat";
            greenSlime.IsBurst = false;
            greenSlime.SetLifeTimes(.8f, 1.3f);
            greenSlime.SetScales(0.3f, .5f);
            greenSlime.ParticlesPerSecond = 50.0f;
            greenSlime.InitialParticleCount = (int)(greenSlime.ParticlesPerSecond * greenSlime.MaximumLifeTime);
            greenSlime.SetDirectionAngles(250, 285);

            AdvancedParticleSystem greenSlimeEmitter = new AdvancedParticleSystem(s_game, greenSlime);
            greenSlimeEmitter.OriginPosition = new Vector2(-400, -400);
            greenSlimeEmitter.ParticleColors.Add(Color.DarkGreen);
            
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.AdvancedStar, advancedStarEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.TheYinYang, advancedStarEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.TheEye, advancedStarEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.BrightStar, starEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.BlueSlime, theFireEmitter);
            AdvancedEmitterDictionary.Add(ParticleEmitterTypes.GreenSlime, greenSlimeEmitter);
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
                    advancedStar.ParticleTextureFileName = "Textures/Star3";
                    advancedStar.IsBurst = false;
                    advancedStar.SetLifeTimes(1.0f, 1.5f);
                    advancedStar.SetScales(0.4f, .8f);
                    advancedStar.ParticlesPerSecond = 100.0f;
                    advancedStar.InitialParticleCount = (int)(advancedStar.ParticlesPerSecond * advancedStar.MaximumLifeTime);
                    advancedStar.SetDirectionAngles(0, 360);

                    AdvancedParticleSystem advancedStarEmitter = new AdvancedParticleSystem(s_game, advancedStar);
                    advancedStarEmitter.OriginPosition = new Vector2(-400, -400);
                    advancedStarEmitter.ParticleColors.Add(new Color(4, 126, 207));
                    emitterToReturn = advancedStarEmitter;

                break;

                case ParticleEmitterTypes.BlueSlime:

                    AdvancedParticleEmitter theFire = new AdvancedParticleEmitter();
                    theFire.ParticleTextureFileName = "Textures/Splat";
                    theFire.IsBurst = false;
                    theFire.SetLifeTimes(.8f, 1.3f);
                    theFire.SetScales(0.3f, .5f);
                    theFire.ParticlesPerSecond = 50.0f;
                    theFire.InitialParticleCount = (int)(theFire.ParticlesPerSecond * theFire.MaximumLifeTime);
                    theFire.SetDirectionAngles(250, 285);

                    AdvancedParticleSystem theFireEmitter = new AdvancedParticleSystem(s_game, theFire);
                    theFireEmitter.OriginPosition = new Vector2(-400, -400);
                    theFireEmitter.ParticleColors.Add(Color.DarkBlue);
                    emitterToReturn = theFireEmitter;

                break;

                case ParticleEmitterTypes.GreenSlime:

                    AdvancedParticleEmitter greenSlime = new AdvancedParticleEmitter();
                    greenSlime.ParticleTextureFileName = "Textures/Splat";
                    greenSlime.IsBurst = false;
                    greenSlime.SetLifeTimes(.8f, 1.3f);
                    greenSlime.SetScales(0.3f, .5f);
                    greenSlime.ParticlesPerSecond = 50.0f;
                    greenSlime.InitialParticleCount = (int)(greenSlime.ParticlesPerSecond * greenSlime.MaximumLifeTime);
                    greenSlime.SetDirectionAngles(250, 285);

                    AdvancedParticleSystem greenSlimeEmitter = new AdvancedParticleSystem(s_game, greenSlime);
                    greenSlimeEmitter.OriginPosition = new Vector2(-400, -400);
                    greenSlimeEmitter.ParticleColors.Add(Color.DarkGreen);
                    emitterToReturn = greenSlimeEmitter;

                break;

                case ParticleEmitterTypes.TheYinYang:

                    AdvancedParticleEmitter yinYang = new AdvancedParticleEmitter();
                    yinYang.ParticleTextureFileName = "Textures/Spark";
                    yinYang.IsBurst = false;
                    yinYang.SetLifeTimes(1.0f, 1.5f);
                    yinYang.SetScales(0.4f, .8f);
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

                case ParticleEmitterTypes.Sawvinguard:

                    AdvancedParticleEmitter saw = new AdvancedParticleEmitter();
                    saw.ParticleTextureFileName = "Textures/Gear5";
                    saw.IsBurst = true;
                    saw.SetLifeTimes(.5f, .5f);
                    saw.SetScales(.6f, .6f);
                    saw.ParticlesPerSecond = 50.0f;
                    saw.InitialParticleCount = (int)(saw.ParticlesPerSecond * saw.MaximumLifeTime);
                    saw.SetDirectionAngles(0, 360);
                    saw.BurstCooldown = 1000f;

                    AdvancedParticleSystem sawEmitter = new AdvancedParticleSystem(s_game, saw);
                    sawEmitter.OriginPosition = new Vector2(-400, -400);
                    sawEmitter.ParticleColors.Add(Color.CornflowerBlue);
                    emitterToReturn = sawEmitter;

                break;

                case ParticleEmitterTypes.BrightStar:

                    AdvancedParticleEmitter star = new AdvancedParticleEmitter();
                    star.ParticleTextureFileName = "Textures/Spark";
                    star.IsBurst = true;
                    star.SetLifeTimes(.4f, .4f);
                    star.SetScales(.6f, .6f);
                    star.ParticlesPerSecond = 200.0f;
                    star.InitialParticleCount = (int)(star.ParticlesPerSecond * star.MaximumLifeTime);
                    star.SetDirectionAngles(0, 360);
                    star.BurstCooldown = 200f;

                    AdvancedParticleSystem starEmitter = new AdvancedParticleSystem(s_game, star);
                    starEmitter.OriginPosition = new Vector2(-400, -400);
                    starEmitter.ParticleColors.Add(Color.DeepPink);
                    emitterToReturn = starEmitter;

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
            if (emitterType == ParticleEmitterTypes.AdvancedStar || emitterType == ParticleEmitterTypes.TheYinYang || emitterType == ParticleEmitterTypes.TheEye || emitterType == ParticleEmitterTypes.Sawvinguard || emitterType == ParticleEmitterTypes.BrightStar || emitterType == ParticleEmitterTypes.BlueSlime || emitterType == ParticleEmitterTypes.GreenSlime)
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
        TheEye,
        Sawvinguard,
        BrightStar,
        BlueSlime,
        GreenSlime
    }
}
