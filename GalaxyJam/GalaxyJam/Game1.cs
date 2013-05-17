using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
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
            TitleScreen,
            SettingsScreen,
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
        private Texture2D upIndicator;
        private Texture2D downIndicator;
        private Texture2D playTexture;
        private Texture2D settingsTexture;

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
        private SpriteFont giantRedPixelFont;
        private SpriteFont streakFont;

        //Starfield
        private Starfield starField;
        
        //Sounds
        private SoundEffect basketBallShotSoundEffect;
        private SoundEffect basketScoredSoundEffect;
        private SoundEffect collisionSoundEffect;
        private SoundEffect countdownGoSoundEffect;
        private SoundEffect countdownBeep;
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
        private StringBuilder highScoresMultiplier = new StringBuilder();
        private StringBuilder playerName = new StringBuilder();
        private bool nameToShort;

        private const string HIGH_SCORES_FILENAME = "highscores.lst";
        private HighScoreManager highScoreManager;
        private string fullHighScorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HIGH_SCORES_FILENAME);

        private PlayerOptions playerOptions = new PlayerOptions();
        private int currentlySelectedBasketballKey;
        private int currentlySelectedSongKey;
        private KeyboardState cachedUpDownKeyboardState;
        private KeyboardState cachedRightLeftKeyboardState;

        private const double GAME_START_COUNTDOWN_LENGTH = 4000;
        private double gameStartCountdownTimer;
        private double gameStartAlphaTimer;
        private float gameStartAlphaFade = 255;
        private int soundEffectCounter = 1;

        private const double NUMBER_SCROLL_EFFECT_TIME = 500;
        private static double numberScrollEffectTimer;
        private static StringBuilder numberScrollStringEffects = new StringBuilder();

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

        //Settings
        private byte titleScreenSelection;
        private const string SETTINGS_FILENAME = "game.settings";
        private string fullSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILENAME);
        private GameSettings gameSettings;
        private List<DisplayMode> displayModes = new List<DisplayMode>();
        private int currentResolution;
        private int currentSettingSelection;
        private DisplayMode defaultDisplayMode;
        private bool fullScreenSetting;
        private int musicVolumeSetting;
        private int soundEffectVolumeSetting;

        private DisplayMode previousDisplayMode;
        private bool previousFullScreenSetting;
        private int previousMusicSetting;
        private int previousSoundEffectSetting;
        private bool displaySettingsSavedMessage;
        private const double SAVE_TIME = 2000;
        private double displaySaveMessageTimer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            ResolutionManager.Init(ref graphics);
            ResolutionManager.SetVirtualResolution(1280, 720);
            ResolutionManager.SetResolution(1280, 720, false);

            foreach (DisplayMode mode in graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if (mode.Format == SurfaceFormat.Color)
                {
                    displayModes.Add(mode);
                    if (mode.Width == 1280 && mode.Height == 720)
                    {
                        defaultDisplayMode = mode;
                    }
                }
            }

            Content.RootDirectory = "Content";
            input = new InputManager(Services, Window.Handle);
            Components.Add(input);

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

            if (!File.Exists(fullSettingsPath))
            {
                gameSettings = new GameSettings(defaultDisplayMode.Width, defaultDisplayMode.Height, false, 10, 10);
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(gameSettings);
                using (FileStream fileStream = File.Create(fullSettingsPath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(json);
                    }
                }

                currentResolution = displayModes.IndexOf(defaultDisplayMode);
                fullScreenSetting = false;
                musicVolumeSetting = 10;
                soundEffectVolumeSetting = 10;
            }
            else
            {
                using (FileStream fileStream = File.Open(fullSettingsPath, FileMode.Open))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string fileData = streamReader.ReadToEnd();
                        gameSettings = javaScriptSerializer.Deserialize<GameSettings>(fileData);
                    }
                }
                if (gameSettings == null)
                {
                    gameSettings = new GameSettings(1280, 720, false, 10, 10);
                }
                else
                {
                    DisplayMode currentMode = GetDisplayMode(gameSettings.DisplayModeWidth,gameSettings.DisplayModeHeight);
                    currentResolution = displayModes.IndexOf(currentMode);
                    fullScreenSetting = gameSettings.IsFullScreen;
                    musicVolumeSetting = gameSettings.MusicVolume;
                    soundEffectVolumeSetting = gameSettings.SoundEffectVolume;
                }
            }

            ResolutionManager.SetResolution(gameSettings.DisplayModeWidth, gameSettings.DisplayModeHeight, fullScreenSetting);

            audioEngine = new AudioEngine("Content\\Audio\\GalaxyJamAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            base.Initialize();
        }

        private DisplayMode GetDisplayMode(int width, int height)
        {
            foreach (DisplayMode mode in graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if (mode.Format == SurfaceFormat.Color)
                {
                    if (mode.Width == width && mode.Height == height)
                    {
                        return mode;
                    }
                }
            }
            return defaultDisplayMode;
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
            downIndicator = Content.Load<Texture2D>(@"Textures/Interface/DownIndicator");
            upIndicator = Content.Load<Texture2D>(@"Textures/Interface/UpIndicator");
            playTexture = Content.Load<Texture2D>(@"Textures/Interface/Play");
            settingsTexture = Content.Load<Texture2D>(@"Textures/Interface/Settings");
        }

        private void LoadFonts()
        {
            pixel = Content.Load<SpriteFont>(@"Fonts/PixelFont");
            pixelGlowFont = Content.Load<SpriteFont>(@"Fonts/PixelScoreGlow");
            giantRedPixelFont = Content.Load<SpriteFont>(@"Fonts/GiantRedPixelFont");
            streakFont = Content.Load<SpriteFont>(@"Fonts/StreakText");
        }

        private void LoadSoundEffectsAndSounds()
        {
            basketBallShotSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/BasketballShot");
            basketScoredSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/BasketScored");
            collisionSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/Thud");
            countdownBeep = Content.Load<SoundEffect>(@"Audio/SoundEffects/Countdown");
            countdownGoSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/Go");
            ambientSpaceSong = Content.Load<Song>(@"Audio/Music/AmbientSpace");
            MediaPlayer.Play(ambientSpaceSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)gameSettings.MusicVolume/10;
            soundManager.SelectMusic(SongTypes.BouncyLoop1);
        }

        private void LoadEffectsAndParticles()
        {
            //Load Starfield
            List<Texture2D> starTextures = new List<Texture2D> { twopxsolidstar, fourpxblurstar, onepxsolidstar };
            starField = new Starfield(1280, 720, 1000, starTextures);
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
                case GameStates.TitleScreen:
                    starField.Update(gameTime);

                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
                    {
                        if (titleScreenSelection == 0)
                        {
                            titleScreenSelection = 1;
                        }
                        else
                        {
                            titleScreenSelection = 0;
                        }
                    }
                    else if(input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
                    {
                        if (titleScreenSelection == 0)
                        {
                            titleScreenSelection = 1;
                        }
                        else
                        {
                            titleScreenSelection = 0;
                        }
                    }
                    cachedUpDownKeyboardState = input.GetKeyboard().GetState();

                    break;
                case GameStates.SettingsScreen:
                    starField.Update(gameTime);

                    if (currentSettingSelection == 0)
                    {
                        if (input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                        {
                            if (currentResolution > 0)
                            {
                                currentResolution--;
                            }
                        }
                        else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                        {
                            if (currentResolution < displayModes.Count - 1)
                            {
                                currentResolution++;
                            }
                        }
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                    }
                    else if (currentSettingSelection == 1)
                    {
                        if (input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                        {
                            if (fullScreenSetting)
                            {
                                fullScreenSetting = false;
                            }
                            else
                            {
                                fullScreenSetting = true;
                            }
                        }
                        else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                        {
                            if (fullScreenSetting)
                            {
                                fullScreenSetting = false;
                            }
                            else
                            {
                                fullScreenSetting = true;
                            }
                        }
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                    }
                    else if (currentSettingSelection == 2)
                    {
                        if (input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                        {
                            if (musicVolumeSetting > 0)
                            {
                                musicVolumeSetting--;
                                MediaPlayer.Volume = (float) musicVolumeSetting/10;
                            }
                        }
                        else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                        {
                            if (musicVolumeSetting < 10)
                            {
                                musicVolumeSetting++;
                                MediaPlayer.Volume = (float)musicVolumeSetting / 10;
                            }
                        }
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                    }
                    else if (currentSettingSelection == 3)
                    {
                        if (input.GetKeyboard().GetState().IsKeyDown(Keys.Left) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Left))
                        {
                            if (soundEffectVolumeSetting > 0)
                            {
                                soundEffectVolumeSetting--;
                                collisionSoundEffect.Play((float)soundEffectVolumeSetting/10, 0f, 0f);
                            }
                        }
                        else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                        {
                            if (soundEffectVolumeSetting < 10)
                            {
                                soundEffectVolumeSetting++;
                                collisionSoundEffect.Play((float)soundEffectVolumeSetting / 10, 0f, 0f);
                            }
                        }
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                    }

                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
                    {
                        if (currentSettingSelection < 6)
                        {
                            currentSettingSelection++;
                        }
                        if (currentSettingSelection == 6)
                        {
                            currentSettingSelection = 0;
                        }
                    }
                    else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
                    {
                        if (currentSettingSelection == 0)
                        {
                            currentSettingSelection = 6;
                        }
                        if (currentSettingSelection > 0)
                        {
                            currentSettingSelection--;
                        }
                    }
                    cachedUpDownKeyboardState = input.GetKeyboard().GetState();

                    if (displaySettingsSavedMessage)
                    {
                        displaySaveMessageTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (SAVE_TIME <= displaySaveMessageTimer)
                        {
                            displaySaveMessageTimer = 0;
                            displaySettingsSavedMessage = false;
                        }
                    }

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

                    gameStartCountdownTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                    gameStartAlphaTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

                    float amountToFade = MathHelper.Clamp((float) gameStartAlphaTimer/1000, 0, 1);
                    float value = MathHelper.Lerp(255, 0, amountToFade);
                    gameStartAlphaFade = value;

                    if (gameStartAlphaTimer >= 1000)
                    {
                        gameStartAlphaTimer = 0;
                        gameStartAlphaFade = 255;
                    }

                    if (gameStartCountdownTimer >= GAME_START_COUNTDOWN_LENGTH)
                    {
                        gameState = GameStates.Playing;
                        GameTimer.StartGameTimer();
                        gameStartCountdownTimer = 0;
                    }

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
                    goalManager.UpdateGoalScored(gameTime, camera, basketballCenterRectangle, basketScoredSoundEffect, basketballSparkle, starField, gameSettings);

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

                    if (goalManager.DrawNumberScrollEffect)
                    {
                        numberScrollStringEffects.Clear();
                        for (int i = 0; i < goalManager.GameScore.ToString().Length; i++)
                        {
                            int number = rand.Next(0, 9);
                            numberScrollStringEffects.Append(number);
                        }
                        goalManager.NumberScrollScoreToDraw = numberScrollStringEffects.ToString();
                        numberScrollEffectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (NUMBER_SCROLL_EFFECT_TIME < numberScrollEffectTimer)
                        {
                            goalManager.DrawNumberScrollEffect = false;
                            numberScrollEffectTimer = 0;
                        }
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
                            highScoresMultiplier.Clear();
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
                            highScoresMultiplier.AppendLine(String.Format("{0}", highScore.PlayerMultiplier));
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
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    starField.Draw(spriteBatch);
                    GameInterface.DrawTitleScreen(spriteBatch, galaxyJamLogo);
                    spriteBatch.End();
                    break;
                case GameStates.TitleScreen:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    starField.Draw(spriteBatch);
                    if (titleScreenSelection == 0)
                    {
                        spriteBatch.Draw(playTexture, new Vector2(1280/2, 720/2), null, Color.White, 0f,
                                         new Vector2((float) playTexture.Width/2, (float) playTexture.Height/2), 1.0f,
                                         SpriteEffects.None, 1.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(settingsTexture, new Vector2(1280 / 2, 720 / 2), null, Color.White, 0f,
                                         new Vector2((float)settingsTexture.Width / 2, (float)settingsTexture.Height / 2), 1.0f,
                                         SpriteEffects.None, 1.0f);
                    }
                    spriteBatch.End();
                    break;
                case GameStates.SettingsScreen:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    starField.Draw(spriteBatch);

                    DisplayMode selectedMode = displayModes[currentResolution];
                    string resolutionText = String.Format("{0} x {1}", selectedMode.Width, selectedMode.Height);
                    string fullScreenText = fullScreenSetting ? "Full Screen" : "Windowed";

                    spriteBatch.DrawString(pixel, resolutionText, new Vector2(740, 300), Color.White);
                    spriteBatch.DrawString(pixel, fullScreenText, new Vector2(740, 325), Color.White);
                    spriteBatch.DrawString(pixel, musicVolumeSetting.ToString(), new Vector2(740, 350), Color.White);
                    spriteBatch.DrawString(pixel, soundEffectVolumeSetting.ToString(), new Vector2(740, 375), Color.White);

                    Vector2 selectionLocation;
                    Vector2 caretOrigin = Vector2.Zero;
                    switch(currentSettingSelection)
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
                            selectionLocation = new Vector2(1280/2-50, 450);
                            caretOrigin = pixel.MeasureString(">") / 2;
                            break;
                        case 5:
                            selectionLocation = new Vector2(1280/2-50, 475);
                            caretOrigin = pixel.MeasureString(">") / 2;
                            break;
                        default:
                            selectionLocation = new Vector2(300, 300);
                            break;
                    }

                    spriteBatch.DrawString(pixel, ">", selectionLocation, Color.White, 0f, caretOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, "  Resolution: ", new Vector2(300, 300), Color.White);
                    spriteBatch.DrawString(pixel, "  Full Screen: ", new Vector2(300, 325), Color.White);
                    spriteBatch.DrawString(pixel, "  Music Volume: ", new Vector2(300, 350), Color.White);
                    spriteBatch.DrawString(pixel, "  Sound Effect Volume: ", new Vector2(300, 375), Color.White);
                    Vector2 saveOrigin = pixel.MeasureString("Save")/2;
                    spriteBatch.DrawString(pixel, "Save", new Vector2(1280/2, 450), Color.White, 0f, saveOrigin, 1.0f,SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, "Back", new Vector2(1280/2, 475), Color.White, 0f, saveOrigin, 1.0f, SpriteEffects.None, 1.0f);

                    spriteBatch.End();
                    break;
                case GameStates.OptionsScreen:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
                    starField.Draw(spriteBatch);
                    GetPlayerName(gameTime);
                    GameInterface.DrawOptionsInterface(spriteBatch, pixel, pixelGlowFont, highScoreManager, nameToShort, currentlySelectedBasketballKey, currentlySelectedSongKey, downIndicator, upIndicator);
                    spriteBatch.End();
                    break;
                case GameStates.GetReadyState:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    starField.Draw(spriteBatch);
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

                    if (gameStartCountdownTimer < 1000)
                    {
                        Vector2 threeOrigin = giantRedPixelFont.MeasureString("3");
                        spriteBatch.DrawString(giantRedPixelFont, "3", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)gameStartAlphaFade), 0f, threeOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                        if (soundEffectCounter == 1)
                        {
                            SoundManager.PlaySoundEffect(countdownBeep, (float)gameSettings.SoundEffectVolume/10, 0f, 0f);
                            soundEffectCounter++;
                        }
                    }
                    else if (gameStartCountdownTimer < 2000)
                    {
                        Vector2 twoOrigin = giantRedPixelFont.MeasureString("2");
                        spriteBatch.DrawString(giantRedPixelFont, "2", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)gameStartAlphaFade), 0f, twoOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                        if (soundEffectCounter == 2)
                        {
                            SoundManager.PlaySoundEffect(countdownBeep, (float)gameSettings.SoundEffectVolume / 10, 0f, 0f);
                            soundEffectCounter++;
                        }
                    }
                    else if (gameStartCountdownTimer < 3000)
                    {
                        Vector2 oneOrigin = giantRedPixelFont.MeasureString("1");
                        spriteBatch.DrawString(giantRedPixelFont, "1", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)gameStartAlphaFade), 0f, oneOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                        if (soundEffectCounter == 3)
                        {
                            SoundManager.PlaySoundEffect(countdownBeep, (float)gameSettings.SoundEffectVolume / 10, 0f, 0f);
                            soundEffectCounter++;
                        }
                    }
                    else
                    {
                        Vector2 goOrigin = giantRedPixelFont.MeasureString("Go!");
                        spriteBatch.DrawString(giantRedPixelFont, "Go!", new Vector2(1280 / 2, 720 / 2), new Color(255, 255, 255, (byte)gameStartAlphaFade), 0f, goOrigin / 2, 1.0f, SpriteEffects.None, 1.0f);
                        if (soundEffectCounter == 4)
                        {
                            SoundManager.PlaySoundEffect(countdownGoSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0f, 0f);
                            soundEffectCounter++;
                        }
                    }

                    spriteBatch.End();
                    break;
                case GameStates.Playing:
                    DrawGameWorld(gameTime);
                    break;
                case GameStates.Paused:
                    DrawGameWorld(gameTime);
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
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
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
            starField.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            basketballSparkle.Draw(spriteBatch);
            spriteBatch.End();

            //draw objects which contain a body that can have forces applied to it
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
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
                Vector2 streakCenter = streakFont.MeasureString(goalManager.DrawStreakMessage) / 2;
                spriteBatch.DrawString(streakFont, goalManager.DrawStreakMessage, new Vector2(1280 / 2, 696), Color.White, 0f, streakCenter, 1.0f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
        }

        private void DrawGameEnd()
        {
            //Draw a full black background.  This helps with rendering the stars in front of it as the cleared viewport renders 3d space and we force it into 2d space with this.
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black); //background
            starField.Draw(spriteBatch); //stars!
            GameInterface.DrawGameEndInterface(spriteBatch, pixel, pixelGlowFont, goalManager);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());

            spriteBatch.DrawString(pixel, highScoresPlayers, new Vector2(10, 74), Color.White);
            spriteBatch.DrawString(pixel, highScoresStreak, new Vector2(170, 74), Color.White);
            spriteBatch.DrawString(pixel, highScoresScore, new Vector2(340, 74), Color.White);
            spriteBatch.DrawString(pixel, highScoresMultiplier, new Vector2(480, 74), Color.White);

            spriteBatch.End();
        }

        #region Basketball Position
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
        #endregion

        #region PlayerInput
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
                    SoundManager.PlaySoundEffect(basketBallShotSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
                }
            }
        }

        private void HandleShotAngle(MouseState state)
        {
            Vector2 basketballLocation = new Vector2(basketballManager.BasketballBody.Position.X * PhysicalWorld.MetersInPixels,
                                                     basketballManager.BasketballBody.Position.Y * PhysicalWorld.MetersInPixels);
            Vector2 mouseLocation =
                Vector2.Transform(
                    new Vector2(state.X, state.Y) -
                    new Vector2(ResolutionManager.GetViewportX, ResolutionManager.GetViewportY),
                    Matrix.Invert(ResolutionManager.GetTransformationMatrix()));

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
                    gameState = GameStates.TitleScreen;
                }
                if (character == 122)
                {
                    ResolutionManager.SetResolution(1280, 720, false);
                }
                if (character == 120)
                {
                    ResolutionManager.SetResolution(1920, 1080, true);
                }
                if (character == 27)
                {
                    Exit();
                }
            }
            else if (gameState == GameStates.TitleScreen)
            {
                if (character == 13)
                {
                    if (titleScreenSelection == 0)
                    {
                        MediaPlayer.Stop();
                        gameState = GameStates.OptionsScreen;
                    }
                    else
                    {
                        currentSettingSelection = 0;
                        gameState = GameStates.SettingsScreen;
                    }
                }
                if (character == 27)
                {
                    Exit();
                }
            }
            else if (gameState == GameStates.SettingsScreen)
            {
                if (character == 13)
                {
                    if (currentSettingSelection == 4)
                    {
                        if (displayModes[currentResolution] == previousDisplayMode && fullScreenSetting == previousFullScreenSetting && previousMusicSetting == musicVolumeSetting && previousSoundEffectSetting == soundEffectVolumeSetting)
                        {
                            displaySettingsSavedMessage = true;
                        }
                        else
                        {
                            DisplayMode mode = displayModes[currentResolution];
                            gameSettings = new GameSettings(mode.Width, mode.Height, fullScreenSetting, musicVolumeSetting, soundEffectVolumeSetting);

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            string json = serializer.Serialize(gameSettings);
                            using (FileStream fileStream = File.Create(fullSettingsPath))
                            {
                                using(StreamWriter streamWriter = new StreamWriter(fileStream))
                                {
                                    streamWriter.Write(json);
                                }
                            }

                            ResolutionManager.SetResolution(mode.Width, mode.Height, fullScreenSetting);

                            displaySettingsSavedMessage = true;
                            previousDisplayMode = mode;
                            previousFullScreenSetting = fullScreenSetting;
                            previousMusicSetting = musicVolumeSetting;
                            previousSoundEffectSetting = soundEffectVolumeSetting;
                        }
                    }
                    if (currentSettingSelection == 5)
                    {
                        titleScreenSelection = 0;
                        gameState = GameStates.TitleScreen;
                    }
                }
                if (character == 27)
                {
                    gameState = GameStates.TitleScreen;
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
                    else if (highScoreManager.LockedBasketballSelection)
                    {
                    }
                    else
                    {
                        playerOptions.PlayerName = playerName.ToString();
                        camera.Limits = new Rectangle(0, 0, 1280, 720);
                        camera.ResetCamera();
                        gameState = GameStates.GetReadyState;
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

                if (character == 113)
                {
                    Exit();
                }

                if (character == 114)
                {
                    ResetPosition();
                    goalManager.ResetGoalManager();
                    highScoresLoaded = false;
                    SoundManager.MuteSounds();
                    soundEffectCounter = 1;
                    gameState = GameStates.GetReadyState;
                    GameTimer.ResetTimer();
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
                    soundEffectCounter = 1;
                    gameState = GameStates.OptionsScreen;
                    GameTimer.ResetTimer();
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
        #endregion

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
            SoundManager.PlaySoundEffect(collisionSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }

        public bool LeftRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            leftRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }

        public bool RightRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            rightRimCollisionHappened = true;
            goalManager.RimHit = true;
            SoundManager.PlaySoundEffect(collisionSoundEffect, (float)gameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }
        #endregion
    }
}
