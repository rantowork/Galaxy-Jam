using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public class BasketballManager
    {
        private Random random = new Random();

        public static Dictionary<BasketballTypes, Basketball> basketballs = new Dictionary<BasketballTypes, Basketball>();
        public static Dictionary<int, Texture2D> lockedBasketballTextures = new Dictionary<int, Texture2D>();
        public static Dictionary<int, BasketballTypes> basketballSelection = new Dictionary<int, BasketballTypes>();
        public static List<Basketball> basketballList = new List<Basketball>();

        private static Basketball selectedBasketball;
        public static Basketball SelectedBasketball
        {
            get { return selectedBasketball; }
            set { selectedBasketball = value; }
        }

        public void SelectBasketball(BasketballTypes type)
        {
            Basketball selectedBall;
            if (basketballs.TryGetValue(type, out selectedBall))
            {
                SelectedBasketball = selectedBall;
            }
            else
            {
                selectedBasketball = basketballs[0];
            }
        }

        private Body basketballBody;
        public Body BasketballBody
        {
            get { return basketballBody; }
        }
        
        public BasketballManager(ContentManager content)
        {
            basketballBody = BodyFactory.CreateCircle(PhysicalWorld.World, 32f / (2f * PhysicalWorld.MetersInPixels), 1.0f, new Vector2((random.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (random.Next(310, 680)) / PhysicalWorld.MetersInPixels));
            basketballBody.BodyType = BodyType.Dynamic;
            basketballBody.Restitution = 0.3f;
            basketballBody.Friction = 0.1f;
            LoadBasketballs(content);
            LoadLockedBasketballs(content);
        }

        private static void LoadBasketballs(ContentManager content)
        {
            Basketball redGlowBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RedGlowBall"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "Red Glow Ball", 0);
            Basketball slimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/SlimeBall"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Green Slime Ball", 0);
            Basketball redSlimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RedSlimeBall"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Red Slime Ball", 10000);
            Basketball cuteInPink = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/CuteInPink"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Cute In Pink", 20000);
            Basketball blueSlimeBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BlueSlimeBall"), new List<Rectangle> {new Rectangle(0, 0, 96, 96)}, false, "Blue Slime Ball", 30000);
            Basketball brokenPlanet = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/BrokenPlanet"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "Broken Planet", 500000);
            Basketball thatsNoMoon = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/ThatsNoMoon"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "That's No Moon!", 750000);
            Basketball earthDay = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/EarthDay"), new List<Rectangle> {new Rectangle(0, 0, 36, 36)}, false, "Earth Day", 1000000);
            Basketball magmaBall = new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/MagmaBall"), new List<Rectangle> {new Rectangle(0, 0, 64, 64)}, false, "Magma Ball", 1500000);

            basketballs.Add(BasketballTypes.RedGlowBall, redGlowBall);
            basketballs.Add(BasketballTypes.SlimeBall, slimeBall);
            basketballs.Add(BasketballTypes.RedSlimeBall, redSlimeBall);
            basketballs.Add(BasketballTypes.CuteInPink, cuteInPink);
            basketballs.Add(BasketballTypes.BlueSlimeBall, blueSlimeBall);
            basketballs.Add(BasketballTypes.BrokenPlanet, brokenPlanet);
            basketballs.Add(BasketballTypes.ThatsNoMoon, thatsNoMoon);
            basketballs.Add(BasketballTypes.EarthDay, earthDay);
            basketballs.Add(BasketballTypes.MagmaBall, magmaBall);

            basketballList.Add(redGlowBall);
            basketballList.Add(slimeBall);
            basketballList.Add(redSlimeBall);
            basketballList.Add(cuteInPink);
            basketballList.Add(blueSlimeBall);
            basketballList.Add(brokenPlanet);
            basketballList.Add(thatsNoMoon);
            basketballList.Add(earthDay);
            basketballList.Add(magmaBall);
        }

        private static void LoadLockedBasketballs(ContentManager content)
        {
            lockedBasketballTextures.Add(0, content.Load<Texture2D>(@"Textures/Basketballs/Locked"));
        }
    }

    public enum BasketballTypes
    {
        RedGlowBall,
        SlimeBall,
        RedSlimeBall,
        CuteInPink,
        BlueSlimeBall,
        BrokenPlanet,
        ThatsNoMoon,
        EarthDay,
        MagmaBall
    }
}
