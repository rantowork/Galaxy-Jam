using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Effects._2D;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class BasketballManager
    {
        private readonly Random m_random = new Random();

        public static Dictionary<BasketballTypes, Basketball> Basketballs = new Dictionary<BasketballTypes, Basketball>();
        public static Dictionary<int, Texture2D> LockedBasketballTextures = new Dictionary<int, Texture2D>();
        public static Dictionary<int, BasketballTypes> BasketballSelection = new Dictionary<int, BasketballTypes>();
        public static List<Basketball> BasketballList = new List<Basketball>();

        public static Basketball SelectedBasketball { get; set; }
        public static Emitter SelectedBasketballEmitter { get; set;}

        public static void SelectBasketball(BasketballTypes type)
        {
            Basketball selectedBall;
            if (Basketballs.TryGetValue(type, out selectedBall))
            {
                SelectedBasketball = selectedBall;
                SelectedBasketballEmitter = ParticleEmitters.GetEmitter(selectedBall.BasketballEmitter);
            }
            else
            {
                SelectedBasketball = Basketballs[0];
                SelectedBasketballEmitter = ParticleEmitters.GetEmitter(SelectedBasketball.BasketballEmitter);
            }
        }

        public Body BasketballBody { get; private set; }

        public BasketballManager(ContentManager content)
        {
            BasketballBody = BodyFactory.CreateCircle(PhysicalWorld.World, 32f / (2f * PhysicalWorld.MetersInPixels), 1.0f, new Vector2((m_random.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (m_random.Next(310, 680)) / PhysicalWorld.MetersInPixels));
            BasketballBody.BodyType = BodyType.Dynamic;
            BasketballBody.Mass = 1f;
            BasketballBody.Restitution = 0.3f;
            BasketballBody.Friction = 0.1f;
            LoadBasketballs(content);
            LoadLockedBasketballs(content);
        }
        
        private static void LoadBasketballs(ContentManager content)
        {
            Basketball redGlowBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RedGlowBall"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "Red Glow Ball", 0, ParticleEmitterTypes.SparkleEmitter);
            Basketball slimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/SlimeBall"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Green Slime Ball", 0, ParticleEmitterTypes.SparkleEmitter);
            Basketball blueSlimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BlueSlimeBall"), new List<Rectangle> { new Rectangle(0, 0, 96, 96) }, false, "Blue Slime Ball", 100000, ParticleEmitterTypes.SparkleEmitter);
            Basketball cuteInPink = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/CuteInPink"), new List<Rectangle> { new Rectangle(0, 0, 96, 96) }, false, "Cute In Pink", 200000, ParticleEmitterTypes.SparkleEmitter);
            Basketball brokenPlanet = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BrokenPlanet"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Broken Planet", 500000, ParticleEmitterTypes.SparkleEmitter);
            Basketball thatsNoMoon = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/ThatsNoMoon"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "That's No Moon!", 750000, ParticleEmitterTypes.SparkleEmitter);
            Basketball earthDay = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/EarthDay"), new List<Rectangle> { new Rectangle(0, 0, 36, 36) }, false, "Earth Day", 1000000, ParticleEmitterTypes.SparkleEmitter);
            Basketball magmaBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/MagmaBall"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Magma Ball", 1500000, ParticleEmitterTypes.SparkleEmitter);

            Basketballs.Add(BasketballTypes.RedGlowBall, redGlowBall);
            Basketballs.Add(BasketballTypes.SlimeBall, slimeBall);
            Basketballs.Add(BasketballTypes.CuteInPink, cuteInPink);
            Basketballs.Add(BasketballTypes.BlueSlimeBall, blueSlimeBall);
            Basketballs.Add(BasketballTypes.BrokenPlanet, brokenPlanet);
            Basketballs.Add(BasketballTypes.ThatsNoMoon, thatsNoMoon);
            Basketballs.Add(BasketballTypes.EarthDay, earthDay);
            Basketballs.Add(BasketballTypes.MagmaBall, magmaBall);

            BasketballList.Add(redGlowBall);
            BasketballList.Add(slimeBall);
            BasketballList.Add(cuteInPink);
            BasketballList.Add(blueSlimeBall);
            BasketballList.Add(brokenPlanet);
            BasketballList.Add(thatsNoMoon);
            BasketballList.Add(earthDay);
            BasketballList.Add(magmaBall);
        }

        private static void LoadLockedBasketballs(ContentManager content)
        {
            LockedBasketballTextures.Add(0, content.Load<Texture2D>(@"Textures/Basketballs/Locked"));
        }
    }

    public enum BasketballTypes
    {
        RedGlowBall,
        SlimeBall,
        BlueSlimeBall,
        CuteInPink,
        BrokenPlanet,
        ThatsNoMoon,
        EarthDay,
        MagmaBall
    }
}
