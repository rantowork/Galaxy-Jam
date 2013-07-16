using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class ArcadeModeScreenState
    {
        private static List<Basketball> s_activeBasketballs = new List<Basketball>();
        private const int BASKETBALL_SPAWN_TIMER = 400;
        private static int s_basketballTimer = 0;

        public static BasketballTypes SelectedBallType { get; set; }

        public static void Update(GameTime gameTime)
        {
            PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            if (s_basketballTimer == 0)
            {

            }

            s_basketballTimer += gameTime.ElapsedGameTime.Milliseconds;

            if (s_basketballTimer >= BASKETBALL_SPAWN_TIMER)
            {
                s_basketballTimer = 0;
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

        }
    }
}
