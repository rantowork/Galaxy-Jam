using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpoidaGamesArcadeLibrary.GameStates;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class ArcadeGoalManager
    {
        public static int Streak { get; set; }
        public static readonly Dictionary<int, PowerUp> ActivePowerUps = new Dictionary<int, PowerUp>();
        public static double Score { get; set; }
        public static int Multiplier { get; set; }
        public static bool DrawNumberScrollEffect { get; set; }
        public static string NumberScrollScoreToDraw { get; set; }

        private static readonly Random s_random = new Random();
        private static bool s_hasPowerUpAlreadyTriggered;
        private static int s_cachedStreak;

        public static void Update(GameTime gameTime)
        {
            if (Streak != 0 && Streak%4 == 0 && !s_hasPowerUpAlreadyTriggered)
            {
                EngageRandomPowerUp();
                Multiplier++;
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
            Multiplier = 1;
            Score = 0;
            DrawNumberScrollEffect = false;
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
                        ArcadeModeScreenState.HomingBallTextTimer = 0;
                        ArcadeModeScreenState.DrawHomingBallText = true;
                    }
                    else if (type.PowerUpName == "Rapid Fire")
                    {
                        ArcadeModeScreenState.RapidFireTextTimer = 0;
                        ArcadeModeScreenState.DrawRapidFireText = true;
                    }
                    else if (type.PowerUpName == "2x Multiplier")
                    {
                        type.TimeRemaining = 10000;
                        ArcadeModeScreenState.MulitplierTextTimer = 0;
                        ArcadeModeScreenState.DrawMultiplierText = true;
                    }
                    else if (type.PowerUpName == "Laser Sight")
                    {
                        type.TimeRemaining = 10000;
                        ArcadeModeScreenState.LaserSightTextTimer = 0;
                        ArcadeModeScreenState.DrawLaserSightText = true;
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
                        ArcadeModeScreenState.RapidFireTextTimer = 0;
                        ArcadeModeScreenState.DrawRapidFireText = true;
                        type.TimeRemaining = 4000;
                    }
                    else if (type.PowerUpName == "Laser Sight")
                    {
                        type.IsActive = true;
                        ArcadeModeScreenState.LaserSightTextTimer = 0;
                        ArcadeModeScreenState.DrawLaserSightText = true;
                        type.TimeRemaining = 10000;
                    }
                    else if (type.PowerUpName == "2x Multiplier")
                    {
                        ArcadeModeScreenState.MulitplierTextTimer = 0;
                        ArcadeModeScreenState.DrawMultiplierText = true;
                        type.TimeRemaining = 10000;
                        type.IsActive = true;
                    }
                    else if (type.PowerUpName == "Homing Ball")
                    {
                        ArcadeModeScreenState.HomingBallTextTimer = 0;
                        ArcadeModeScreenState.DrawHomingBallText = true;
                        type.TimeRemaining = 10000;
                        type.IsActive = true;
                    }
                }
            }
        }

        public static void LoadPowerUps()
        {
            ActivePowerUps.Add(1, new PowerUp("Laser Sight"));
            ActivePowerUps.Add(2, new PowerUp("2x Multiplier"));
            ActivePowerUps.Add(3, new PowerUp("Homing Ball"));
            ActivePowerUps.Add(4, new PowerUp("Rapid Fire"));
            Multiplier = 1;
        }
    }
}
