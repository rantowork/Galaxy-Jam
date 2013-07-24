using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ArcadeGoalManager
    {
        public static int Streak { get; set; }
        public static Queue<PowerUpTypes> ActivePowerUps = new Queue<PowerUpTypes>();

        private static Random s_random = new Random();
        private static readonly Dictionary<int, PowerUpTypes> s_powerUps = new Dictionary<int, PowerUpTypes>();
        private const double POWER_UP_TIME_REMAINING = 10000;

        public static void Update(GameTime gameTim)
        {
            if (Streak%3 == 0)
            {

            }
        }

        public static void LoadPowerUps()
        {
            s_powerUps.Add(1,PowerUpTypes.LaserSight);
        }
    }

    public enum PowerUpTypes
    {
        LaserSight,
    }
}
