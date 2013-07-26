using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ArcadeGoalManager
    {
        public static int Streak { get; set; }
        public static Queue<PowerUpTypes> ActivePowerUps = new Queue<PowerUpTypes>();

        private static Random s_random = new Random();
        private static readonly Dictionary<int, PowerUpTypes> s_powerUps = new Dictionary<int, PowerUpTypes>();
        private const double POWER_UP_TIME_REMAINING = 10000;

        public static void Update(GameTime gameTime)
        {
            if (Streak%3 == 0)
            {

            }
            if (Streak == 3 || Streak == 6 || Streak == 9 || Streak == 15)
            {
                SoundManager.PlaySoundEffect(Sounds.StreakWubSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
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
