using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nuclex.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects.Environment;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.GameOptions;
using SpoidaGamesArcadeLibrary.Interface.Screen;
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

        //Sounds
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        private Cue selectedSong;

        //Input
        private InputManager input;

        //Physical World
        private BasketballManager basketballManager;
        private Body backboardBody;
        private Body leftRimBody;
        private Body rightRimBody;

        //Random
        private Random rand = new Random();

        //Fonts
        private SpriteFont pixel;
        private SpriteFont pixelGlowFont;

        //Starfield
        private Starfield starField;
        
        //Sounds
        private SoundEffect basketBallShotSoundEffect;
        private SoundEffect basketScoredSoundEffect;
        private SoundEffect collisionSoundEffect;
        private Song ambientSpaceSong;
        private SoundManager soundManager;

        //Particles
        private SparkleEmitter basketballSparkle;

        //Game goals
        private GoalManager goalManager = new GoalManager(100, true, new Rectangle(85, 208, 76, 1));
        
        private bool highScoresLoaded;
        private StringBuilder highScoresPlayers = new StringBuilder();
        private StringBuilder highScoresScore = new StringBuilder();
        private StringBuilder highScoresStreak = new StringBuilder();
        private StringBuilder playerName = new StringBuilder();
        private bool nameToShort;

        private const string HIGH_SCORES_FILENAME = "highscores.lst";
        private HighScoreManager highScoreManager;
        string fullHighScorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HIGH_SCORES_FILENAME);

        private PlayerOptions playerOptions = new PlayerOptions();
        private int currentlySelectedBasketballKey;
        private int currentlySelectedSongKey;
        private KeyboardState cachedUpDownKeyboardState;
        private KeyboardState cachedRightLeftKeyboardState;

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

            highScoreManager = new HighScoreManager(fullHighScorePath);
            
            if (!File.Exists(highScoreManager.HighScoreFilePath))
            {
                List<HighScore> tempList = new List<HighScore> { new HighScore("Jerry Rice", 2000, 1, 5), new HighScore("Glen Rice", 1000, 1, 5) };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(tempList);
                string encryptedJson = highScoreManager.EncodeHighScores(json);

                using (FileStream fileStream = File.Create(highScoreManager.HighScoreFilePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(encryptedJson);
                    }
                }
                highScoreManager.HighScores = tempList;
            }
            else
            {
                highScoreManager.LoadHighScoresFromDisk();
            }

            audioEngine = new AudioEngine("Content\\Audio\\GalaxyJamAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            basketballManager = new BasketballManager(Content);
            soundManager = new SoundManager(soundBank);
            
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
            galaxyJamLogo = Content.Load<Texture2D>(@"Textures/GalaxyJamConcept");
            backboardSprite = Content.Load<Texture2D>(@"Textures/Backboard2");
            backboardSpriteGlow = Content.Load<Texture2D>(@"Textures/Backboard2Glow");
            rimSprite = Content.Load<Texture2D>(@"Textures/Rim2");
            rimSpriteGlow = Content.Load<Texture2D>(@"Textures/Rim2Glow");
            lineSprite = Content.Load<Texture2D>(@"Textures/LineSprite");
            twopxsolidstar = Content.Load<Texture2D>(@"Textures/2x2SolidStar");
            fourpxblurstar = Content.Load<Texture2D>(@"Textures/4x4BlurStar");
            onepxsolidstar = Content.Load<Texture2D>(@"Textures/1x1SolidStar");
            cursor = Content.Load<Texture2D>(@"Textures/Cursor");
        }

        private void LoadFonts()
        {
            pixel = Content.Load<SpriteFont>(@"Fonts/PixelFont");
            pixelGlowFont = Content.Load<SpriteFont>(@"Fonts/PixelScoreGlow");
        }

        private void LoadSoundEffectsAndSounds()
        {
            basketBallShotSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/BasketballShot");
            basketScoredSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/BasketScored");
            collisionSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/Collision");
            ambientSpaceSong = Content.Load<Song>(@"Audio/Music/AmbientSpace");
            MediaPlayer.Play(ambientSpaceSong);
            MediaPlayer.IsRepeating = true;
            soundManager.SelectMusic(SongTypes.BouncyLoop1);
        }

        private void LoadEffectsAndParticles()
        {
            //Load Starfield
            List<Texture2D> starTextures = new List<Texture2D> { twopxsolidstar, fourpxblurstar, onepxsolidstar };
            starField = new Starfield(Window.ClientBounds.Width, Window.ClientBounds.Height, 1000, starTextures);
            starField.StarSpeedModifier = 1;
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
                    starField.Update(gameTime);
                    break;
                case GameStates.OptionsScreen:
                    starField.Update(gameTime);

                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                    {
                        if (currentlySelectedSongKey > 0)
                        {
                            currentlySelectedSongKey--;
                        }
                    }
                    else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                    {
                        if (currentlySelectedSongKey < SoundManager.music.Count - 1)
                        {
                            currentlySelectedSongKey++;
                        }
                    }
                    cachedRightLeftKeyboardState = input.GetKeyboard().GetState();

                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
                    {
                        if (currentlySelectedBasketballKey > 0)
                        {
                            currentlySelectedBasketballKey--;
                        }
                    }
                    else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
                    {
                        if (currentlySelectedBasketballKey < BasketballManager.basketballs.Count - 1)
                        {
                            currentlySelectedBasketballKey++;
                        }
                    }
                    cachedUpDownKeyboardState = input.GetKeyboard().GetState();
                    
                    break;
                case GameStates.GetReadyState:
                    starField.StarSpeedModifier = 1;
                    starField.Update(gameTime);
                    break;
                case GameStates.Playing:
                    starField.Update(gameTime);
                    BasketballManager.SelectedBasketball.Update(gameTime);
                    PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
                    
                    HandlePlayerInput();
                    HandleBasketballPosition();

                    basketballSparkle.EmitterLocation = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                    basketballSparkle.Update();

                    Vector2 basketballCenter = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
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
                        if (basketballManager.BasketballBody.Awake == false)
                        {
                            highScoreManager.SaveHighScore(playerOptions.PlayerName, goalManager.GameScore, goalManager.TopStreak, goalManager.ScoreMulitplier);
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
                    starField.Update(gameTime);
                    if (!highScoresLoaded)
                    {
                        foreach (HighScore highScore in highScoreManager.HighScores)
                        {
                            highScoresPlayers.AppendLine(String.Format("{0}", highScore.CurrentPlayerName));
                            highScoresScore.AppendLine(String.Format("{0}", highScore.PlayerScore));
                            highScoresStreak.AppendLine(String.Format("{0}", highScore.PlayerTopStreak));
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
                    starField.Draw(spriteBatch);
                    GameInterface.DrawTitleScreen(spriteBatch, galaxyJamLogo);
                    spriteBatch.End();
                    break;
                case GameStates.OptionsScreen:
                    spriteBatch.Begin();
                    spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
                    starField.Draw(spriteBatch);
                    GetPlayerName(gameTime);
                    GameInterface.DrawOptionsInterface(spriteBatch, pixel, pixelGlowFont, nameToShort, currentlySelectedBasketballKey, currentlySelectedSongKey);
                    spriteBatch.End();
                    break;
                case GameStates.GetReadyState:
                    DrawGameWorld(gameTime);
                    break;
                case GameStates.Playing:
                    DrawGameWorld(gameTime);
                    break;
                case GameStates.Paused:
                    DrawGameWorld(gameTime);
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

        private const double EFFECT_TIME = 1000;
        private double effectTimer;
        private void DrawGameWorld(GameTime gameTime)
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
            spriteBatch.Draw(BasketballManager.SelectedBasketball.BasketballTexture, (basketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.SelectedBasketball.Source, Color.White, basketballManager.BasketballBody.Rotation, BasketballManager.SelectedBasketball.Origin, 1f, SpriteEffects.None, 0f);
            //draw backboard
            spriteBatch.Draw(backboardCollisionHappened ? backboardSpriteGlow : backboardSprite, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
            //draw left rim
            spriteBatch.Draw(leftRimCollisionHappened ? rimSpriteGlow : rimSprite, leftRimPosition, null, Color.White, 0f, leftRimOrigin, 1f, SpriteEffects.None, 0f);
            //draw right rim
            spriteBatch.Draw(rightRimCollisionHappened ? rimSpriteGlow : rimSprite, rightRimPosition, null, Color.White, 0f, rightRimOrigin, 1f, SpriteEffects.None, 0f);

            GameInterface.DrawPlayingInterface(spriteBatch, pixel, pixelGlowFont, goalManager);

            if (goalManager.DrawSwish)
            {
                effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                spriteBatch.DrawString(pixelGlowFont, "SWISH!", new Vector2(1280 / 2, 720 / 2), Color.White);
                if (EFFECT_TIME < effectTimer)
                {
                    effectTimer = 0;
                    goalManager.DrawSwish = false;
                }
            }

            if (goalManager.DrawCleanShot)
            {
                effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                spriteBatch.DrawString(pixelGlowFont, "Clean Shot!", new Vector2(1280 / 2, 720 / 2), Color.White);
                if (EFFECT_TIME < effectTimer)
                {
                    effectTimer = 0;
                    goalManager.DrawCleanShot = false;
                }
            }

            if (!String.IsNullOrEmpty(goalManager.DrawStreakMessage))
            {
                spriteBatch.DrawString(pixelGlowFont, goalManager.DrawStreakMessage, new Vector2(1280 / 2, 620), Color.White);
            }

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

        private void HandlePlayerInput()
        {
            MouseState state = input.GetMouse().GetState();
            if (basketballManager.BasketballBody.Awake == false)
            {
                if (state.LeftButton == ButtonState.Pressed)
                {
                    PhysicalWorld.World.Gravity.Y = 25;
                    basketballManager.BasketballBody.Awake = true;
                    HandleShotAngle(state);
                    SoundManager.PlaySoundEffect(basketBallShotSoundEffect, 0.3f, 0.0f, 0.0f);
                }
            }
        }

        private void HandleBasketballPosition()
        {
            if (basketballManager.BasketballBody.Position.Y > 720 / PhysicalWorld.MetersInPixels)
            {
                PhysicalWorld.World.Gravity.Y = 0;
                basketballManager.BasketballBody.Awake = false;
                basketballManager.BasketballBody.Position = RandomizePosition();
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
            basketballManager.BasketballBody.Awake = false;
            basketballManager.BasketballBody.Position = RandomizePosition();
        }

        private Vector2 RandomizePosition()
        {
            return new Vector2((rand.Next(370, 1230)) / PhysicalWorld.MetersInPixels, (rand.Next(310, 680)) / PhysicalWorld.MetersInPixels);
        }

        private void HandleShotAngle(MouseState state)
        {
            Vector2 basketballLocation = new Vector2(basketballManager.BasketballBody.Position.X * PhysicalWorld.MetersInPixels,
                                                     basketballManager.BasketballBody.Position.Y * PhysicalWorld.MetersInPixels);
            Vector2 mouseLocation = new Vector2(state.X, state.Y);

            double radians = MouseAngle(basketballLocation, mouseLocation);
            Vector2 pointingAt = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));

            float distance = Vector2.Distance(basketballLocation, mouseLocation);

            Vector2 shotVector = new Vector2(MathHelper.Clamp((pointingAt.X * distance) / (PhysicalWorld.MetersInPixels * 1.5f), -3, 3), MathHelper.Clamp(((pointingAt.Y * distance) / (PhysicalWorld.MetersInPixels)), -4, 3));

            basketballManager.BasketballBody.ApplyLinearImpulse(shotVector);
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
                    MediaPlayer.Stop();
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
