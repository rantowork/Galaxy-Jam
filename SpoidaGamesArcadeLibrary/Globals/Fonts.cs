using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class Fonts
    {
        public static SpriteFont SpriteFont { get; set; }
        public static SpriteFont SpriteFontGlow { get; set; }
        public static SpriteFont StreakText { get; set; }
        public static SpriteFont PixelScoreGlow { get; set; }
        public static SpriteFont GiantRedPixelFont { get; set; }

        public static void LoadFonts(ContentManager content)
        {
            SpriteFont = content.Load<SpriteFont>(@"Fonts/SpriteFont");
            SpriteFontGlow = content.Load<SpriteFont>(@"Fonts/SpriteFontGlow");
            PixelScoreGlow = content.Load<SpriteFont>(@"Fonts/PixelScoreGlow");
            GiantRedPixelFont = content.Load<SpriteFont>(@"Fonts/GiantRedPixelFont");
            StreakText = content.Load<SpriteFont>(@"Fonts/StreakText");
        }
    }
}
