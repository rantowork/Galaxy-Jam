using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ArcadeGoalManager
    {
        public static int Streak { get; set; }
        public static readonly Dictionary<int, PowerUp> ActivePowerUps = new Dictionary<int, PowerUp>();
        public static double Score { get; set; }

        private static readonly Random s_random = new Random();
        private static bool s_hasPowerUpAlreadyTriggered;
        private static int s_cachedStreak;

        public static void Update(GameTime gameTime)
        {
            if (Streak != 0 && Streak%4 == 0 && !s_hasPowerUpAlreadyTriggered)
            {
                EngageRandomPowerUp();
                s_hasPowerUpAlreadyTriggered = true;
                s_cachedStreak = Streak;
            }

            if (Streak < s_cachedStreak || Streak > s_cachedStreak)
            {
                s_hasPowerUpAlreadyTriggered = false;
            }

            if (Streak < 4)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(new Color(255, 120, 0));
            }
            else if (Streak >= 4 && Streak < 8)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.Plum);
            }
            else if (Streak >= 8 && Streak < 12)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.Lime);
            }
            else if (Streak >= 12 && Streak < 16)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.DarkRed);
            }
            else if (Streak >= 16)
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(Color.BlueViolet);
            }
            else
            {
                ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.ChangeExplosionColor(new Color(255, 120, 0));
            }
        }

        public static void ResetArcadeGoals()
        {
            Streak = 0;
            foreach (KeyValuePair<int, PowerUp> powerUp in ActivePowerUps)
            {
                if (powerUp.Value.PowerUpName == "Homing Ball")
                {
                    powerUp.Value.AvailableInventory = 3;
                }
                else
                {
                    powerUp.Value.TimeRemaining = 0;
                    powerUp.Value.IsActive = false;
                }
            }
        }

        private static void EngageRandomPowerUp()
        {
            int powerUpToEngage = s_random.Next(1, ActivePowerUps.Count + 1);
            PowerUp type;
            if (ActivePowerUps.TryGetValue(powerUpToEngage, out type))
            {
                if (type.IsActive)
                {
                    if (type.PowerUpName == "Homing Ball")
                    {
                        type.AvailableInventory += 1;
                    }
                    else if (type.PowerUpName != "Rapid Fire")
                    {
                        type.TimeRemaining = 10000;
                    }
                }
                else
                {
                    if (type.PowerUpName == "Rapid Fire")
                    {
                        type.IsActive = true;
                        type.TimeRemaining = 6000;
                    }
                    else
                    {
                        type.IsActive = true;
                        type.TimeRemaining = 10000;
                    }
                }
            }
        }

        public static void LoadPowerUps()
        {
            ActivePowerUps.Add(1, new PowerUp("Laser Sight"));
            ActivePowerUps.Add(2, new PowerUp("Double Score"));
            ActivePowerUps.Add(3, new PowerUp("Homing Ball"));
            ActivePowerUps.Add(4, new PowerUp("Rapid Fire"));
        }
    }
}
