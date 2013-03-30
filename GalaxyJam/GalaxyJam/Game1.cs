using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nuclex.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects.Environment;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

namespace GalaxyJam
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        //Camera & Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Camera camera;

        //Game State
        private enum GameStates
        {
            IntroScreens,
            StartScreen,
            OptionsScreen,
            GetReadyState,
            Playing,
            GameEnd,
            Paused
        } ;
        private GameStates gameState = GameStates.StartScreen;

        //Textures
        private Texture2D basketBallSprite;
        private Texture2D lineSprite;
        private Texture2D backboardSprite;
        private Texture2D rimSprite;
        private Texture2D rimSpriteGlow;
        private Texture2D galaxyJamLogo;
        private Texture2D backboardSpriteGlow;
        private Texture2D twopxsolidstar;
        private Texture2D fourpxblurstar;
        private Texture2D onepxsolidstar;

        //Input
        private InputManager input;

        //Physics
        private const float METER_IN_PIXEL = 64f;
        private World world;
        private Body basketBallBody;
        private Body backboardBody;
        private Body leftRimBody;
        private Body rightRimBody;

        //Random
        private Random rand = new Random();

        //Fonts
        private SpriteFont segoe;
        private SpriteFont pixel;

        //Starfield
        private Starfield starField;

        //Music
        private Song bgm;

        //Sounds
        private SoundEffect basketBallShotSoundEffect;
        private SoundEffect basketScoredSoundEffect;
        private SoundEffect collisionSoundEffect;

        //Particles
        private SparkleEmitter basketballSparkle;

        //Game goals
        private GoalManager goalManager = new GoalManager(100, true, new Rectangle(85, 208, 76, 1));
        private const string HIGH_SCORES_FILENAME = "highscores.lst";
        private HighScoreManager.HighScoreData highScoreData;
        private bool highScoresLoaded;
        private StringBuilder highScoresPlayers = new StringBuilder();
        private StringBuilder highScoresScore = new StringBuilder();
        private StringBuilder highScoresStreak = new StringBuilder();

        //screen shake get me outta here!

        //collisions get me outta here!
        private const double GLOWTIME = 200;
        private bool backboardCollisionHappened;
        private double backboardGlowTimer;

        private bool leftRimCollisionHappened;
        private double leftrimGlowTimer;

        private bool rightRimCollisionHappened;
        private double rightrimGlowTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this) { PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720 };

            Content.RootDirectory = "Content";
            input = new InputManager(Services, Window.Handle);
            Components.Add(input);
            //Components.Add(new ParticleEmitter(this, 100));

            world = new World(Vector2.Zero);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsMouseVisible = true;
            input.GetKeyboard().CharacterEntered += GamePlayInput;
            string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HIGH_SCORES_FILENAME);
            if (!File.Exists(fullpath))
            {
                //If the file doesn't exist, make a fake one...
                // Create the data to save
                HighScoreManager.HighScoreData data = new HighScoreManager.HighScoreData(10);
                data.playerName[0] = "Null Man";
                data.streak[0] = 1;
                data.score[0] = 100;

                HighScoreManager.SaveHighScores(data, HIGH_SCORES_FILENAME);
            }

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Load Textures
            galaxyJamLogo = Textures.LoadPersistentTexture("Textures/GalaxyJamLogo", Content);
            basketBallSprite = Textures.LoadPersistentTexture("Textures/Basketballs/BasketBall2", Content);
            backboardSprite = Textures.LoadPersistentTexture("Textures/Backboard2", Content);
            backboardSpriteGlow = Textures.LoadPersistentTexture("Textures/Backboard2Glow", Content);
            rimSprite = Textures.LoadPersistentTexture("Textures/Rim2", Content);
            rimSpriteGlow = Textures.LoadPersistentTexture("Textures/Rim2Glow", Content);
            lineSprite = Textures.LoadPersistentTexture("Textures/LineSprite", Content);
            twopxsolidstar = Textures.LoadPersistentTexture("Textures/2x2SolidStar", Content);
            fourpxblurstar = Textures.LoadPersistentTexture("Textures/4x4BlurStar", Content);
            onepxsolidstar = Textures.LoadPersistentTexture("Textures/1x1SolidStar", Content);
            List<Texture2D> starTextures = new List<Texture2D> { twopxsolidstar, fourpxblurstar, onepxsolidstar };
            #endregion

            #region Load Fonts
            segoe = Fonts.LoadPersistentFont("Fonts/Segoe", Content);
            pixel = Fonts.LoadPersistentFont("Fonts/PixelFont", Content);
            #endregion

            basketBallShotSoundEffect = SoundEffects.LoadPersistentSoundEffect("SoundEffects/BasketballShot", Content);
            basketScoredSoundEffect = SoundEffects.LoadPersistentSoundEffect("SoundEffects/BasketScored", Content);
            collisionSoundEffect = SoundEffects.LoadPersistentSoundEffect("SoundEffects/Collision", Content);

            bgm = Music.LoadPersistentSong("Music/SpaceLoop3", Content);

            Vector2 basketBallPosition = new Vector2((rand.Next(370, 1230)) / METER_IN_PIXEL, (rand.Next(310, 680)) / METER_IN_PIXEL);
            basketBallBody = BodyFactory.CreateCircle(world, 32f / (2f * METER_IN_PIXEL), 1f, basketBallPosition);
            basketBallBody.BodyType = BodyType.Dynamic;
            basketBallBody.Restitution = 0.3f;
            basketBallBody.Friction = 0.5f;

            backboardBody = StaticEntity.CreateStaticRectangleBody(world, new Vector2(64f / METER_IN_PIXEL, 116f / METER_IN_PIXEL), METER_IN_PIXEL, 6f, 140f, 1f, .3f, .1f);
            backboardBody.OnCollision += BackboardCollision;

            leftRimBody = StaticEntity.CreateStaticRectangleBody(world, new Vector2(80f/METER_IN_PIXEL, 206/METER_IN_PIXEL), METER_IN_PIXEL, 10f, 16f, 1f, .3f, .1f);
            leftRimBody.OnCollision += LeftRimCollision;

            rightRimBody = StaticEntity.CreateStaticRectangleBody(world, new Vector2(166/METER_IN_PIXEL, 206/METER_IN_PIXEL), METER_IN_PIXEL, 10f, 16f, 1f, .3f, 1f);
            rightRimBody.OnCollision += RightRimCollision;

            starField = new Starfield(Window.ClientBounds.Width, Window.ClientBounds.Height, 1000, starTextures);

            MediaPlayer.IsRepeating = true;

            basketballSparkle = new SparkleEmitter(new List<Texture2D> { twopxsolidstar }, new Vector2(-40, -40));

            camera = new Camera(GraphicsDevice.Viewport)
                         {
                             Limits = null
                         };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameStates.StartScreen:

                    break;
                case GameStates.OptionsScreen:

                    break;
                case GameStates.GetReadyState:
                    SoundManager.PlayBackgroundMusic(bgm, .8f);
                    starField.Update(gameTime);
                    break;
                case GameStates.Playing:
                    SoundManager.PlayBackgroundMusic(bgm, .8f);
                    starField.Update(gameTime);

                    world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
                    HandleInput();
                    HandlePosition();

                    basketballSparkle.EmitterLocation = basketBallBody.WorldCenter * METER_IN_PIXEL;
                    basketballSparkle.Update();

                    Vector2 basketballCenter = basketBallBody.WorldCenter * METER_IN_PIXEL;
                    Rectangle basketballCenterRectangle = new Rectangle((int)basketballCenter.X - 8, (int)basketballCenter.Y - 8, 16, 16);
                    goalManager.UpdateGoalScored(gameTime, camera, basketballCenterRectangle, basketScoredSoundEffect, basketballSparkle, starField);

                    if (backboardCollisionHappened)
                    {
                        GlowBackboard(gameTime);
                    }

                    if (leftRimCollisionHappened)
                    {
                        GlowLeftRim(gameTime);
                    }

                    if (rightRimCollisionHappened)
                    {
                        GlowRightRim(gameTime);
                    }

                    if (GameTimer.GetElapsedTimeSpan() >= new TimeSpan(0, 0, 2, 0))
                    {
                        GameTimer.StopGameTimer();
                        HighScoreManager.SaveHighScore(goalManager.GameScore, HIGH_SCORES_FILENAME, "Player1", goalManager.TopStreak);
                        highScoresPlayers.Clear();
                        highScoresScore.Clear();
                        highScoresStreak.Clear();
                        gameState = GameStates.GameEnd;
                    }

                    break;
                case GameStates.Paused:
                    break;
                case GameStates.GameEnd:
                    SoundManager.PlayBackgroundMusic(bgm, .8f);
                    starField.Update(gameTime);
                    if (!highScoresLoaded)
                    {
                        highScoreData = HighScoreManager.LoadHighScores(HIGH_SCORES_FILENAME);

                        for (int i = 0; i < highScoreData.count; i++)
                        {
                            if (highScoreData.playerName[i] != null)
                            {
                                highScoresPlayers.AppendLine(String.Format("{0}", highScoreData.playerName[i]));
                                highScoresScore.AppendLine(String.Format("{0}", highScoreData.score[i]));
                                highScoresStreak.AppendLine(String.Format("{0}", highScoreData.streak[i]));
                            }
                        }

                        highScoresLoaded = true;
                    }
                    break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case GameStates.StartScreen:
                    spriteBatch.Begin();
                    spriteBatch.Draw(galaxyJamLogo, new Rectangle(0, 0, 1280, 720), Color.White);
                    spriteBatch.End();
                    break;
                case GameStates.OptionsScreen:
                    spriteBatch.Begin();
                    spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
                    spriteBatch.End();
                    break;
                case GameStates.GetReadyState:
                    DrawGameWorld();
                    break;
                case GameStates.Playing:
                    DrawGameWorld();
                    break;
                case GameStates.Paused:
                    DrawGameWorld();
                    spriteBatch.Begin();
                    const string paused = "Paused!";
                    Vector2 pausedOrigin = pixel.MeasureString(paused) / 2;
                    spriteBatch.DrawString(pixel, paused, new Vector2(1280 / 2, 720 / 2), Color.White, 0, pausedOrigin, 1f, SpriteEffects.None, 0);
                    spriteBatch.End();
                    break;
                case GameStates.GameEnd:
                    spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix);
                    spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
                    starField.Draw(spriteBatch);
                    spriteBatch.End();

                    const string gameOver = "Game Over!";
                    string finalScore = String.Format("Final Score: {0}!", goalManager.GameScore);
                    string timeRemaining2 = String.Format("Time Remaining: {0}", String.Format("{0:00}:{1:00}", new TimeSpan(0, 0, 0, 0).Minutes, new TimeSpan(0, 0, 0, 0).Seconds));

                    Vector2 gameOverOrigin = pixel.MeasureString(gameOver) / 2;
                    Vector2 finalScoreOrigin = pixel.MeasureString(finalScore) / 2;

                    spriteBatch.Begin();
                    spriteBatch.DrawString(pixel, gameOver, new Vector2(1280 / 2, 340), Color.White, 0, gameOverOrigin, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(pixel, finalScore, new Vector2(1280 / 2, 370), Color.White, 0, finalScoreOrigin, 1f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(pixel, timeRemaining2, new Vector2(10, 694), Color.White);
                    spriteBatch.DrawString(pixel, "High Scores", new Vector2(10, 30), Color.White);
                    spriteBatch.DrawString(pixel, "Player", new Vector2(10, 50), Color.White);
                    spriteBatch.DrawString(pixel, "Top Streak", new Vector2(154, 50), Color.White);
                    spriteBatch.DrawString(pixel, "Score", new Vector2(324, 50), Color.White);
                    
                    spriteBatch.DrawString(pixel, highScoresPlayers, new Vector2(10, 74), Color.White);
                    spriteBatch.DrawString(pixel, highScoresStreak, new Vector2(154, 74), Color.White);
                    spriteBatch.DrawString(pixel, highScoresScore, new Vector2(324, 74), Color.White);
                    
                    spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawGameWorld()
        {
            Vector2 basketBallPosition = basketBallBody.Position * METER_IN_PIXEL;
            float basketBallRotation = basketBallBody.Rotation;
            Vector2 basketBallOrigin = new Vector2(basketBallSprite.Width / 2f, basketBallSprite.Height / 2f);

            Vector2 backboardPosition = backboardBody.Position * METER_IN_PIXEL;
            Vector2 backboardOrigin = new Vector2(backboardSprite.Width / 2f, backboardSprite.Height / 2f);

            Vector2 leftRimPosition = leftRimBody.Position * METER_IN_PIXEL;
            Vector2 leftRimOrigin = new Vector2(rimSprite.Width / 2f, rimSprite.Height / 2f);

            Vector2 rightRimPosition = rightRimBody.Position * METER_IN_PIXEL;
            Vector2 rightRimOrigin = new Vector2(rimSprite.Width / 2f, rimSprite.Height / 2f);

            //draw starfield separate from other draw methods to keep it simple
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix);
            spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
            starField.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix);
            basketballSparkle.Draw(spriteBatch);
            spriteBatch.End();

            //draw objects which contain a body that can have forces applied to it
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix);
            //draw basketball
            spriteBatch.Draw(basketBallSprite, basketBallPosition, null, Color.White, basketBallRotation, basketBallOrigin, 1f, SpriteEffects.None, 0f);
            //draw backboard
            spriteBatch.Draw(backboardCollisionHappened ? backboardSpriteGlow : backboardSprite, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
            //draw left rim
            spriteBatch.Draw(leftRimCollisionHappened ? rimSpriteGlow : rimSprite, leftRimPosition, null, Color.White, 0f, leftRimOrigin, 1f, SpriteEffects.None, 0f);
            //draw right rim
            spriteBatch.Draw(rightRimCollisionHappened ? rimSpriteGlow : rimSprite, rightRimPosition, null, Color.White, 0f, rightRimOrigin, 1f, SpriteEffects.None, 0f);

            string currentScore = String.Format("Player Score: {0}", goalManager.GameScore);
            spriteBatch.DrawString(segoe, currentScore, new Vector2(10, 10), Color.White);

            string currentMultiplier = String.Format("Score Multiplier: {0}", goalManager.ScoreMulitplier);
            spriteBatch.DrawString(pixel, currentMultiplier, new Vector2(1020, 694), Color.White);

            string currentStreak = String.Format("Streak: {0}", goalManager.Streak);
            spriteBatch.DrawString(segoe, currentStreak, new Vector2(1180, 22), Color.White);

            string timeRemaining = String.Format("Time Remaining: {0}", GameTimer.GetElapsedGameTime());
            spriteBatch.DrawString(pixel, timeRemaining, new Vector2(10, 694), Color.White);

            spriteBatch.End();
        }

        private void HandleInput()
        {
            MouseState state = input.GetMouse().GetState();
            if (basketBallBody.Awake == false)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    world.Gravity.Y = 25;
                    basketBallBody.Awake = true;
                    HandleShotAngle(state);
                    SoundManager.PlaySoundEffect(basketBallShotSoundEffect, 1.0f, 0.0f, 0.0f);
                }
            }
        }

        private void HandlePosition()
        {
            if (basketBallBody.Position.Y > 720 / METER_IN_PIXEL)
            {
                world.Gravity.Y = 0;
                basketBallBody.Awake = false;
                basketBallBody.Position = RandomizePosition();
                goalManager.GoalScored = false;
                goalManager.BackboardHit = false;
                goalManager.RimHit = false;
                if (goalManager.ScoredOnShot)
                {
                    goalManager.ScoredOnShot = false;
                }
                else if (!goalManager.ScoredOnShot)
                {
                    goalManager.Streak = 0;
                }
            }
        }

        private void ResetPosition()
        {
            world.Gravity.Y = 0;
            basketBallBody.Awake = false;
            basketBallBody.Position = RandomizePosition();
        }

        private Vector2 RandomizePosition()
        {
            return new Vector2((rand.Next(370, 1230)) / METER_IN_PIXEL, (rand.Next(310, 680)) / METER_IN_PIXEL);
        }

        private void HandleShotAngle(MouseState state)
        {
            Vector2 basketballLocation = new Vector2(basketBallBody.Position.X * METER_IN_PIXEL,
                                                     basketBallBody.Position.Y * METER_IN_PIXEL);
            Vector2 mouseLocation = new Vector2(state.X, state.Y);

            double radians = MouseAngle(basketballLocation, mouseLocation);
            Vector2 pointingAt = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));

            float distance = Vector2.Distance(basketballLocation, mouseLocation);

            Vector2 shotVector = new Vector2(MathHelper.Clamp((pointingAt.X * distance) / (METER_IN_PIXEL * 1.5f), -3, 3), MathHelper.Clamp(((pointingAt.Y * distance) / (METER_IN_PIXEL)), -4, 3));

            basketBallBody.ApplyLinearImpulse(shotVector);
        }

        private static double MouseAngle(Vector2 spriteLocation, Vector2 mouseLocation)
        {
            return Math.Atan2(mouseLocation.Y - (spriteLocation.Y), mouseLocation.X - (spriteLocation.X)); //this will return the angle(in radians) from sprite to mouse.
        }

        private void GamePlayInput(char character)
        {
            if (gameState == GameStates.StartScreen)
            {
                if (character == 13)
                {
                    gameState = GameStates.OptionsScreen;
                }
                if (character == 27)
                {
                    Exit();
                }
            }
            else if (gameState == GameStates.OptionsScreen)
            {
                if (character == 13)
                {
                    camera.Limits = new Rectangle(0, 0, 1280, 720);
                    camera.ResetCamera();
                    gameState = GameStates.Playing;
                    GameTimer.StartGameTimer();
                }
                if (character == 27)
                {
                    Exit();
                }
            }
            else if (gameState == GameStates.Playing)
            {
                if (character == 27)
                {
                    gameState = GameStates.Paused;
                    SoundManager.MuteSounds();
                    GameTimer.StopGameTimer();
                }

                if (character == 112)
                {
                    SoundManager.PauseBackgroundMusic();
                }

                if (character == 109)
                {
                    SoundManager.MuteSounds();
                }
            }
            else if (gameState == GameStates.Paused)
            {
                if (character == 27)
                {
                    gameState = GameStates.Playing;
                    SoundManager.MuteSounds();
                    GameTimer.StartGameTimer();
                }
            }
            else if (gameState == GameStates.GameEnd)
            {
                if (character == 27)
                {
                    Exit();
                }

                if (character == 116)
                {
                    ResetPosition();
                    goalManager.ResetGoalManager();
                    highScoresLoaded = false;
                    gameState = GameStates.Playing;
                    GameTimer.ResetTimer();
                    GameTimer.StartGameTimer();
                }
            }
        }

        #region Glow Events
        private void GlowBackboard(GameTime gameTime)
        {
            backboardGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (backboardGlowTimer > GLOWTIME)
            {
                backboardCollisionHappened = false;
                backboardGlowTimer = 0;
            }
        }

        private void GlowLeftRim(GameTime gameTime)
        {
            leftrimGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (leftrimGlowTimer > GLOWTIME)
            {
                leftRimCollisionHappened = false;
                leftrimGlowTimer = 0;
            }
        }

        private void GlowRightRim(GameTime gameTime)
        {
            rightrimGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (rightrimGlowTimer > GLOWTIME)
            {
                rightRimCollisionHappened = false;
                rightrimGlowTimer = 0;
            }
        }
        #endregion

        #region Collision Events
        public bool BackboardCollision(Fixture f1, Fixture f2, Contact contact)
        {
            backboardCollisionHappened = true;
            goalManager.BackboardHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, .8f, 0.0f, 0.0f);
            return true;
        }

        public bool LeftRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            leftRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, .8f, 0.0f, 0.0f);
            return true;
        }

        public bool RightRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            rightRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, .8f, 0.0f, 0.0f);
            return true;
        }
        #endregion
    }
}
