using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.GameStates
{
    public class GetReadyScreenState
    {
        private const double GAME_START_COUNTDOWN_LENGTH = 4000;
        private static double s_gameStartCountdownTimer;
        private static double s_gameStartAlphaTimer;
        private static float s_gameStartAlphaFade = 255;
        public static int SoundEffectCounter = 1;

        public static void Update(GameTime gameTime, int mode)
        {
            InterfaceSettings.StarField.StarSpeedModifier = 1;

            s_gameStartCountdownTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            s_gameStartAlphaTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            float amountToFade = MathHelper.Clamp((float)s_gameStartAlphaTimer / 1000, 0, 1);
            float value = MathHelper.Lerp(255, 0, amountToFade);
            s_gameStartAlphaFade = value;

            if (s_gameStartAlphaTimer >= 1000)
            {
                s_gameStartAlphaTimer = 0;
                s_gameStartAlphaFade = 255;
            }

            if (s_gameStartCountdownTimer >= GAME_START_COUNTDOWN_LENGTH)
            {
                if (mode == 0)
                {
                    GameState.States = GameState.GameStates.Playing;
                    GameTimer.GameTime = new TimeSpan(0, 0, 2, 0);
                }
                else
                {
                    GameState.States = GameState.GameStates.ArcadeMode;
                    GameTimer.GameTime = new TimeSpan(0, 0, 3, 0);
                }
                GameTimer.StartGameTimer();
                s_gameStartCountdownTimer = 0;
            }
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            if (s_gameStartCountdownTimer < 1000)
            {
                Vector2 threeOrigin = Fonts.GiantRedPixelFont.MeasureString("3");
                spriteBatch.DrawString(Fonts.GiantRedPixelFont, "3", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)s_gameStartAlphaFade), 0f, threeOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                if (SoundEffectCounter == 1)
                {
                    SoundManager.PlaySoundEffect(Sounds.CountdownBeepSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
                    SoundEffectCounter++;
                }
            }
            else if (s_gameStartCountdownTimer < 2000)
            {
                Vector2 twoOrigin = Fonts.GiantRedPixelFont.MeasureString("2");
                spriteBatch.DrawString(Fonts.GiantRedPixelFont, "2", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)s_gameStartAlphaFade), 0f, twoOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                if (SoundEffectCounter == 2)
                {
                    SoundManager.PlaySoundEffect(Sounds.CountdownBeepSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
                    SoundEffectCounter++;
                }
            }
            else if (s_gameStartCountdownTimer < 3000)
            {
                Vector2 oneOrigin = Fonts.GiantRedPixelFont.MeasureString("1");
                spriteBatch.DrawString(Fonts.GiantRedPixelFont, "1", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)s_gameStartAlphaFade), 0f, oneOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                if (SoundEffectCounter == 3)
                {
                    SoundManager.PlaySoundEffect(Sounds.CountdownBeepSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
                    SoundEffectCounter++;
                }
            }
            else
            {
                Vector2 goOrigin = Fonts.GiantRedPixelFont.MeasureString("Go!");
                spriteBatch.DrawString(Fonts.GiantRedPixelFont, "Go!", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)s_gameStartAlphaFade), 0f, goOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                if (SoundEffectCounter == 4)
                {
                    SoundManager.PlaySoundEffect(Sounds.CountdownGoSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0f, 0f);
                    SoundEffectCounter++;
                }
            }

            spriteBatch.End();
        }
    }
}
