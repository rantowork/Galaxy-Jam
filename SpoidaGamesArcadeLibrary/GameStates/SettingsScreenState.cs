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
        private static KeyboardState s_cachedUpDownKeyboardState;
        private static KeyboardState s_cachedRightLeftKeyboardState;

        private const double SAVE_TIME = 2000;
        private static double s_displaySaveMessageTimer;
        
        public static void Update(GameTime gameTime)
        {
            if (ComputerSettings.CurrentSettingSelection == 0)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (ComputerSettings.CurrentResolution > 0)
                    {
                        ComputerSettings.CurrentResolution--;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (ComputerSettings.CurrentResolution < Screen.DisplayModes.Count - 1)
                    {
                        ComputerSettings.CurrentResolution++;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (ComputerSettings.CurrentSettingSelection == 1)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (ComputerSettings.FullScreenSetting > 0)
                    {
                        ComputerSettings.FullScreenSetting--;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (ComputerSettings.FullScreenSetting < 2)
                    {
                        ComputerSettings.FullScreenSetting++;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (ComputerSettings.CurrentSettingSelection == 2)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (ComputerSettings.MusicVolumeSetting > 0)
                    {
                        ComputerSettings.MusicVolumeSetting--;
                        MediaPlayer.Volume = (float)ComputerSettings.MusicVolumeSetting / 10;
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (ComputerSettings.MusicVolumeSetting < 10)
                    {
                        ComputerSettings.MusicVolumeSetting++;
                        MediaPlayer.Volume = (float)ComputerSettings.MusicVolumeSetting / 10;
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }
            else if (ComputerSettings.CurrentSettingSelection == 3)
            {
                if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                {
                    if (ComputerSettings.SoundEffectVolumeSetting > 0)
                    {
                        ComputerSettings.SoundEffectVolumeSetting--;
                        Sounds.CollisionSoundEffect.Play((float)ComputerSettings.SoundEffectVolumeSetting / 10, 0f, 0f);
                    }
                }
                else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !s_cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                {
                    if (ComputerSettings.SoundEffectVolumeSetting < 10)
                    {
                        ComputerSettings.SoundEffectVolumeSetting++;
                        Sounds.CollisionSoundEffect.Play((float)ComputerSettings.SoundEffectVolumeSetting / 10, 0f, 0f);
                    }
                }
                s_cachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
            }

            if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
            {
                if (ComputerSettings.CurrentSettingSelection < 6)
                {
                    ComputerSettings.CurrentSettingSelection++;
                }
                if (ComputerSettings.CurrentSettingSelection == 6)
                {
                    ComputerSettings.CurrentSettingSelection = 0;
                }
            }
            else if (Screen.Input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !s_cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
            {
                if (ComputerSettings.CurrentSettingSelection == 0)
                {
                    ComputerSettings.CurrentSettingSelection = 6;
                }
                if (ComputerSettings.CurrentSettingSelection > 0)
                {
                    ComputerSettings.CurrentSettingSelection--;
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
            DisplayMode selectedMode = Screen.DisplayModes[ComputerSettings.CurrentResolution];
            string resolutionText = String.Format("{0} x {1}", selectedMode.Width, selectedMode.Height);
            string fullScreenText;
            switch (ComputerSettings.FullScreenSetting)
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
            spriteBatch.DrawString(Fonts.SpriteFont, ComputerSettings.MusicVolumeSetting.ToString(CultureInfo.InvariantCulture), new Vector2(740, 350), Color.White);
            spriteBatch.DrawString(Fonts.SpriteFont, ComputerSettings.SoundEffectVolumeSetting.ToString(CultureInfo.InvariantCulture), new Vector2(740, 375), Color.White);

            Vector2 selectionLocation;
            Vector2 caretOrigin = Vector2.Zero;
            switch (ComputerSettings.CurrentSettingSelection)
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
