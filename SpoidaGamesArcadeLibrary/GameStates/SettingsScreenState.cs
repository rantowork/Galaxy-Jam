using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class SettingsScreenState
    {
        private static int s_currentSettingSelection;
        private static int s_currentResolution;
        private static int s_fullScreenSetting;
        private static int s_musicVolumeSetting;
        private static int s_soundEffectVolumeSetting;

        private static KeyboardState s_cachedUpDownKeyboardState;
        private static KeyboardState s_cachedRightLeftKeyboardState;

        private const double SAVE_TIME = 2000;
        private static double s_displaySaveMessageTimer;
        
        public static void Update(GameTime gameTime)
        {
            if (s_currentSettingSelection == 0)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (s_currentResolution > 0)
                    {
                        s_currentResolution--;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (s_currentResolution < Screen.DisplayModes.Count - 1)
                    {
                        s_currentResolution++;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (s_currentSettingSelection == 1)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (s_fullScreenSetting > 0)
                    {
                        s_fullScreenSetting--;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (s_fullScreenSetting < 2)
                    {
                        s_fullScreenSetting++;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (s_currentSettingSelection == 2)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (s_musicVolumeSetting > 0)
                    {
                        s_musicVolumeSetting--;
                        MediaPlayer.Volume = (float)s_musicVolumeSetting / 10;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (s_musicVolumeSetting < 10)
                    {
                        s_musicVolumeSetting++;
                        MediaPlayer.Volume = (float)s_musicVolumeSetting / 10;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (s_currentSettingSelection == 3)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (s_soundEffectVolumeSetting > 0)
                    {
                        s_soundEffectVolumeSetting--;
                        Sounds.CollisionSoundEffect.Play((float)s_soundEffectVolumeSetting / 10, 0f, 0f);
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (s_soundEffectVolumeSetting < 10)
                    {
                        s_soundEffectVolumeSetting++;
                        Sounds.CollisionSoundEffect.Play((float)s_soundEffectVolumeSetting / 10, 0f, 0f);
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }

            if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
            {
                if (s_currentSettingSelection < 6)
                {
                    s_currentSettingSelection++;
                }
                if (s_currentSettingSelection == 6)
                {
                    s_currentSettingSelection = 0;
                }
            }
            else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
            {
                if (s_currentSettingSelection == 0)
                {
                    s_currentSettingSelection = 6;
                }
                if (s_currentSettingSelection > 0)
                {
                    s_currentSettingSelection--;
                }
            }
            s_cachedUpDownKeyboardState = Screen.Input.GetKeyboard().GetState();

            if (InterfaceSettings.DisplaySettingsSavedMessage)
            {
                s_displaySaveMessageTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (SAVE_TIME <= s_displaySaveMessageTimer)
                {
                    s_displaySaveMessageTimer = 0;
                    InterfaceSettings.DisplaySettingsSavedMessage = false;
                }
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DisplayMode selectedMode = Screen.DisplayModes[s_currentResolution];
            string resolutionText = String.Format("{0} x {1}", selectedMode.Width, selectedMode.Height);
            string fullScreenText;
            switch (s_fullScreenSetting)
            {
                case 0:
                    fullScreenText = "Windowed";
                    break;
                case 1:
                    fullScreenText = "Full Screen";
                    break;
                case 2:
                    fullScreenText = "Full Screen Borderless";
                    break;
                default:
                    fullScreenText = "Windowed";
                    break;
            }
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.DrawString(Fonts.SpriteFont, resolutionText, new Vector2(740, 300), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, fullScreenText, new Vector2(740, 325), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, s_musicVolumeSetting.ToString(CultureInfo.InvariantCulture), new Vector2(740, 350), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, s_soundEffectVolumeSetting.ToString(CultureInfo.InvariantCulture), new Vector2(740, 375), Color.White);

            Vector2 selectionLocation;
            Vector2 caretOrigin = Vector2.Zero;
            switch (s_currentSettingSelection)
            {
                case 0:
                    selectionLocation = new Vector2(300, 300);
                    break;
                case 1:
                    selectionLocation = new Vector2(300, 325);
                    break;
                case 2:
                    selectionLocation = new Vector2(300, 350);
                    break;
                case 3:
                    selectionLocation = new Vector2(300, 375);
                    break;
                case 4:
                    selectionLocation = new Vector2(1280 / 2 - 50, 450);
                    caretOrigin = Fonts.SpriteFont.MeasureString(">") / 2;
                    break;
                case 5:
                    selectionLocation = new Vector2(1280 / 2 - 50, 475);
                    caretOrigin = Fonts.SpriteFont.MeasureString(">") / 2;
                    break;
                default:
                    selectionLocation = new Vector2(300, 300);
                    break;
            }

            spriteBatch.DrawString(Fonts.SpriteFont, ">", selectionLocation, Color.White, 0f, caretOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.SpriteFont, "  Resolution: ", new Vector2(300, 300), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, "  Full Screen: ", new Vector2(300, 325), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, "  Music Volume: ", new Vector2(300, 350), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, "  Sound Effect Volume: ", new Vector2(300, 375), Color.White);
            Vector2 saveOrigin = Fonts.SpriteFont.MeasureString("Save") / 2;
            spriteBatch.DrawString(Fonts.SpriteFont, "Save", new Vector2(1280 / 2, 450), Color.White, 0f, saveOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.DrawString(Fonts.SpriteFont, "Back", new Vector2(1280 / 2, 475), Color.White, 0f, saveOrigin, 1.0f, SpriteEffects.None, 1.0f);

            if (InterfaceSettings.DisplaySettingsSavedMessage)
            {
                const string savedText = "Settings Saved";
                Vector2 savedTextOrigin = Fonts.SpriteFont.MeasureString(savedText) / 2;
                spriteBatch.DrawString(Fonts.SpriteFont, savedText, new Vector2(1280 / 2, 680), Color.Red, 0f, savedTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }

            spriteBatch.End();
        }
    }
}
