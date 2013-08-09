using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class StartScreenState
    {
        private const double FADE_IN_DELAY = 1500;
        private static double s_fadeTimer;
        private static float s_amount;
        private static float s_fade;

        public static void Update(GameTime gameTime)
        {
            s_fadeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (s_fadeTimer <= FADE_IN_DELAY)
            {
                s_amount = MathHelper.Clamp((float)s_fadeTimer/1500, 0, 1);
                s_fade = MathHelper.Lerp(0, 1, s_amount);
            }
        }
        
        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.Draw(Textures.GalaxyJamLogo, new Rectangle(0, 0, 1280, 720), new Color(255, 255, 255, s_fade));
            spriteBatch.End();
        }
    }
}
