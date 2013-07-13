using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class Screen
    {
        public static Camera Camera { get; set; }
        public static InputManager Input { get; set; }
        public static KeyboardState CachedRightLeftKeyboardState { get; set; }
        public static readonly List<DisplayMode> DisplayModes = new List<DisplayMode>();
        private static readonly Random s_rand = new Random();

        public static void HandlePlayerInput()
        {
            MouseState state = Input.GetMouse().GetState();

            InterfaceSettings.BasketballLocation = InterfaceSettings.BasketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 mouseLocation =
                Vector2.Transform(
                    new Vector2(state.X, state.Y) -
                    new Vector2(ResolutionManager.GetViewportX, ResolutionManager.GetViewportY),
                    Matrix.Invert(ResolutionManager.GetTransformationMatrix()));

            double radians = MouseAngle(InterfaceSettings.BasketballLocation, mouseLocation);
            InterfaceSettings.PointingAt = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
            float distance = Vector2.Distance(InterfaceSettings.BasketballLocation, mouseLocation);
            float modifier = MathHelper.Clamp(distance, 0, 1200);
            InterfaceSettings.Force = (6 / 10f) * modifier + 1200;

            if (InterfaceSettings.BasketballManager.BasketballBody.Awake == false)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    PhysicalWorld.World.Gravity.Y = 25;
                    InterfaceSettings.BasketballManager.BasketballBody.Awake = true;
                    HandleShotAngle(InterfaceSettings.Force);
                    SoundManager.PlaySoundEffect(Sounds.BasketBallShotSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }
            }
        }

        private static void HandleShotAngle(float shotForce)
        {
            InterfaceSettings.BasketballManager.BasketballBody.ApplyLinearImpulse(ConvertUnits.ToSimUnits(InterfaceSettings.PointingAt) * shotForce);
            InterfaceSettings.BasketballManager.BasketballBody.ApplyAngularImpulse(.2f);
        }

        private static double MouseAngle(Vector2 spriteLocation, Vector2 mouseLocation)
        {
            return Math.Atan2(mouseLocation.Y - (spriteLocation.Y), mouseLocation.X - (spriteLocation.X)); //this will return the angle(in radians) from sprite to mouse.
        }

        public static void HandleBasketballPosition()
        {
            if (InterfaceSettings.BasketballManager.BasketballBody.Position.Y > 720 / PhysicalWorld.MetersInPixels)
            {
                PhysicalWorld.World.Gravity.Y = 0;
                InterfaceSettings.BasketballManager.BasketballBody.Awake = false;
                InterfaceSettings.BasketballManager.BasketballBody.Position = RandomizePosition();
                InterfaceSettings.GoalManager.GoalScored = false;
                InterfaceSettings.GoalManager.BackboardHit = false;
                InterfaceSettings.GoalManager.RimHit = false;
                if (InterfaceSettings.GoalManager.ScoredOnShot)
                {
                    InterfaceSettings.GoalManager.ScoredOnShot = false;
                }
                else if (!InterfaceSettings.GoalManager.ScoredOnShot)
                {
                    InterfaceSettings.GoalManager.Streak = 0;
                }
            }
        }

        public static Vector2 RandomizePosition()
        {
            return new Vector2((s_rand.Next(400, 1200)) / PhysicalWorld.MetersInPixels, (s_rand.Next(310, 650)) / PhysicalWorld.MetersInPixels);
        }
    }
}
