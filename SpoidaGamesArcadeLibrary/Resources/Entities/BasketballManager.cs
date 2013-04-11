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

        public Dictionary<BasketballTypes, Basketball> basketballs = new Dictionary<BasketballTypes, Basketball>();

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
        
        public BasketballManager()
        {
            basketballBody = BodyFactory.CreateCircle(PhysicalWorld.World, 32f / (2f * PhysicalWorld.MetersInPixels), 1.0f, new Vector2((random.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (random.Next(310, 680)) / PhysicalWorld.MetersInPixels));
            basketballBody.BodyType = BodyType.Dynamic;
            basketballBody.Restitution = 0.3f;
            basketballBody.Friction = 0.1f;
        }

        public void LoadBasketballs(ContentManager content)
        {
            basketballs.Add(BasketballTypes.RedGlowBall, new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/RedGlowBall"), new List<Rectangle> {new Rectangle(0, 0, 48, 48)}, false));
            basketballs.Add(BasketballTypes.GreenGlowBall, new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/GreenGlowBall"), new List<Rectangle> { new Rectangle(0, 0, 48, 48) }, false));
            basketballs.Add(BasketballTypes.YellowGlowBall, new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/YellowGlowBall"), new List<Rectangle> { new Rectangle(0, 0, 48, 48) }, false));
            basketballs.Add(BasketballTypes.PurpleSkullBall, new Basketball(content.Load<Texture2D>(@"Textures/Basketballs/PurpleSkull"), new List<Rectangle> { new Rectangle(0, 0, 64, 64), new Rectangle(64,0,64,64) }, true));
        }
    }

    public enum BasketballTypes
    {
        RedGlowBall,
        GreenGlowBall,
        YellowGlowBall,
        PurpleSkullBall
    }
}
