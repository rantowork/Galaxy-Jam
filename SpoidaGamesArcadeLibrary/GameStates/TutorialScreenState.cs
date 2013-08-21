using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class TutorialScreenState
    {
        public static void Update(GameTime gameTime)
        {
            if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Enter) && !Screen.CachedRightLeftKeyboardState.IsKeyDown(Keys.Enter))
            {
                if (InterfaceSettings.CurrentTutorialScreen < 4)
                {
                    InterfaceSettings.CurrentTutorialScreen++;
                }
            }
            Screen.CachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            if (InterfaceSettings.CurrentTutorialScreen == 0)
            {
                BasketballManager.Basketballs[0].Update(gameTime);
                
                Screen.HandlePlayerInput();
                Screen.HandleBasketballPosition();
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            const string escapeTutorial = "(Esc) Exit";
            string enterContinue = "(Enter) Next";
            if (InterfaceSettings.CurrentTutorialScreen == 4)
            {
                enterContinue = "";
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.DrawString(Fonts.SpriteFont, escapeTutorial, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, enterContinue, new Vector2(1080, 10), Color.White);
            spriteBatch.End();

            if (InterfaceSettings.CurrentTutorialScreen == 0)
            {
                const string tutText01 = "Click to shoot. Try it out!";
                Vector2 tutText1Origin = Fonts.SpriteFont.MeasureString(tutText01) / 2;
                BasketballManager.Basketballs[0].DrawEmitter(gameTime, spriteBatch);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.DrawString(Fonts.SpriteFont, tutText01, new Vector2(1280 / 2, 700), Color.White, 0f, tutText1Origin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.Draw(BasketballManager.Basketballs[0].BasketballTexture, (InterfaceSettings.BasketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.Basketballs[0].Source, Color.White, InterfaceSettings.BasketballManager.BasketballBody.Rotation, BasketballManager.Basketballs[0].Origin, 1f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }
            else if (InterfaceSettings.CurrentTutorialScreen == 1)
            {
                const string tutText02 = "Game timer.  You only get 2 minutes!";
                const string tutText02Timer = "2:00";
                Vector2 tutText02Origin = Fonts.SpriteFont.MeasureString(tutText02) / 2;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.DrawString(Fonts.SpriteFont, tutText02, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText02Origin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.PixelScoreGlow, tutText02Timer, new Vector2(10, 664), Color.White);
                spriteBatch.End();
            }
            else if (InterfaceSettings.CurrentTutorialScreen == 2)
            {
                const string tutText03 = "Your score.  Earn new basketballs with a high score.";
                const string tutText03Score = "100000";
                Vector2 tutText03Origin = Fonts.SpriteFont.MeasureString(tutText03) / 2;
                Vector2 tutText03ScoreOrigin = Fonts.PixelScoreGlow.MeasureString(tutText03Score) / 2;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.DrawString(Fonts.SpriteFont, tutText03, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText03Origin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.PixelScoreGlow, tutText03Score, new Vector2(1280 / 2, 30), Color.White, 0f, tutText03ScoreOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.End();
            }
            else if (InterfaceSettings.CurrentTutorialScreen == 3)
            {
                const string tutText04 = "Current streak.  A higher streak means more points.";
                const string tutText04Streak = "+9";
                Vector2 tutText04Origin = Fonts.SpriteFont.MeasureString(tutText04) / 2;
                Vector2 tutText04StreakOrigin = Fonts.PixelScoreGlow.MeasureString(tutText04Streak);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.DrawString(Fonts.SpriteFont, tutText04, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText04Origin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.PixelScoreGlow, tutText04Streak, new Vector2(1260, 100), Color.White, 0f, tutText04StreakOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.End();
            }
            else if (InterfaceSettings.CurrentTutorialScreen == 4)
            {
                const string tutText05 = "Score multiplier.  Big multiplier, big points.";
                const string tutText05Mult = "x42";
                Vector2 tutText05Origin = Fonts.SpriteFont.MeasureString(tutText05) / 2;
                Vector2 tutText05MultOrigin = Fonts.PixelScoreGlow.MeasureString(tutText05Mult);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                spriteBatch.DrawString(Fonts.SpriteFont, tutText05, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText05Origin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.PixelScoreGlow, tutText05Mult, new Vector2(1260, 720), Color.White, 0f, tutText05MultOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.End();
            }
        }
    }
}
