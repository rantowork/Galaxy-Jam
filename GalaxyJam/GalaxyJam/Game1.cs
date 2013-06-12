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
            StartScreen,
            TitleScreen,
            SettingsScreen,
            PracticeScreen,
            TutorialScreen,
            OptionsScreen,
            GetReadyState,
            Playing,
            GameEnd,
            Paused
        } ;
        private GameStates gameState = GameStates.StartScreen;

        //Textures
        private Texture2D lineSprite;
        private Texture2D backboard1;
        private Texture2D backboard1Glow;
        private Texture2D backboard2;
        private Texture2D backboard2Glow;
        private Texture2D backboard3;
        private Texture2D backboard3Glow;
        private Texture2D backboard4;
        private Texture2D backboard4Glow;
        private Texture2D backboard5;
        private Texture2D backboard5Glow;
        private Texture2D leftRim1;
        private Texture2D leftRim1Glow;
        private Texture2D rightRim1;
        private Texture2D rightRim1Glow;
        private Texture2D leftRim2;
        private Texture2D leftRim2Glow;
        private Texture2D rightRim2;
        private Texture2D rightRim2Glow;
        private Texture2D leftRim3;
        private Texture2D leftRim3Glow;
        private Texture2D rightRim3;
        private Texture2D rightRim3Glow;
        private Texture2D leftRim4;
        private Texture2D leftRim4Glow;
        private Texture2D rightRim4;
        private Texture2D rightRim4Glow;
        private Texture2D leftRim5;
        private Texture2D leftRim5Glow;
        private Texture2D rightRim5;
        private Texture2D rightRim5Glow;
        private Texture2D galaxyJamLogo;
        private Texture2D twopxsolidstar;
        private Texture2D fourpxblurstar;
        private Texture2D onepxsolidstar;
        private Texture2D cursor;
        private Texture2D galaxyJamText;
        private Texture2D optionsScreenUi;
        private Texture2D upArrowKey;
        private Texture2D downArrowKey;
        private Texture2D rightArrowKey;
        private Texture2D leftArrowKey;

        //Sounds
        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;

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
        private SoundEffect streakWub;
        private SoundEffect laserBoom;
        private Song ambientSpaceSong;
        private SoundManager soundManager;

        //Particles
        private SparkleEmitter basketballSparkle;

        //Game goals
        private GoalManager goalManager = new GoalManager(100, true, new Rectangle(85, 208, 76, 4));
        
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
        
        //collisions get me outta here!
        private const double GLOWTIME = 200;
        private bool backboardCollisionHappened;
        private double backboardGlowTimer;

        private bool leftRimCollisionHappened;
        private double leftrimGlowTimer;

        private bool rightRimCollisionHappened;
        private double rightrimGlowTimer;

        //Settings
        private short titleScreenSelection;
        private const string SETTINGS_FILENAME = "game.settings";
        private string fullSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILENAME);
        private GameSettings gameSettings;
        private List<DisplayMode> displayModes = new List<DisplayMode>();
        private int currentResolution;
        private int currentSettingSelection;
        private DisplayMode defaultDisplayMode;
        private int fullScreenSetting;
        private int musicVolumeSetting;
        private int soundEffectVolumeSetting;

        private DisplayMode previousDisplayMode;
        private int previousFullScreenSetting;
        private int previousMusicSetting;
        private int previousSoundEffectSetting;
        private bool displaySettingsSavedMessage;
        private const double SAVE_TIME = 2000;
        private double displaySaveMessageTimer;

        //Tutorial
        private short currentTutorialScreen;

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
            highScoreManager.CanChangeBasketballSelection = true;
            
            if (!File.Exists(highScoreManager.HighScoreFilePath))
            {
                List<HighScore> tempList = new List<HighScore> { new HighScore("Tim Randall", 2000, 1, 5), new HighScore("Dan Randall", 1000, 1, 5) };

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
                gameSettings = new GameSettings(defaultDisplayMode.Width, defaultDisplayMode.Height, 0, 10, 10);
                
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
                fullScreenSetting = 0;
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
                    gameSettings = new GameSettings(1280, 720, 0, 10, 10);
                }
                else
                {
                    DisplayMode currentMode = GetDisplayMode(gameSettings.DisplayModeWidth,gameSettings.DisplayModeHeight);
                    currentResolution = displayModes.IndexOf(currentMode);
                    fullScreenSetting = gameSettings.FullScreenOption;
                    musicVolumeSetting = gameSettings.MusicVolume;
                    soundEffectVolumeSetting = gameSettings.SoundEffectVolume;
                }
            }
            
            bool fullScreen;
            if (gameSettings.FullScreenOption == 0 || gameSettings.FullScreenOption == 2)
            {
                fullScreen = false;
            }
            else
            {
                fullScreen = true;
            }

            ResolutionManager.SetResolution(gameSettings.DisplayModeWidth, gameSettings.DisplayModeHeight, fullScreen);
            
            if (gameSettings.FullScreenOption == 2)
            {
                MakeGameBorderless();
            }
            else
            {
                MakeGameWindowed();
            }

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
            //Title Screen
            galaxyJamLogo = Content.Load<Texture2D>(@"Textures/GalaxyJamConcept");
            galaxyJamText = Content.Load<Texture2D>(@"Textures/GalaxyJamText");
            optionsScreenUi = Content.Load<Texture2D>(@"Textures/Interface/OptionsUI");
            upArrowKey = Content.Load<Texture2D>(@"Textures/Interface/UpArrowKey");
            downArrowKey = Content.Load<Texture2D>(@"Textures/Interface/DownArrowKey");
            rightArrowKey = Content.Load<Texture2D>(@"Textures/Interface/RightArrowKey");
            leftArrowKey = Content.Load<Texture2D>(@"Textures/Interface/LeftArrowKey");

            //Backboards
            backboard1 = Content.Load<Texture2D>(@"Textures/Backboard/RedOrangeBoard2");
            backboard1Glow = Content.Load<Texture2D>(@"Textures/Backboard/RedOrangeBoardContact2");
            backboard2 = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumBackboard2");
            backboard2Glow = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumBackboardContact2");
            backboard3 = Content.Load<Texture2D>(@"Textures/Backboard/TealLimeGreen2");
            backboard3Glow = Content.Load<Texture2D>(@"Textures/Backboard/TealLimeGreenContact2");
            backboard4 = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRed2");
            backboard4Glow = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedContact2");
            backboard5 = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletBackBoard2");
            backboard5Glow = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContact2");
            leftRim1 = Content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrange");
            leftRim1Glow = Content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeContact");
            rightRim1 = Content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeFlip");
            rightRim1Glow = Content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeContactFlip");
            leftRim2 = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumRegular");
            leftRim2Glow = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumRegularContact");
            rightRim2 = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumFlip");
            rightRim2Glow = Content.Load<Texture2D>(@"Textures/Backboard/PurplePlumContactFlip");
            leftRim3 = Content.Load<Texture2D>(@"Textures/Backboard/GreenLime");
            leftRim3Glow = Content.Load<Texture2D>(@"Textures/Backboard/GreenLimeConract");
            rightRim3 = Content.Load<Texture2D>(@"Textures/Backboard/GreenLimeFlip");
            rightRim3Glow = Content.Load<Texture2D>(@"Textures/Backboard/GreenLimeConractFlip");
            leftRim4 = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRed3");
            leftRim4Glow = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedContact3");
            rightRim4 = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedFlip3");
            rightRim4Glow = Content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedFlipContact3");
            leftRim5 = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueViolet");
            leftRim5Glow = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContact");
            rightRim5 = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletFlip");
            rightRim5Glow = Content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContactFlip");

            //Empty Sprite 1px
            lineSprite = Content.Load<Texture2D>(@"Textures/LineSprite");

            //Stars
            twopxsolidstar = Content.Load<Texture2D>(@"Textures/2x2SolidStar");
            fourpxblurstar = Content.Load<Texture2D>(@"Textures/4x4BlurStar");
            onepxsolidstar = Content.Load<Texture2D>(@"Textures/1x1SolidStar");

            //Keyboard Curso
            cursor = Content.Load<Texture2D>(@"Textures/Cursor");
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
            basketBallShotSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/pulse");
            basketScoredSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/BasketScored");
            collisionSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/Thud");
            countdownBeep = Content.Load<SoundEffect>(@"Audio/SoundEffects/Countdown");
            countdownGoSoundEffect = Content.Load<SoundEffect>(@"Audio/SoundEffects/Go");
            streakWub = Content.Load<SoundEffect>(@"Audio/SoundEffects/wub");
            laserBoom = Content.Load<SoundEffect>(@"Audio/SoundEffects/explosion2");
            ambientSpaceSong = Content.Load<Song>(@"Audio/Music/IntroAmbientCreativeZero");
            MediaPlayer.Play(ambientSpaceSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)gameSettings.MusicVolume/10;
            AudioCategory category = audioEngine.GetCategory("Music");
            category.SetVolume((float)gameSettings.MusicVolume/10);
            soundManager.SelectMusic(SongTypes.SpaceLoop1);
        }

        private void LoadEffectsAndParticles()
        {
            //Load Starfield
            List<Texture2D> starTextures = new List<Texture2D> { twopxsolidstar, fourpxblurstar, onepxsolidstar };
            starField = new Starfield(1280, 720, 1000, starTextures);
            starField.StarSpeedModifier = 1;
            basketballSparkle = new SparkleEmitter(new List<Texture2D> { twopxsolidstar }, new Vector2(-40, -40));
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
            starField.Update(gameTime);
            switch (gameState)
            {
                case GameStates.StartScreen:
                    
                    break;
                case GameStates.TitleScreen:
                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Down))
                    {
                        if (titleScreenSelection >= 0 && titleScreenSelection < 4)
                        {
                            titleScreenSelection++;
                        }
                        else if (titleScreenSelection == 4)
                        {
                            titleScreenSelection = 0;
                        }
                    }
                    else if(input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Up))
                    {
                        if (titleScreenSelection > 0 && titleScreenSelection <= 4)
                        {
                            titleScreenSelection--;
                        }
                        else if (titleScreenSelection == 0)
                        {
                            titleScreenSelection = 4;
                        }
                    }
                    cachedUpDownKeyboardState = input.GetKeyboard().GetState();

                    break;
                case GameStates.SettingsScreen:

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
                            if (fullScreenSetting > 0)
                            {
                                fullScreenSetting--;
                            }
                        }
                        else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Right) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Right))
                        {
                            if (fullScreenSetting < 2)
                            {
                                fullScreenSetting++;
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

                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Up) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Up) && highScoreManager.CanChangeBasketballSelection)
                    {
                        if (currentlySelectedBasketballKey > 0)
                        {
                            currentlySelectedBasketballKey--;
                        }
                    }
                    else if (input.GetKeyboard().GetState().IsKeyDown(Keys.Down) && !cachedUpDownKeyboardState.IsKeyDown(Keys.Down) && highScoreManager.CanChangeBasketballSelection)
                    {
                        if (currentlySelectedBasketballKey < BasketballManager.basketballs.Count - 1)
                        {
                            currentlySelectedBasketballKey++;
                        }
                    }
                    cachedUpDownKeyboardState = input.GetKeyboard().GetState();
                    
                    if (playerName.Length >= 3)
                    {
                        nameToShort = false;
                    }

                    break;

                case GameStates.GetReadyState:
                    starField.StarSpeedModifier = 1;

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
                case GameStates.TutorialScreen:
                    if (input.GetKeyboard().GetState().IsKeyDown(Keys.Enter) && !cachedRightLeftKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        if (currentTutorialScreen < 4)
                        {
                            currentTutorialScreen++;
                        }
                    }
                    cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                    if (currentTutorialScreen == 0)
                    {
                        BasketballManager.basketballs[0].Update(gameTime);
                        PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                        HandlePlayerInput();
                        HandleBasketballPosition();

                        basketballSparkle.EmitterLocation = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                        basketballSparkle.Update();
                    }
                    break;
                case GameStates.PracticeScreen:
                    BasketballManager.basketballs[0].Update(gameTime);
                    PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                    HandlePlayerInput();
                    HandleBasketballPosition();

                    basketballSparkle.EmitterLocation = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                    basketballSparkle.Update();

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
                    break;
                case GameStates.Playing:
                    BasketballManager.SelectedBasketball.Update(gameTime);
                    PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
                    
                    HandlePlayerInput();
                    HandleBasketballPosition();

                    basketballSparkle.EmitterLocation = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                    basketballSparkle.Update();

                    Vector2 basketballCenter = basketballManager.BasketballBody.WorldCenter * PhysicalWorld.MetersInPixels;
                    Rectangle basketballCenterRectangle = new Rectangle((int)basketballCenter.X - 8, (int)basketballCenter.Y - 8, 16, 16);
                    goalManager.UpdateGoalScored(gameTime, camera, basketballCenterRectangle, basketScoredSoundEffect, streakWub, laserBoom, basketballSparkle, starField, gameSettings);

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
                            basketballSparkle.CleanUpParticles();
                            gameState = GameStates.GameEnd;
                        }
                    }

                    break;

                case GameStates.Paused:

                    break;
                case GameStates.GameEnd:
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            spriteBatch.Draw(lineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
            starField.Draw(spriteBatch);
            spriteBatch.End();
            switch (gameState)
            {
                case GameStates.StartScreen:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    GameInterface.DrawTitleScreen(spriteBatch, galaxyJamLogo);
                    spriteBatch.End();
                    break;
                case GameStates.TitleScreen:
                    const string playText = "Play";
                    const string practiceText = "Practice";
                    const string settingsText = "Settings";
                    const string tutorialText = "How to Play";
                    const string exitText = "Exit";
                    const string tickerSymbol = ">";
                    Vector2 tickerOrigin = pixel.MeasureString(tickerSymbol)/2;
                    Vector2 playTextOrigin = pixel.MeasureString(playText) / 2;
                    Vector2 practiceOrigin = pixel.MeasureString(practiceText) / 2;
                    Vector2 settingsTextOrigin = pixel.MeasureString(settingsText) / 2;
                    Vector2 tutorialTextOrigin = pixel.MeasureString(tutorialText) / 2;
                    Vector2 exitOrigin = pixel.MeasureString(exitText)/2;
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    spriteBatch.DrawString(pixel, playText, new Vector2(1280/2, 290), Color.White, 0f, playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, practiceText, new Vector2(1280 / 2, 320), Color.White, 0f, practiceOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, settingsText, new Vector2(1280 / 2, 350), Color.White, 0f, settingsTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, tutorialText, new Vector2(1280/2, 380), Color.White, 0f, tutorialTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.DrawString(pixel, exitText, new Vector2(1280 / 2, 410), Color.White, 0f, exitOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    if (titleScreenSelection == 0)
                    {
                        spriteBatch.DrawString(pixel, tickerSymbol, new Vector2(1280/2 - 60, 290), Color.White, 0f, tickerOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    else if (titleScreenSelection == 1)
                    {
                        spriteBatch.DrawString(pixel, tickerSymbol, new Vector2(1280 / 2 - 90, 320), Color.White, 0f, tickerOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    else if (titleScreenSelection == 2)
                    {
                        spriteBatch.DrawString(pixel, tickerSymbol, new Vector2(1280 / 2 - 90, 350), Color.White, 0f, tickerOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    else if (titleScreenSelection == 3)
                    {
                        spriteBatch.DrawString(pixel, tickerSymbol, new Vector2(1280 / 2 - 120, 380), Color.White, 0f, tickerOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    else
                    {
                        spriteBatch.DrawString(pixel, tickerSymbol, new Vector2(1280 / 2 - 60, 410), Color.White, 0f, tickerOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }
                    spriteBatch.End();
                    break;
                case GameStates.PracticeScreen:
                    const string escapePractice = "(Esc) Exit";
                    const string practiceModeText = "Practice Mode";

                    Vector2 practiceModeOrigin = pixel.MeasureString(practiceModeText)/2;

                    Vector2 backboardPosition = backboardBody.Position * PhysicalWorld.MetersInPixels;
                    Vector2 backboardOrigin = new Vector2(backboard1.Width / 2f, backboard1.Height / 2f);
                    
                    Vector2 leftRimOrigin = new Vector2(leftRim1.Width, leftRim1.Height)/2;
                    Vector2 rightRimOrigin2 = new Vector2(rightRim1.Width, rightRim1.Height)/2;

                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    spriteBatch.DrawString(pixel, escapePractice, new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(pixel, practiceModeText, new Vector2(1280/2, 18), Color.White, 0f, practiceModeOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    basketballSparkle.Draw(spriteBatch);
                    spriteBatch.Draw(BasketballManager.basketballs[0].BasketballTexture, (basketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.basketballs[0].Source, Color.White, basketballManager.BasketballBody.Rotation, BasketballManager.basketballs[0].Origin, 1f, SpriteEffects.None, 0f);
                    
                    //draw backboard
                    spriteBatch.Draw(backboardCollisionHappened ? backboard1Glow : backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                    //draw left rim
                    spriteBatch.Draw(leftRimCollisionHappened ? leftRim1Glow : leftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    //draw right rim
                    spriteBatch.Draw(rightRimCollisionHappened ? rightRim1Glow : rightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
                    spriteBatch.End();

                    break;
                case GameStates.TutorialScreen:
                    const string escapeTutorial = "(Esc) Exit";
                    string enterContinue = "(Enter) Next";
                    if (currentTutorialScreen == 4)
                    {
                        enterContinue = "";
                    }
                    
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    spriteBatch.DrawString(pixel, escapeTutorial, new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(pixel, enterContinue, new Vector2(1080, 10), Color.White);
                    spriteBatch.End();

                    if (currentTutorialScreen == 0)
                    {
                        const string tutText01 = "Click to shoot. Try it out!";
                        Vector2 tutText1Origin = pixel.MeasureString(tutText01)/2;
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.DrawString(pixel, tutText01, new Vector2(1280/2, 700), Color.White, 0f, tutText1Origin, 1.0f, SpriteEffects.None, 1.0f);
                        basketballSparkle.Draw(spriteBatch);
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.Draw(BasketballManager.basketballs[0].BasketballTexture, (basketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.basketballs[0].Source, Color.White, basketballManager.BasketballBody.Rotation, BasketballManager.basketballs[0].Origin, 1f, SpriteEffects.None, 0f);
                        spriteBatch.End();
                    }
                    else if (currentTutorialScreen == 1)
                    {
                        const string tutText02 = "Game timer.  You get only 2 minutes!";
                        const string tutText02Timer = "2:00";
                        Vector2 tutText02Origin = pixel.MeasureString(tutText02)/2;
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.DrawString(pixel, tutText02, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText02Origin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.DrawString(pixelGlowFont, tutText02Timer, new Vector2(10, 664), Color.White);
                        spriteBatch.End();
                    }
                    else if (currentTutorialScreen == 2)
                    {
                        const string tutText03 = "Your score.  Earn new basketballs with a high score.";
                        const string tutText03Score = "100000";
                        Vector2 tutText03Origin = pixel.MeasureString(tutText03)/2;
                        Vector2 tutText03ScoreOrigin = pixelGlowFont.MeasureString(tutText03Score) / 2;
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.DrawString(pixel, tutText03, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText03Origin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.DrawString(pixelGlowFont, tutText03Score, new Vector2(1280 / 2, 30), Color.White, 0f, tutText03ScoreOrigin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.End();
                    }
                    else if (currentTutorialScreen == 3)
                    {
                        const string tutText04 = "Current streak.  A higher streak means more points.";
                        const string tutText04Streak = "+9";
                        Vector2 tutText04Origin = pixel.MeasureString(tutText04) / 2;
                        Vector2 tutText04StreakOrigin = pixelGlowFont.MeasureString(tutText04Streak);
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.DrawString(pixel, tutText04, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText04Origin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.DrawString(pixelGlowFont, tutText04Streak, new Vector2(1260, 100), Color.White, 0f, tutText04StreakOrigin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.End();
                    }
                    else if (currentTutorialScreen == 4)
                    {
                        const string tutText05 = "Score multiplier.  Big multiplier, big points.";
                        const string tutText05Mult = "x42";
                        Vector2 tutText05Origin = pixel.MeasureString(tutText05) / 2;
                        Vector2 tutText05MultOrigin = pixelGlowFont.MeasureString(tutText05Mult);
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                        spriteBatch.DrawString(pixel, tutText05, new Vector2(1280 / 2, 720 / 2), Color.White, 0f, tutText05Origin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.DrawString(pixelGlowFont, tutText05Mult, new Vector2(1260, 720), Color.White, 0f, tutText05MultOrigin, 1.0f, SpriteEffects.None, 1.0f);
                        spriteBatch.End();
                    }

                    break;
                case GameStates.SettingsScreen:
                    DisplayMode selectedMode = displayModes[currentResolution];
                    string resolutionText = String.Format("{0} x {1}", selectedMode.Width, selectedMode.Height);
                    string fullScreenText;
                    switch (fullScreenSetting)
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
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
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

                    if (displaySettingsSavedMessage)
                    {
                        const string savedText = "Settings Saved";
                        Vector2 savedTextOrigin = pixel.MeasureString(savedText)/2;
                        spriteBatch.DrawString(pixel, savedText, new Vector2(1280/2, 680), Color.Red, 0f, savedTextOrigin, 1.0f, SpriteEffects.None, 1.0f);
                    }

                    spriteBatch.End();
                    break;
                case GameStates.OptionsScreen:
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    spriteBatch.Draw(optionsScreenUi, new Vector2(726, 0), Color.White);
                    GetPlayerName(gameTime);
                    spriteBatch.Draw(galaxyJamText, new Vector2(0, 10), Color.White);
                    GameInterface.DrawOptionsInterface(spriteBatch, gameTime, pixel, highScoreManager, nameToShort, currentlySelectedBasketballKey, currentlySelectedSongKey);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
                    if (currentlySelectedBasketballKey == 0)
                    {
                        spriteBatch.Draw(downArrowKey, new Vector2(880, 214), Color.White);
                    }
                    else if (currentlySelectedBasketballKey > 0 && currentlySelectedBasketballKey < (BasketballManager.basketballSelection.Count - 1))
                    {
                        spriteBatch.Draw(downArrowKey, new Vector2(880, 214), Color.White);
                        spriteBatch.Draw(upArrowKey, new Vector2(1102, 214), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(upArrowKey, new Vector2(1102, 214), Color.White);
                    }
                    if (currentlySelectedSongKey == 0)
                    {
                        spriteBatch.Draw(rightArrowKey, new Vector2(1102, 560), Color.White);
                    }
                    else if (currentlySelectedSongKey > 0 && currentlySelectedSongKey < (SoundManager.music.Count - 1))
                    {
                        spriteBatch.Draw(rightArrowKey, new Vector2(1102, 560), Color.White);
                        spriteBatch.Draw(leftArrowKey, new Vector2(880, 560), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(leftArrowKey, new Vector2(880, 560), Color.White);
                    }
                    spriteBatch.End();
                    break;
                case GameStates.GetReadyState:
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
                    //DrawGameWorld(gameTime);
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

        private void LoadPhysicalWorldEntities()
        {
            backboardBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(64f / PhysicalWorld.MetersInPixels, 116f / PhysicalWorld.MetersInPixels), 140f, 6f, 1f, .3f, .1f);
            backboardBody.OnCollision += BackboardCollision;

            leftRimBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(80f / PhysicalWorld.MetersInPixels, 206 / PhysicalWorld.MetersInPixels), 16f, 10f, 1f, .3f, .1f);
            leftRimBody.OnCollision += LeftRimCollision;

            rightRimBody = PhysicalWorld.CreateStaticRectangleBody(new Vector2(188 / PhysicalWorld.MetersInPixels, 206 / PhysicalWorld.MetersInPixels), 16f, 54f, 1f, .3f, .1f);
            rightRimBody.OnCollision += RightRimCollision;
        }

        private const double EFFECT_TIME = 1500;
        private double effectTimer;
        private void DrawGameWorld(GameTime gameTime)
        {
            Vector2 backboardPosition = backboardBody.Position * PhysicalWorld.MetersInPixels;
            Vector2 backboardOrigin = new Vector2(backboard1.Width, backboard1.Height)/2;

            Vector2 leftRimOrigin = new Vector2(leftRim1.Width, leftRim1.Height) / 2;
            Vector2 rightRimOrigin2 = new Vector2(rightRim1.Width, rightRim1.Height) / 2;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            basketballSparkle.Draw(spriteBatch);
            spriteBatch.End();

            //draw objects which contain a body that can have forces applied to it
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            //draw basketball
            spriteBatch.Draw(BasketballManager.SelectedBasketball.BasketballTexture, (basketballManager.BasketballBody.Position * PhysicalWorld.MetersInPixels), BasketballManager.SelectedBasketball.Source, Color.White, basketballManager.BasketballBody.Rotation, BasketballManager.SelectedBasketball.Origin, 1f, SpriteEffects.None, 0f);
            //draw backboard
            if (goalManager.Streak < 3)
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard1Glow : backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim1Glow : leftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim1Glow : rightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (goalManager.Streak >= 3 && goalManager.Streak < 6)
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard2Glow : backboard2, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim2Glow : leftRim2, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim2Glow : rightRim2, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (goalManager.Streak >= 6 && goalManager.Streak < 9)
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard3Glow : backboard3, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim3Glow : leftRim3, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim3Glow : rightRim3, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (goalManager.Streak >= 9 && goalManager.Streak < 15)
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard4Glow : backboard4, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim4Glow : leftRim4, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim4Glow : rightRim4, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else if (goalManager.Streak >= 15)
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard5Glow : backboard5, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim5Glow : leftRim5, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim5Glow : rightRim5, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }
            else
            {
                spriteBatch.Draw(backboardCollisionHappened ? backboard1Glow : backboard1, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(leftRimCollisionHappened ? leftRim1Glow : leftRim1, new Vector2(57, 208), null, Color.White, 0f, leftRimOrigin, 1.0f, SpriteEffects.None, 1.0f);
                spriteBatch.Draw(rightRimCollisionHappened ? rightRim1Glow : rightRim1, new Vector2(188, 208), null, Color.White, 0f, rightRimOrigin2, 1.0f, SpriteEffects.None, 1.0f);
            }

            GameInterface.DrawPlayingInterface(spriteBatch, pixel, pixelGlowFont, goalManager);

            if (goalManager.DrawSwish)
            {
                effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                const string swishText = "SWISH!";
                Vector2 swishOrigin = pixelGlowFont.MeasureString(swishText)/2;
                spriteBatch.DrawString(pixelGlowFont, swishText, new Vector2(1280/2, 70), Color.White, 0f, swishOrigin,1.0f, SpriteEffects.None, 1.0f);
                if (EFFECT_TIME < effectTimer)
                {
                    effectTimer = 0;
                    goalManager.DrawSwish = false;
                }
            }

            if (goalManager.DrawCleanShot)
            {
                effectTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
                const string niceShotText = "Nice Shot!";
                Vector2 niceShotOrigin = pixelGlowFont.MeasureString(niceShotText)/2;
                spriteBatch.DrawString(pixelGlowFont, niceShotText, new Vector2(1280/2, 70), Color.White, 0f, niceShotOrigin, 1.0f, SpriteEffects.None, 1.0f);
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
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
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
            return new Vector2((rand.Next(400, 1200)) / PhysicalWorld.MetersInPixels, (rand.Next(310, 650)) / PhysicalWorld.MetersInPixels);
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
            basketballManager.BasketballBody.ApplyAngularImpulse(.2f);
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
                        basketballManager.BasketballBody.RestoreCollisionWith(backboardBody);
                        basketballManager.BasketballBody.RestoreCollisionWith(leftRimBody);
                        basketballManager.BasketballBody.RestoreCollisionWith(rightRimBody);
                        MediaPlayer.Stop();
                        playerName.Clear();
                        nameToShort = false;
                        SoundManager.SelectedMusic.Resume();
                        gameState = GameStates.OptionsScreen;
                    }
                    else if (titleScreenSelection == 1)
                    {
                        basketballManager.BasketballBody.RestoreCollisionWith(backboardBody);
                        basketballManager.BasketballBody.RestoreCollisionWith(leftRimBody);
                        basketballManager.BasketballBody.RestoreCollisionWith(rightRimBody);
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                        gameState = GameStates.PracticeScreen;
                    }
                    else if (titleScreenSelection == 2)
                    {
                        currentSettingSelection = 0;
                        gameState = GameStates.SettingsScreen;
                    }
                    else if (titleScreenSelection == 3)
                    {
                        basketballManager.BasketballBody.IgnoreCollisionWith(backboardBody);
                        basketballManager.BasketballBody.IgnoreCollisionWith(leftRimBody);
                        basketballManager.BasketballBody.IgnoreCollisionWith(rightRimBody);
                        currentTutorialScreen = 0;
                        cachedRightLeftKeyboardState = input.GetKeyboard().GetState();
                        gameState = GameStates.TutorialScreen;
                    }
                    else
                    {
                        Exit();
                    }
                }
            }
            else if (gameState == GameStates.TutorialScreen)
            {
                if (character == 27)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
                    gameState = GameStates.TitleScreen;
                }
            }
            else if (gameState == GameStates.PracticeScreen)
            {
                if (character == 27)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
                    gameState = GameStates.TitleScreen;
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
                            
                            bool fullScreen;
                            if (gameSettings.FullScreenOption == 0 || gameSettings.FullScreenOption == 2)
                            {
                                fullScreen = false;
                            }
                            else
                            {
                                fullScreen = true;
                            }
                            
                            ResolutionManager.SetResolution(mode.Width, mode.Height, fullScreen);
                            
                            if (gameSettings.FullScreenOption == 2)
                            {
                                MakeGameBorderless();
                            }
                            else
                            {
                                MakeGameWindowed();
                            }

                            AudioCategory category = audioEngine.GetCategory("Music");
                            category.SetVolume(gameSettings.MusicVolume / 10f);

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
                        if (character != 13 && character != '\t')
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
                    SoundManager.SelectedMusic.Pause();
                    MediaPlayer.Resume();
                    gameState = GameStates.TitleScreen;
                    //Exit();
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

                if (character == 109)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
                    goalManager.ResetGoalManager();
                    highScoresLoaded = false;
                    soundEffectCounter = 1;
                    SoundManager.MuteSounds();
                    SoundManager.SelectedMusic.Resume();
                    gameState = GameStates.OptionsScreen;
                    GameTimer.ResetTimer();
                }

                if (character == 114)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
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
                if (character == 113)
                {
                    Exit();
                }

                if (character == 109)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
                    goalManager.ResetGoalManager();
                    highScoresLoaded = false;
                    soundEffectCounter = 1;
                    SoundManager.SelectedMusic.Resume();
                    gameState = GameStates.OptionsScreen;
                    GameTimer.ResetTimer();
                }

                if (character == 114)
                {
                    ResetPosition();
                    basketballSparkle.CleanUpParticles();
                    goalManager.ResetGoalManager();
                    highScoresLoaded = false;
                    soundEffectCounter = 1;
                    gameState = GameStates.GetReadyState;
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

        private void MakeGameBorderless()
        {
            IntPtr windowHandle = Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(windowHandle);
            var form = control.FindForm();
            if (form != null)
            {
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                form.Width = gameSettings.DisplayModeWidth;
                form.Height = gameSettings.DisplayModeHeight;
            }
        }

        private void MakeGameWindowed()
        {
            IntPtr windowHandle = Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(windowHandle);
            var form = control.FindForm();
            if (form != null)
            {
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                form.WindowState = System.Windows.Forms.FormWindowState.Normal;
                form.Width = gameSettings.DisplayModeWidth;
                form.Height = gameSettings.DisplayModeHeight;
            }
        }
    }
}
