using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class ArcadeModeScreenState
    {
        private static readonly Random s_random = new Random();
        private static readonly List<ArcadeBasketball> s_activeBasketballs = new List<ArcadeBasketball>();
        private const int BASKETBALL_SPAWN_TIMER = 660;
        private static int s_basketballTimer = 0;
        private static bool s_readyToFire = true;
        private static Vector2 s_cachedShootableBasketballLocation;

        public static Basketball PlayerSelectedBall { get; set; }

        public static void Update(GameTime gameTime)
        {
            PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            if (s_readyToFire)
            {
                s_readyToFire = false;
                SpawnNewBasketball();
            }

            if (!s_readyToFire)
            {
                s_basketballTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (s_basketballTimer >= BASKETBALL_SPAWN_TIMER)
                {
                    s_basketballTimer = 0;
                    s_readyToFire = true;
                }
            }

            foreach (ArcadeBasketball basketball in s_activeBasketballs)
            {
                basketball.Update(gameTime);
                if (basketball.BasketballBody.Position.Y > 720/PhysicalWorld.MetersInPixels)
                {
                    s_activeBasketballs.Remove(basketball);
                }
            }

            if (PhysicalWorld.BackboardCollisionHappened)
            {
                PhysicalWorld.GlowBackboard(gameTime);
            }

            if (PhysicalWorld.LeftRimCollisionHappened)
            {
                PhysicalWorld.GlowLeftRim(gameTime);
            }

            if (PhysicalWorld.RightRimCollisionHappened)
            {
                PhysicalWorld.GlowRightRim(gameTime);
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }

        private static void SpawnNewBasketball()
        {
            ArcadeBasketball newBall = new ArcadeBasketball(PlayerSelectedBall.BasketballTexture, PlayerSelectedBall.FrameList, PlayerSelectedBall.BasketballEmitter);
            s_cachedShootableBasketballLocation = new Vector2((s_random.Next(400, 1200)) / PhysicalWorld.MetersInPixels, (s_random.Next(310, 650)) / PhysicalWorld.MetersInPixels);
            newBall.BasketballBody.Awake = false;
            newBall.BasketballBody.Position = s_cachedShootableBasketballLocation;
            newBall.HasBallScored = false;
            s_activeBasketballs.Add(newBall);
        }

        private static void HandlePlayerInput()
        {
            MouseState state = Screen.Input.GetMouse().GetState();

            
        }
    }
}
