using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class OptionsScreenState
    {
        private static readonly Rectangle s_downArrowBox = new Rectangle(564, 322, 32, 32);
        private static readonly Rectangle s_upArrowBox = new Rectangle(924, 322, 32, 32);
        private static readonly Rectangle s_leftArrowBox = new Rectangle(564, 522, 32, 32);
        private static readonly Rectangle s_rightArrowBox = new Rectangle(924, 522, 32, 32);

        public static void Update(GameTime gameTime)
        {
            if (InterfaceSettings.DownArrowHovered)
            {
                if (Screen.Input.GetMouse().GetState().LeftButton == ButtonState.Pressed && InterfaceSettings.PreviousMouseClick.LeftButton != ButtonState.Pressed && InterfaceSettings.HighScoreManager.CanChangeBasketballSelection)
                {
                    if (InterfaceSettings.CurrentlySelectedBasketballKey < BasketballManager.Basketballs.Count - 1)
                    {
                        InterfaceSettings.CurrentlySelectedBasketballKey++;
                    }
                }
            }
            if (InterfaceSettings.UpArrowHovered)
            {
                if (Screen.Input.GetMouse().GetState().LeftButton == ButtonState.Pressed && InterfaceSettings.PreviousMouseClick.LeftButton != ButtonState.Pressed && InterfaceSettings.HighScoreManager.CanChangeBasketballSelection)
                {
                    if (InterfaceSettings.CurrentlySelectedBasketballKey > 0)
                    {
                        InterfaceSettings.CurrentlySelectedBasketballKey--;
                    }
                }
            }
            if (InterfaceSettings.LeftArrowHovered)
            {
                if (Screen.Input.GetMouse().GetState().LeftButton == ButtonState.Pressed && InterfaceSettings.PreviousMouseClick.LeftButton != ButtonState.Pressed)
                {
                    if (InterfaceSettings.CurrentlySelectedSongKey > 0)
                    {
                        InterfaceSettings.CurrentlySelectedSongKey--;
                    }
                }
            }
            if (InterfaceSettings.RightArrowHovered)
            {
                if (Screen.Input.GetMouse().GetState().LeftButton == ButtonState.Pressed && InterfaceSettings.PreviousMouseClick.LeftButton != ButtonState.Pressed)
                {
                    if (InterfaceSettings.CurrentlySelectedSongKey < SoundManager.music.Count - 1)
                    {
                        InterfaceSettings.CurrentlySelectedSongKey++;
                    }
                }
            }
            InterfaceSettings.PreviousMouseClick = Screen.Input.GetMouse().GetState();

            if (InterfaceSettings.PlayerName.Length >= 3)
            {
                InterfaceSettings.NameToShort = false;
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameInterface.DrawOptionsInterface(spriteBatch, gameTime, Fonts.SpriteFont, Fonts.SpriteFontGlow, InterfaceSettings.HighScoreManager, InterfaceSettings.NameToShort, InterfaceSettings.CurrentlySelectedBasketballKey, InterfaceSettings.CurrentlySelectedSongKey, Screen.Camera, InterfaceSettings.ArcadeHighScoreManager);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            GetPlayerName(gameTime, spriteBatch);
            spriteBatch.Draw(Textures.MenuHull, new Vector2(38, 0), Color.White);
            Vector2 galaxyJamLogoOrigin = new Vector2(Textures.GalaxyJamText.Width, Textures.GalaxyJamText.Height) / 2;
            spriteBatch.Draw(Textures.GalaxyJamText, new Vector2(1280 / 2 + 120, 85), null, Color.White, 0f, galaxyJamLogoOrigin, 1.0f, SpriteEffects.None, 1.0f);

            const string basketballselection = "Basketball";
            Vector2 bsOrigin = Fonts.SpriteFontGlow.MeasureString(basketballselection) / 2;
            spriteBatch.DrawString(Fonts.SpriteFontGlow, basketballselection, new Vector2(141.5f, 47.5f), Color.White, 0f, bsOrigin, 1.0f, SpriteEffects.None, 1.0f);

            const string goBackText = "(Esc) Return to Menu";
            Vector2 goBackOrigin = Fonts.SpriteFontGlow.MeasureString(goBackText) / 2;
            spriteBatch.DrawString(Fonts.SpriteFontGlow, goBackText, new Vector2(1280 / 2 + 120, 700), Color.White, 0f, goBackOrigin, 1.0f, SpriteEffects.None, 1.0f);

            const string selectBasketballText = "Selected Basketball";
            const string selectMusicText = "Selected Music";
            Vector2 selectedBasketballTextOrigin = Fonts.SpriteFont.MeasureString(selectBasketballText) / 2;
            Vector2 selectedMusicTextOrigin = Fonts.SpriteFont.MeasureString(selectMusicText) / 2;
            spriteBatch.DrawString(Fonts.SpriteFont, selectBasketballText, new Vector2(1280 / 2 + 120, 338), Color.White, 0f, selectedBasketballTextOrigin, 1.0f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(Fonts.SpriteFont, selectMusicText, new Vector2(1280 / 2 + 120, 538), Color.White, 0f, selectedMusicTextOrigin, 1.0f, SpriteEffects.None, 1f);

            Vector2 arrowKeyOrigin = new Vector2(Textures.ArrowKey.Width, Textures.ArrowKey.Height) / 2;
            MouseState state = Mouse.GetState();
            Vector2 mouseLocation = Vector2.Transform(new Vector2(state.X, state.Y) - new Vector2(ResolutionManager.GetViewportX, ResolutionManager.GetViewportY), Matrix.Invert(ResolutionManager.GetTransformationMatrix()));
            Rectangle mouseRectangle = new Rectangle(Convert.ToInt32(mouseLocation.X), Convert.ToInt32(mouseLocation.Y), 1, 1);

            if (InterfaceSettings.CurrentlySelectedBasketballKey == 0)
            {
                //down
                if (mouseRectangle.Intersects(s_downArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 - 60, 338), null, Color.White, 0f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.DownArrowHovered = true;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.RightArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 - 60, 338), null, Color.White, 0f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.DownArrowHovered = false;
                }
            }
            else if (InterfaceSettings.CurrentlySelectedBasketballKey > 0 && InterfaceSettings.CurrentlySelectedBasketballKey < (BasketballManager.BasketballSelection.Count - 1))
            {
                //down
                if (mouseRectangle.Intersects(s_downArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 - 60, 338), null, Color.White, 0f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.DownArrowHovered = true;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.RightArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 - 60, 338), null, Color.White, 0f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.DownArrowHovered = false;
                }
                //up
                if (mouseRectangle.Intersects(s_upArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 + 300, 338), null, Color.White, (float)Math.PI, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.UpArrowHovered = true;
                    InterfaceSettings.DownArrowHovered = false;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.RightArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 + 300, 338), null, Color.White, (float)Math.PI, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.UpArrowHovered = false;
                }
            }
            else
            {
                //up
                if (mouseRectangle.Intersects(s_upArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 + 300, 338), null, Color.White, (float)Math.PI, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.UpArrowHovered = true;
                    InterfaceSettings.DownArrowHovered = false;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.RightArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 + 300, 338), null, Color.White, (float)Math.PI, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.UpArrowHovered = false;
                }
            }
            if (InterfaceSettings.CurrentlySelectedSongKey == 0)
            {
                //right
                if (mouseRectangle.Intersects(s_rightArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 + 300, 538), null, Color.White, (float)Math.PI * 1.5f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.RightArrowHovered = true;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.DownArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 + 300, 538), null, Color.White, (float)Math.PI * 1.5f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.RightArrowHovered = false;
                }
            }
            else if (InterfaceSettings.CurrentlySelectedSongKey > 0 && InterfaceSettings.CurrentlySelectedSongKey < (SoundManager.music.Count - 1))
            {
                //right
                if (mouseRectangle.Intersects(s_rightArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 + 300, 538), null, Color.White, (float)Math.PI * 1.5f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.RightArrowHovered = true;
                    InterfaceSettings.LeftArrowHovered = false;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.DownArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 + 300, 538), null, Color.White, (float)Math.PI * 1.5f, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.RightArrowHovered = false;
                }
                //left
                if (mouseRectangle.Intersects(s_leftArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 - 60, 538), null, Color.White, (float)Math.PI / 2, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.LeftArrowHovered = true;
                    InterfaceSettings.RightArrowHovered = false;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.DownArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 - 60, 538), null, Color.White, (float)Math.PI / 2, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.LeftArrowHovered = false;
                }
            }
            else
            {
                //left
                if (mouseRectangle.Intersects(s_leftArrowBox))
                {
                    spriteBatch.Draw(Textures.ArrowKeyHover, new Vector2(1280 / 2 - 60, 538), null, Color.White, (float)Math.PI / 2, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.LeftArrowHovered = true;
                    InterfaceSettings.RightArrowHovered = false;
                    InterfaceSettings.UpArrowHovered = false;
                    InterfaceSettings.DownArrowHovered = false;
                }
                else
                {
                    spriteBatch.Draw(Textures.ArrowKey, new Vector2(1280 / 2 - 60, 538), null, Color.White, (float)Math.PI / 2, arrowKeyOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    InterfaceSettings.LeftArrowHovered = false;
                }
            }

            spriteBatch.End();
        }

        private static void GetPlayerName(GameTime gameTime, SpriteBatch spriteBatch)
        {
            bool caretVisible = (gameTime.TotalGameTime.TotalMilliseconds % 1000) >= 500;

            spriteBatch.DrawString(Fonts.SpriteFont, "Your Name:", new Vector2(570, 188), Color.White);
            Vector2 size = Fonts.SpriteFont.MeasureString("Your Name: ");
            spriteBatch.DrawString(Fonts.SpriteFont, InterfaceSettings.PlayerName, new Vector2(10 + size.X + 570, 188), Color.White);

            if (caretVisible)
            {
                Vector2 inputLength = Fonts.SpriteFont.MeasureString(InterfaceSettings.PlayerName + "Your Name: ");
                spriteBatch.Draw(Textures.Cursor, new Vector2(11 + inputLength.X + 570, 188), Color.White);
            }
        }
    }
}
