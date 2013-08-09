using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class TitleScreenState
    {
        private static KeyboardState s_cachedUpDownKeyboardState;
        
        public static void Update(GameTime gameTime)
        {
            if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
            {
                if (InterfaceSettings.TitleScreenSelection >= 0 && InterfaceSettings.TitleScreenSelection < 6)
                {
                    InterfaceSettings.TitleScreenSelection++;
                }
                else if (InterfaceSettings.TitleScreenSelection == 6)
                {
                    InterfaceSettings.TitleScreenSelection = 0;
                }
            }
            else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
            {
                if (InterfaceSettings.TitleScreenSelection > 0 && InterfaceSettings.TitleScreenSelection <= 6)
                {
                    InterfaceSettings.TitleScreenSelection--;
                }
                else if (InterfaceSettings.TitleScreenSelection == 0)
                {
                    InterfaceSettings.TitleScreenSelection = 6;
                }
            }
            s_cachedUpDownKeyboardState = Screen.Input.GetKeyboard().GetState();
        }

        private const string PLAY_TEXT = "Classic Mode";
        private const string PLAY_TEXT2 = "Arcade Mode";
        private const string PRACTICE_TEXT = "Practice";
        private const string SETTINGS_TEXT = "Settings";
        private const string TUTORIAL_TEXT = "How to Play";
        private const string CREDITS_TEXT = "Credits";
        private const string EXIT_TEXT = "Exit";
        private const string COPYRIGHT = "(c) Spoida Games LLC, 2013";
        private const string VERSION = "v0.20 RC 2 (Beta)";

        private static readonly Vector2 s_playTextOrigin = Fonts.SpriteFont.MeasureString(PLAY_TEXT) / 2;
        private static readonly Vector2 s_practiceOrigin = Fonts.SpriteFont.MeasureString(PRACTICE_TEXT) / 2;
        private static readonly Vector2 s_settingsTextOrigin = Fonts.SpriteFont.MeasureString(SETTINGS_TEXT) / 2;
        private static readonly Vector2 s_tutorialTextOrigin = Fonts.SpriteFont.MeasureString(TUTORIAL_TEXT) / 2;
        private static readonly Vector2 s_exitOrigin = Fonts.SpriteFont.MeasureString(EXIT_TEXT) / 2;
        private static readonly Vector2 s_copyrightOrigin = Fonts.SpriteFontGlow.MeasureString(COPYRIGHT) / 2;
        private static readonly Vector2 s_playTextGlowOrigin = Fonts.SpriteFontGlow.MeasureString(PLAY_TEXT) / 2;
        private static readonly Vector2 s_practiceGlowOrigin = Fonts.SpriteFontGlow.MeasureString(PRACTICE_TEXT) / 2;
        private static readonly Vector2 s_settingsTextGlowOrigin = Fonts.SpriteFontGlow.MeasureString(SETTINGS_TEXT) / 2;
        private static readonly Vector2 s_tutorialTextGlowOrigin = Fonts.SpriteFontGlow.MeasureString(TUTORIAL_TEXT) / 2;
        private static readonly Vector2 s_exitGlowOrigin = Fonts.SpriteFontGlow.MeasureString(EXIT_TEXT) / 2;
        private static readonly Vector2 s_playTextOrigin2 = Fonts.SpriteFont.MeasureString(PLAY_TEXT2) / 2;
        private static readonly Vector2 s_playTextGlowOrigin2 = Fonts.SpriteFontGlow.MeasureString(PLAY_TEXT2) / 2;
        private static readonly Vector2 s_creditsTextOrigin = Fonts.SpriteFont.MeasureString(CREDITS_TEXT)/2;
        private static readonly Vector2 s_creditsTextGlowOrigin = Fonts.SpriteFontGlow.MeasureString(CREDITS_TEXT)/2;

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            Vector2 galaxyLogoOrigin = new Vector2(Textures.GalaxyJamText.Width, Textures.GalaxyJamText.Height) / 2;
            spriteBatch.Draw(Textures.GalaxyJamText, new Vector2(1280 / 2, 85), null, Color.White, 0f, galaxyLogoOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.SpriteFontGlow, COPYRIGHT, new Vector2(1280 / 2, 696), Color.DarkOrange, 0f, s_copyrightOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.SpriteFont, VERSION, new Vector2(10, 690), Color.White);
            if (InterfaceSettings.TitleScreenSelection == 0)
            {
                spriteBatch.DrawString(Fonts.SpriteFontGlow, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.TitleScreenSelection == 1)
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextGlowOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.TitleScreenSelection == 2)
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.TitleScreenSelection == 3)
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.TitleScreenSelection == 4)
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (InterfaceSettings.TitleScreenSelection == 5)
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT, new Vector2(1280 / 2, 290), Color.White, 0f, s_playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PLAY_TEXT2, new Vector2(1280 / 2, 320), Color.White, 0f, s_playTextOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, PRACTICE_TEXT, new Vector2(1280 / 2, 350), Color.White, 0f, s_practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, SETTINGS_TEXT, new Vector2(1280 / 2, 380), Color.White, 0f, s_settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, TUTORIAL_TEXT, new Vector2(1280 / 2, 410), Color.White, 0f, s_tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFont, CREDITS_TEXT, new Vector2(1280 / 2, 440), Color.White, 0f, s_creditsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.DrawString(Fonts.SpriteFontGlow, EXIT_TEXT, new Vector2(1280 / 2, 470), Color.White, 0f, s_exitGlowOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }
            spriteBatch.End();
        }
    }
}
