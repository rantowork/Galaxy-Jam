﻿using System;
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

        public static void SelectBasketball(BasketballTypes type)
        {
            Basketball selectedBall;
            if (Basketballs.TryGetValue(type, out selectedBall))
            {
                SelectedBasketball = selectedBall;
            }
            else
            {
                SelectedBasketball = Basketballs[0];
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
            Basketball regularBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RegularBall"), new List<Rectangle> {new Rectangle(0,0,64,64)}, false, "Basketball", 0, ParticleEmitterTypes.SparkleEmitter);
            Basketball redGlowBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RedGlowBall"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "Red Glow Ball", 0, ParticleEmitterTypes.SparkleEmitter);
            Basketball slimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/SlimeBall"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Green Slime Ball", 0, ParticleEmitterTypes.GreenSlime);
            Basketball blueSlimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BlueSlimeBall"), new List<Rectangle> { new Rectangle(0, 0, 96, 96) }, false, "Blue Slime Ball", 250000, ParticleEmitterTypes.BlueSlime);
            Basketball cuteInPink = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/CuteInPink"), new List<Rectangle> { new Rectangle(0, 0, 96, 96) }, false, "Cute In Pink", 500000, ParticleEmitterTypes.StarEmitter);
            Basketball brokenPlanet = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BrokenPlanet"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Broken Planet", 1000000, ParticleEmitterTypes.SparkleEmitter);
            Basketball thatsNoMoon = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/ThatsNoMoon"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "That's No Moon!", 2500000, ParticleEmitterTypes.SparkleEmitter);
            Basketball earthDay = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/EarthDay"), new List<Rectangle> { new Rectangle(0, 0, 36, 36) }, false, "Earth Day", 4000000, ParticleEmitterTypes.AdvancedStar);
            Basketball sawBlade = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/Saw"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Sawvinguard", 5000000, ParticleEmitterTypes.None);
            Basketball distantStar = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/DistantStar"), new List<Rectangle> { new Rectangle(0, 0, 128, 128) }, false, "Distant Star", 7500000, ParticleEmitterTypes.BrightStar);
            Basketball magmaBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/MagmaBall"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Magma Ball", 10000000, ParticleEmitterTypes.Explosion);
            Basketball fireYang = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/FireYang"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Fire & Ice", 15000000, ParticleEmitterTypes.TheYinYang);
            Basketball eternalEye = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/EternalEye"), new List<Rectangle> { new Rectangle(0, 0, 64, 64) }, false, "Eternal Eye", 20000000, ParticleEmitterTypes.TheEye);

            Basketballs.Add(BasketballTypes.RegularBall, regularBall);
            Basketballs.Add(BasketballTypes.RedGlowBall, redGlowBall);
            Basketballs.Add(BasketballTypes.SlimeBall, slimeBall);
            Basketballs.Add(BasketballTypes.CuteInPink, cuteInPink);
            Basketballs.Add(BasketballTypes.BlueSlimeBall, blueSlimeBall);
            Basketballs.Add(BasketballTypes.BrokenPlanet, brokenPlanet);
            Basketballs.Add(BasketballTypes.ThatsNoMoon, thatsNoMoon);
            Basketballs.Add(BasketballTypes.EarthDay, earthDay);
            Basketballs.Add(BasketballTypes.SawBlade, sawBlade);
            Basketballs.Add(BasketballTypes.DistantStar, distantStar);
            Basketballs.Add(BasketballTypes.MagmaBall, magmaBall);
            Basketballs.Add(BasketballTypes.FireYang, fireYang);
            Basketballs.Add(BasketballTypes.EternalEye, eternalEye);

            BasketballList.Add(regularBall);
            BasketballList.Add(redGlowBall);
            BasketballList.Add(slimeBall);
            BasketballList.Add(cuteInPink);
            BasketballList.Add(blueSlimeBall);
            BasketballList.Add(brokenPlanet);
            BasketballList.Add(thatsNoMoon);
            BasketballList.Add(earthDay);
            BasketballList.Add(sawBlade);
            BasketballList.Add(distantStar);
            BasketballList.Add(magmaBall);
            BasketballList.Add(fireYang);
            BasketballList.Add(eternalEye);
        }

        private static void LoadLockedBasketballs(ContentManager content)
        {
            LockedBasketballTextures.Add(0, content.Load<Texture2D>(@"Textures/Basketballs/Locked"));
        }
    }

    public enum BasketballTypes
    {
        RegularBall,
        RedGlowBall,
        SlimeBall,
        BlueSlimeBall,
        CuteInPink,
        BrokenPlanet,
        ThatsNoMoon,
        EarthDay,
        SawBlade,
        DistantStar,
        MagmaBall,
        FireYang,
        EternalEye,
    }
}
