using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class PausedScreenState
    {
        public static void Update(GameTime gameTime)
        {

        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            GameInterface.DrawPausedInterface(spriteBatch, Fonts.SpriteFont, Fonts.PixelScoreGlow);
            spriteBatch.End();
        }
    }
}
