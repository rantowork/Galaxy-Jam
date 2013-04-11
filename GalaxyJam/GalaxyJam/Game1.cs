using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects.Environment;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.GameOptions;
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
        private Texture2D lineSprite;
        private Texture2D backboardSprite;
        private Texture2D rimSprite;
        private Texture2D rimSpriteGlow;
        private Texture2D galaxyJamLogo;
        private Texture2D backboardSpriteGlow;
        private Texture2D twopxsolidstar;
        private Texture2D fourpxblurstar;
        private Texture2D onepxsolidstar;
        private Texture2D cursor;

        private Rectangle redRectangle = new Rectangle(1100, 140, 32, 32);
        private Rectangle greenRectangle = new Rectangle(1100, 210, 32, 32);
        private Rectangle yellowRectangle = new Rectangle(1100, 280, 32, 32);

        //Basketballs
        private Texture2D greenGlowBasketball;
        private Texture2D redGlowBasketball;
        private Texture2D yellowGlowBasketball;

        //Sounds
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue currentlySelectedSong;

        //Input
        private InputManager input;

        //Physical World
        private BasketballManager basketball;
        private Body backboardBody;
        private Body leftRimBody;
        private Body rightRimBody;

        //Random
        private Random rand = new Random();

        //Fonts
        private SpriteFont segoe;
        private SpriteFont pixel;
        private SpriteFont pixelGlowFont;

        //Starfield
        private Starfield starField;
        
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
        private StringBuilder playerName = new StringBuilder();
        private bool nameToShort;

        private PlayerOptions playerOptions = new PlayerOptions();

        //Shot Constants
        private const double TEXT_FADE_TIME = 2000;
        private double textFadeTimer;
        
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

            PhysicalWorld.World = new World(Vector2.Zero);
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

            audioEngine = new AudioEngine("Content\\Audio\\GalaxyJamAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            basketball = new BasketballManager();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadTextures();
            LoadFonts();
            LoadSoundEffectsAndSounds();
            LoadEffectsAndParticles();
            LoadPhysicalWorldEntities();
            
            camera = new Camera(GraphicsDevice.Viewport)
                         {
                             Limits = null
                         };
        }

        private void LoadTextures()
        {
            galaxyJamLogo = Textures.LoadPersistentTexture("Textures/GalaxyJamLogo", Content);

            basketball.LoadBasketballs(Content);
            basketball.SelectBasketball(BasketballTypes.PurpleSkullBall);
            
            backboardSprite = Textures.LoadPersistentTexture("Textures/Backboard2", Content);
            backboardSpriteGlow = Textures.LoadPersistentTexture("Textures/Backboard2Glow", Content);
            rimSprite = Textures.LoadPersistentTexture("Textures/Rim2", Content);
            rimSpriteGlow = Textures.LoadPersistentTexture("Textures/Rim2Glow", Content);
            lineSprite = Textures.LoadPersistentTexture("Textures/LineSprite", Content);
            twopxsolidstar = Textures.LoadPersistentTexture("Textures/2x2SolidStar", Content);
            fourpxblurstar = Textures.LoadPersistentTexture("Textures/4x4BlurStar", Content);
            onepxsolidstar = Textures.LoadPersistentTexture("Textures/1x1SolidStar", Content);
            cursor = Textures.LoadPersistentTexture("Textures/Cursor", Content);

            //Temporary loading basketballs
            redGlowBasketball = Content.Load<Texture2D>(@"Textures/Basketballs/RedGlowBall");
            greenGlowBasketball = Content.Load<Texture2D>(@"Textures/Basketballs/GreenGlowBall");
            yellowGlowBasketball = Content.Load<Texture2D>(@"Textures/Basketballs/YellowGlowBall");
        }

        private void LoadFonts()
        {
            segoe = Fonts.LoadPersistentFont("Fonts/Segoe", Content);
            pixel = Fonts.LoadPersistentFont("Fonts/PixelFont", Content);
            pixelGlowFont = Fonts.LoadPersistentFont("Fonts/PixelScoreGlow", Content);
        }

        private void LoadSoundEffectsAndSounds()
        {
            basketBallShotSoundEffect = SoundEffects.LoadPersistentSoundEffect("Audio/SoundEffects/BasketballShot", Content);
            basketScoredSoundEffect = SoundEffects.LoadPersistentSoundEffect("Audio/SoundEffects/BasketScored", Content);
            collisionSoundEffect = SoundEffects.LoadPersistentSoundEffect("Audio/SoundEffects/Collision", Content);

            currentlySelectedSong = soundBank.GetCue(playerOptions.GetSelectedMusic());
        }

        private void LoadEffectsAndParticles()
        {
            //Load Starfield
            List<Texture2D> starTextures = new List<Texture2D> { twopxsolidstar, fourpxblurstar, onepxsolidstar };
            starField = new Starfield(Window.ClientBounds.Width, Window.ClientBounds.Height, 1000, starTextures);

            basketballSparkle = new SparkleEmitter(new List<Texture2D> { twopxsolidstar }, new Vector2(-40, -40));
        }

        private void LoadPhysicalWorldEntities()
        {
            backboardBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(64f / PhysicalWorld.MetersInPixels, 116f / PhysicalWorld.MetersInPixels), 6f, 140f, 1f, .3f, .1f);
            backboardBody.OnCollision += BackboardCollision;

            leftRimBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(80f / PhysicalWorld.MetersInPixels, 206 / PhysicalWorld.MetersInPixels), 10f, 16f, 1f, .3f, .1f);
            leftRimBody.OnCollision += LeftRimCollision;

            rightRimBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(166 / PhysicalWorld.MetersInPixels, 206 / PhysicalWorld.MetersInPixels), 10f, 16f, 1f, .3f, 1f);
            rightRimBody.OnCollision += RightRimCollision;
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
                    SoundManager.PlayBackgroundMusic(currentlySelectedSong);
                    break;
                case GameStates.GetReadyState:
                    starField.Update(gameTime);
                    break;
                case GameStates.Playing:
                    SoundManager.PlayBackgroundMusic(currentlySelectedSong);
                    starField.Update(gameTime);
                    BasketballManager.SelectedBasketball.Update(gameTime);
                    PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
                    
                    HandleInput();
                    HandlePosition();

                    basketballSparkle.EmitterLocation = basketball.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                    basketballSparkle.Update();

                    Vector2 basketballCenter = basketball.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
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
                        if (basketball.BasketballBody.Awake == false)
                        {
                            HighScoreManager.SaveHighScore(goalManager.GameScore, HIGH_SCORES_FILENAME, playerOptions.PlayerName, goalManager.TopStreak);
                            highScoreData = HighScoreManager.LoadHighScores(HIGH_SCORES_FILENAME);
                            highScoresPlayers.Clear();
                            highScoresScore.Clear();
                            highScoresStreak.Clear();
                            gameState = GameStates.GameEnd;
                        }
                    }

                    break;
                case GameStates.Paused:
                    break;
                case GameStates.GameEnd:
                    SoundManager.PlayBackgroundMusic(currentlySelectedSong);
                    starField.Update(gameTime);
                    if (!highScoresLoaded)
                    {
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
                    GameInterface.DrawTitleScreen(spriteBatch, galaxyJamLogo);
                    spriteBatch.End();
                    break;
                case GameStates.OptionsScreen:
                    spriteBatch.Begin();
                    spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
                    GetPlayerName(gameTime);
                    GameInterface.DrawOptionsInterface(spriteBatch, pixel, pixelGlowFont, nameToShort);

                    Vector2 ballSelectionCenterLine = pixel.MeasureString("Select a Basketball") / 2;
                    spriteBatch.Draw(redGlowBasketball, new Vector2(1000 + ballSelectionCenterLine.X, 140), null, Color.White, 0f, new Vector2(redGlowBasketball.Width/2, 0), 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(greenGlowBasketball, new Vector2(1000 + ballSelectionCenterLine.X, 210), null, Color.White, 0f, new Vector2(greenGlowBasketball.Width / 2, 0), 1.0f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(yellowGlowBasketball, new Vector2(1000 + ballSelectionCenterLine.X, 280), null, Color.White, 0f, new Vector2(yellowGlowBasketball.Width / 2, 0), 1.0f, SpriteEffects.None, 0f);

                    MouseState state = input.GetMouse().GetState();
                    Rectangle mouseLocation = new Rectangle(state.X, state.Y, 1, 1);

                    if (mouseLocation.Intersects(redRectangle))
                    {
                        spriteBatch.DrawString(pixel, "Red Glow Ball", new Vector2(1000 + ballSelectionCenterLine.X, 400), Color.White, 0, new Vector2(pixel.MeasureString("Red Glow Ball").X / 2, 0), 1f, SpriteEffects.None, 0f);
                    }
                    else if (mouseLocation.Intersects(greenRectangle))
                    {
                        spriteBatch.DrawString(pixel, "Green Glow Ball", new Vector2(1000 + ballSelectionCenterLine.X, 400), Color.White, 0, new Vector2(pixel.MeasureString("Green Glow Ball").X / 2, 0), 1f, SpriteEffects.None, 0f);
                    }
                    else if (mouseLocation.Intersects(yellowRectangle))
                    {
                        spriteBatch.DrawString(pixel, "Yellow Glow Ball", new Vector2(1000 + ballSelectionCenterLine.X, 400), Color.White, 0, new Vector2(pixel.MeasureString("Yellow Glow Ball").X/2,0), 1f, SpriteEffects.None, 0f);
                    }

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
                    GameInterface.DrawPausedInterface(spriteBatch, pixel, pixelGlowFont);
                    spriteBatch.End();
                    break;
                case GameStates.GameEnd:
                    DrawGameEnd();
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawGameWorld()
        {
            Vector2 backboardPosition = backboardBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 backboardOrigin = new Vector2(backboardSprite.Width / 2f, backboardSprite.Height / 2f);

            Vector2 leftRimPosition = leftRimBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 leftRimOrigin = new Vector2(rimSprite.Width / 2f, rimSprite.Height / 2f);

            Vector2 rightRimPosition = rightRimBody.Position * PhysicalWorld.MetersInPixels;
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
            spriteBatch.Draw(BasketballManager.SelectedBasketball.BasketballTexture, (basketball.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.SelectedBasketball.Source, Color.White, basketball.BasketballBody.Rotation, BasketballManager.SelectedBasketball.Origin, 1f, SpriteEffects.None, 0f);
            //draw backboard
            spriteBatch.Draw(backboardCollisionHappened ? backboardSpriteGlow : backboardSprite, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
            //draw left rim
            spriteBatch.Draw(leftRimCollisionHappened ? rimSpriteGlow : rimSprite, leftRimPosition, null, Color.White, 0f, leftRimOrigin, 1f, SpriteEffects.None, 0f);
            //draw right rim
            spriteBatch.Draw(rightRimCollisionHappened ? rimSpriteGlow : rimSprite, rightRimPosition, null, Color.White, 0f, rightRimOrigin, 1f, SpriteEffects.None, 0f);

            GameInterface.DrawPlayingInterface(spriteBatch, pixel, pixelGlowFont, goalManager);

            spriteBatch.End();
        }
        private void DrawGameEnd()
        {
            //Draw a full black background.  This helps with rendering the stars in front of it as the cleared viewport renders 3d space and we force it into 2d space with this.
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix);

            spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black); //background
            starField.Draw(spriteBatch); //stars!
            GameInterface.DrawGameEndInterface(spriteBatch, pixel, pixelGlowFont, goalManager);

            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(pixel, highScoresPlayers, new Vector2(10, 74), Color.White);
            spriteBatch.DrawString(pixel, highScoresStreak, new Vector2(170, 74), Color.White);
            spriteBatch.DrawString(pixel, highScoresScore, new Vector2(340, 74), Color.White);

            spriteBatch.End();
        }

        private void HandleInput()
        {
            MouseState state = input.GetMouse().GetState();
            if (basketball.BasketballBody.Awake == false)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    PhysicalWorld.World.Gravity.Y = 25;
                    basketball.BasketballBody.Awake = true;
                    HandleShotAngle(state);
                    SoundManager.PlaySoundEffect(basketBallShotSoundEffect, 0.3f, 0.0f, 0.0f);
                }
            }
        }

        private void HandlePosition()
        {
            if (basketball.BasketballBody.Position.Y > 720 / PhysicalWorld.MetersInPixels)
            {
                PhysicalWorld.World.Gravity.Y = 0;
                basketball.BasketballBody.Awake = false;
                basketball.BasketballBody.Position = RandomizePosition();
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
            PhysicalWorld.World.Gravity.Y = 0;
            basketball.BasketballBody.Awake = false;
            basketball.BasketballBody.Position = RandomizePosition();
        }

        private Vector2 RandomizePosition()
        {
            return new Vector2((rand.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (rand.Next(310, 680)) / PhysicalWorld.MetersInPixels);
        }

        private void HandleShotAngle(MouseState state)
        {
            Vector2 basketballLocation = new Vector2(basketball.BasketballBody.Position.X * PhysicalWorld.MetersInPixels,
                                                     basketball.BasketballBody.Position.Y * PhysicalWorld.MetersInPixels);
            Vector2 mouseLocation = new Vector2(state.X, state.Y);

            double radians = MouseAngle(basketballLocation, mouseLocation);
            Vector2 pointingAt = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));

            float distance = Vector2.Distance(basketballLocation, mouseLocation);

            Vector2 shotVector = new Vector2(MathHelper.Clamp((pointingAt.X * distance) / (PhysicalWorld.MetersInPixels * 1.5f), -3, 3), MathHelper.Clamp(((pointingAt.Y * distance) / (PhysicalWorld.MetersInPixels)), -4, 3));

            basketball.BasketballBody.ApplyLinearImpulse(shotVector);
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
                if (character == '\b')
                { // backspace
                    if (playerName.Length > 0)
                    {
                        playerName.Remove(playerName.Length - 1, 1);
                    }
                }
                else
                {
                    if (playerName.Length != 12)
                    {
                        if (character != 13)
                            playerName.Append(character);
                    }
                }
                //commit options
                if (character == 13)
                {
                    if (playerName.Length < 3)
                    {
                        nameToShort = true;
                    }
                    else
                    {
                        playerOptions.PlayerName = playerName.ToString();
                        camera.Limits = new Rectangle(0, 0, 1280, 720);
                        camera.ResetCamera();
                        gameState = GameStates.Playing;
                        GameTimer.StartGameTimer();   
                    }
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
                    SoundManager.MuteSounds(currentlySelectedSong);
                    GameTimer.StopGameTimer();
                }

                if (character == 112)
                {
                    SoundManager.PauseBackgroundMusic(currentlySelectedSong);
                }

                if (character == 109)
                {
                    SoundManager.MuteSounds(currentlySelectedSong);
                }
            }
            else if (gameState == GameStates.Paused)
            {
                if (character == 27)
                {
                    gameState = GameStates.Playing;
                    SoundManager.MuteSounds(currentlySelectedSong);
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

        private void GetPlayerName(GameTime gameTime)
        {
            bool caretVisible = (gameTime.TotalGameTime.TotalMilliseconds % 1000) >= 500;

            spriteBatch.DrawString(pixel, "Player Name:", new Vector2(1280 / 2 - 180, 200), Color.White);
            Vector2 size = pixel.MeasureString("Player Name: ");
            spriteBatch.DrawString(pixel, playerName, new Vector2(10 + size.X + 1280 / 2 - 180, 200), Color.White);

            if (caretVisible)
            {
                Vector2 inputLength = pixel.MeasureString(playerName + "Player Name: ");
                spriteBatch.Draw(cursor, new Vector2(11 + inputLength.X + 1280 / 2 - 180, 200), Color.White);
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
            SoundManager.PlaySoundEffect(collisionSoundEffect, 0.3f, 0.0f, 0.0f);
            return true;
        }

        public bool LeftRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            leftRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, 0.3f, 0.0f, 0.0f);
            return true;
        }

        public bool RightRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            rightRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, 0.3f, 0.0f, 0.0f);
            return true;
        }
        #endregion
    }
}
