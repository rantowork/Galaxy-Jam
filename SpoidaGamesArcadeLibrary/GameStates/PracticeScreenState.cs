using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class PracticeScreenState
    {
        public static void Update(GameTime gameTime)
        {
            BasketballManager.Basketballs[0].Update(gameTime);

            float timeStep = Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 60f));
            PhysicalWorld.World.Step(timeStep);

            Screen.HandlePlayerInput();
            Screen.HandleBasketballPosition();

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
            const string escapePractice = "(Esc) Exit";
            const string practiceModeText = "Practice Mode";

            Vector2 practiceModeOrigin = Fonts.SpriteFont.MeasureString(practiceModeText) / 2;

            Vector2 backboardPosition = PhysicalWorld.BackboardBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 backboardOrigin = new Vector2(Textures.Backboard1.Width / 2f, Textures.Backboard1.Height / 2f);

            Vector2 leftRimOrigin = new Vector2(Textures.LeftRim1.Width, Textures.LeftRim1.Height) / 2;
            Vector2 rightRimOrigin2 = new Vector2(Textures.RightRim1.Width, Textures.RightRim1.Height) / 2;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.DrawString(Fonts.SpriteFont, escapePractice, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, practiceModeText, new Vector2(1280 / 2, 18), Color.White, 0f, practiceModeOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            if (InterfaceSettings.BasketballManager.BasketballBody.Awake == false)
            {
                for (float t = 0; t < 70f; t += .01f)
                {
                    const float steps = 1 / 60f;
                    Vector2 stepVelocity = (InterfaceSettings.PointingAt * InterfaceSettings.Force * steps);
                    Vector2 gravity = (ConvertUnits.ToDisplayUnits(new Vector2(0, 25f))) * steps * steps;
                    Vector2 position = InterfaceSettings.BasketballLocation + t * stepVelocity + .5f * (t * t + t) * gravity;
                    spriteBatch.Draw(Textures.Twopxsolidstar, position, Color.MediumPurple);
                }
            }

            BasketballManager.SelectedBasketball.DrawEmitter(spriteBatch);
            spriteBatch.Draw(BasketballManager.Basketballs[0].BasketballTexture, (InterfaceSettings.BasketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.Basketballs[0].Source, Color.White, InterfaceSettings.BasketballManager.BasketballBody.Rotation, BasketballManager.Basketballs[0].Origin, 1f, SpriteEffects.None, 0f);

            //draw backboard
            spriteBatch.Draw(PhysicalWorld.BackboardCollisionHappened ? Textures.Backboard1Glow : Textures.Backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
            //draw left rim
            spriteBatch.Draw(PhysicalWorld.LeftRimCollisionHappened ? Textures.LeftRim1Glow : Textures.LeftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
            //draw right rim
            spriteBatch.Draw(PhysicalWorld.RightRimCollisionHappened ? Textures.RightRim1Glow : Textures.RightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

        }
    }
}
