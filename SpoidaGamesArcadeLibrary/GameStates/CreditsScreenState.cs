using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class CreditsScreenState
    {
        private static double s_creditsTimer;
        private const string THANK_YOU = "Thanks for playing!";
        private static Vector2 s_creditsLocation = new Vector2(1280 / 2, 650);
        
        public static void Update(GameTime gameTime)
        {
            s_creditsTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (s_creditsTimer <= 70000)
            {
                float amount = MathHelper.Clamp((float)s_creditsTimer / 70000, 0, 1);
                float lerp = MathHelper.Lerp(650, -2000, amount);
                s_creditsLocation.Y = lerp;
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 thankYouOrigin = Fonts.PixelScoreGlow.MeasureString(THANK_YOU)/2;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.Draw(Textures.Credits, s_creditsLocation, null, Color.White, 0f, new Vector2(400, 0), 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(Textures.TextFade, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(Textures.TextFade, new Vector2(0, 320), null, Color.White, (float)Math.PI, new Vector2(Textures.TextFade.Width, Textures.TextFade.Height), 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.PixelScoreGlow, THANK_YOU, new Vector2(1280 / 2, 30), Color.White, 0f, thankYouOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();
        }
    }
}
