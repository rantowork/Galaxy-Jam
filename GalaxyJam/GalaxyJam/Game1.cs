using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Nuclex.Input;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Effects._3D.Particles;
using SpoidaGamesArcadeLibrary.GameStates;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.Screen;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Screen = SpoidaGamesArcadeLibrary.Globals.Screen;

namespace GalaxyJam
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        //Camera & Graphics
        readonly GraphicsDeviceManager m_graphics;
        SpriteBatch m_spriteBatch;

        //Sounds
        private AudioEngine m_audioEngine;
        private WaveBank m_waveBank;
        private SoundBank m_soundBank;
        private SoundManager m_soundManager;
        
        //High Scores
        private const string HIGH_SCORES_FILENAME = "highscores.lst";
        private readonly string m_fullHighScorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, HIGH_SCORES_FILENAME);

        private const string ARCADE_HIGH_SCORES_FILENAME = "arcadehighscores.lst";
        private readonly string m_arcadeFullHighScorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ARCADE_HIGH_SCORES_FILENAME);
        
        //Settings
        private const string SETTINGS_FILENAME = "game.settings";
        private readonly string m_fullSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_FILENAME);

        public Game1()
        {
            m_graphics = new GraphicsDeviceManager(this);
            ResolutionManager.Init(ref m_graphics);
            ResolutionManager.SetVirtualResolution(1280, 720);
            ResolutionManager.SetResolution(1280, 720, false);

            foreach (DisplayMode mode in m_graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if (mode.Format == SurfaceFormat.Color)
                {
                    Screen.DisplayModes.Add(mode);
                    if (mode.Width == 1280 && mode.Height == 720)
                    {
                        ComputerSettings.DefaultDisplayMode = mode;
                    }
                }
            }

            Content.RootDirectory = "Content";
            Screen.Input = new InputManager(Services, Window.Handle);
            Components.Add(Screen.Input);

            PhysicalWorld.World = new World(Vector2.Zero);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            GameState.States = GameState.GameStates.StartScreen;
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
            Cursor myCursor = LoadCustomCursor(@"Content\Cursor\crosshair.cur");
            Form winForm = (Form)Control.FromHandle(Window.Handle);
            winForm.Cursor = myCursor;

            Screen.Input.GetKeyboard().CharacterEntered += GamePlayInput;

            InterfaceSettings.HighScoreManager = new HighScoreManager(m_fullHighScorePath) {CanChangeBasketballSelection = true};
            InterfaceSettings.ArcadeHighScoreManager = new ArcadeHighScoreManager(m_arcadeFullHighScorePath) { CanChangeBasketballSelection = true };

            if (!File.Exists(InterfaceSettings.ArcadeHighScoreManager.HighScoreFilePath))
            {
                List<HighScore> tempList = new List<HighScore> { new HighScore("Tim Randall", 2000, 1, 5), new HighScore("Dan Randall", 1000, 1, 5) };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(tempList);
                string encryptedJson = InterfaceSettings.ArcadeHighScoreManager.EncodeHighScores(json);

                using (FileStream fileStream = File.Create(InterfaceSettings.ArcadeHighScoreManager.HighScoreFilePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(encryptedJson);
                    }
                }
                InterfaceSettings.ArcadeHighScoreManager.HighScores = tempList;
            }
            else
            {
                InterfaceSettings.ArcadeHighScoreManager.LoadHighScoresFromDisk();
            }

            if (!File.Exists(InterfaceSettings.HighScoreManager.HighScoreFilePath))
            {
                List<HighScore> tempList = new List<HighScore> { new HighScore("Tim Randall", 2000, 1, 5), new HighScore("Dan Randall", 1000, 1, 5) };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(tempList);
                string encryptedJson = InterfaceSettings.HighScoreManager.EncodeHighScores(json);

                using (FileStream fileStream = File.Create(InterfaceSettings.HighScoreManager.HighScoreFilePath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(encryptedJson);
                    }
                }
                InterfaceSettings.HighScoreManager.HighScores = tempList;
            }
            else
            {
                InterfaceSettings.HighScoreManager.LoadHighScoresFromDisk();
            }

            if (!File.Exists(m_fullSettingsPath))
            {
                InterfaceSettings.GameSettings = new GameSettings(ComputerSettings.DefaultDisplayMode.Width, ComputerSettings.DefaultDisplayMode.Height, 0, 10, 10);
                
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(InterfaceSettings.GameSettings);
                using (FileStream fileStream = File.Create(m_fullSettingsPath))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(json);
                    }
                }

                ComputerSettings.CurrentResolution = Screen.DisplayModes.IndexOf(ComputerSettings.DefaultDisplayMode);
                ComputerSettings.FullScreenSetting = 0;
                ComputerSettings.MusicVolumeSetting = 10;
                ComputerSettings.SoundEffectVolumeSetting = 10;
            }
            else
            {
                using (FileStream fileStream = File.Open(m_fullSettingsPath, FileMode.Open))
                {
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    using (StreamReader streamReader = new StreamReader(fileStream))
                    {
                        string fileData = streamReader.ReadToEnd();
                        InterfaceSettings.GameSettings = javaScriptSerializer.Deserialize<GameSettings>(fileData);
                    }
                }
                if (InterfaceSettings.GameSettings == null)
                {
                    InterfaceSettings.GameSettings = new GameSettings(1280, 720, 0, 10, 10);
                }
                else
                {
                    DisplayMode currentMode = GetDisplayMode(InterfaceSettings.GameSettings.DisplayModeWidth,InterfaceSettings.GameSettings.DisplayModeHeight);
                    ComputerSettings.CurrentResolution = Screen.DisplayModes.IndexOf(currentMode);
                    ComputerSettings.FullScreenSetting = InterfaceSettings.GameSettings.FullScreenOption;
                    ComputerSettings.MusicVolumeSetting = InterfaceSettings.GameSettings.MusicVolume;
                    ComputerSettings.SoundEffectVolumeSetting = InterfaceSettings.GameSettings.SoundEffectVolume;
                }
            }
            
            bool fullScreen;
            if (InterfaceSettings.GameSettings.FullScreenOption == 0 || InterfaceSettings.GameSettings.FullScreenOption == 2)
            {
                fullScreen = false;
            }
            else
            {
                fullScreen = true;
            }

            ResolutionManager.SetResolution(InterfaceSettings.GameSettings.DisplayModeWidth, InterfaceSettings.GameSettings.DisplayModeHeight, fullScreen);
            ResolutionManager.ResetViewport();

            if (InterfaceSettings.GameSettings.FullScreenOption == 2)
            {
                MakeGameBorderless();
            }
            else
            {
                MakeGameWindowed();
            }

            m_audioEngine = new AudioEngine("Content\\Audio\\GalaxyJamAudio.xgs");
            m_waveBank = new WaveBank(m_audioEngine, "Content\\Audio\\Wave Bank.xwb");
            m_soundBank = new SoundBank(m_audioEngine, "Content\\Audio\\Sound Bank.xsb");
            base.Initialize();
        }

        private DisplayMode GetDisplayMode(int width, int height)
        {
            foreach (DisplayMode mode in m_graphics.GraphicsDevice.Adapter.SupportedDisplayModes)
            {
                if (mode.Format == SurfaceFormat.Color)
                {
                    if (mode.Width == width && mode.Height == height)
                    {
                        return mode;
                    }
                }
            }
            return ComputerSettings.DefaultDisplayMode;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Textures.LoadTextures(Content);
            InterfaceSettings.LoadEffects();
            ParticleEmitters.LoadEmitters(Content);
            Fonts.LoadFonts(Content);
            Sounds.LoadSongs(Content, InterfaceSettings.GameSettings);
            Sounds.LoadSounds(Content);
            Screen.Camera = new Camera(GraphicsDevice.Viewport)
            {
                Limits = null
            };

            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            InterfaceSettings.BasketballManager = new BasketballManager(Content);
            m_soundManager = new SoundManager(m_soundBank);

            PhysicalWorld.LoadPhysicalWorldEntities();

            ArcadeGoalManager.LoadPowerUps();

            AudioCategory category = m_audioEngine.GetCategory("Music");
            category.SetVolume((float)InterfaceSettings.GameSettings.MusicVolume / 10);
            m_soundManager.SelectMusic(SongTypes.SpaceLoop1);

            ParticleSystems.TrailParticleSystemWrapper = new TrailParticleSystemWrapper(this);
            ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper = new ExplosionFlyingSparksParticleSystemWrapper(this);
            ParticleSystems.InitializeParticleSystems();
            ParticleSystems.TrailParticleSystemWrapper.AutoInitialize(GraphicsDevice, Content, null);
            ParticleSystems.TrailParticleSystemWrapper.AfterAutoInitialize();
            ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.AutoInitialize(GraphicsDevice, Content, null);
            ParticleSystems.ExplosionFlyingSparksParticleSystemWrapper.AfterAutoInitialize();
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
            PhysicalWorld.World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            InterfaceSettings.StarField.Update(gameTime);
            switch (GameState.States)
            {
                case GameState.GameStates.StartScreen:
                    StartScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.TitleScreen:
                    TitleScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.SettingsScreen:
                    SettingsScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.OptionsScreen:
                    OptionsScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.GetReadyState:
                    GetReadyScreenState.Update(gameTime, GameState.SelectedGameMode);
                    break;
                case GameState.GameStates.TutorialScreen:
                    TutorialScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.PracticeScreen:
                    PracticeScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.Playing:
                    PlayingScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.Paused:
                    PausedScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.GameEnd:
                    GameEndScreenState.Update(gameTime);
                    break;
                case GameState.GameStates.ArcadeMode:
                    ArcadeModeScreenState.Update(gameTime);
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

            m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Screen.Camera.ViewMatrix * ResolutionManager.GetTransformationMatrix());
            m_spriteBatch.Draw(Textures.LineSprite, new Rectangle(0, 0, 1280, 720), Color.Black);
            InterfaceSettings.StarField.Draw(m_spriteBatch);
            m_spriteBatch.End();

            switch (GameState.States)
            {
                case GameState.GameStates.StartScreen:
                    StartScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.TitleScreen:
                    TitleScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.PracticeScreen:
                    PracticeScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.TutorialScreen:
                    TutorialScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.SettingsScreen:
                    SettingsScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.OptionsScreen:
                    OptionsScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.GetReadyState:
                    GetReadyScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.Playing:
                    PlayingScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.Paused:
                    PausedScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.GameEnd:
                    GameEndScreenState.Draw(gameTime, m_spriteBatch);
                    break;
                case GameState.GameStates.ArcadeMode:
                    ArcadeModeScreenState.Draw(gameTime, m_spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }

        #region PlayerInput

        private void GamePlayInput(char character)
        {
            if (GameState.States == GameState.GameStates.StartScreen)
            {
                if (character == 13)
                {
                    GameState.States = GameState.GameStates.TitleScreen;
                }
                if (character == 27)
                {
                    Exit();
                }
            }
            else if (GameState.States == GameState.GameStates.TitleScreen)
            {
                if (character == 13)
                {
                    if (InterfaceSettings.TitleScreenSelection == 0)
                    {
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.BackboardBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.LeftRimBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.RightRimBody);
                        MediaPlayer.Stop();
                        InterfaceSettings.PlayerName.Clear();
                        InterfaceSettings.NameToShort = false;
                        SoundManager.SelectedMusic.Resume();
                        GameState.SelectedGameMode = 0;
                        GameState.States = GameState.GameStates.OptionsScreen;
                    }
                    else if (InterfaceSettings.TitleScreenSelection == 1)
                    {
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.BackboardBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.LeftRimBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.RightRimBody);
                        Screen.CachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
                        MediaPlayer.Stop();
                        InterfaceSettings.PlayerName.Clear();
                        InterfaceSettings.NameToShort = false;
                        SoundManager.SelectedMusic.Resume();
                        //BasketballManager.SelectBasketball(BasketballTypes.CuteInPink);
                        //ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                        GameState.SelectedGameMode = 1;
                        GameState.States = GameState.GameStates.OptionsScreen;
                    }
                    else if (InterfaceSettings.TitleScreenSelection == 2)
                    {
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.BackboardBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.LeftRimBody);
                        InterfaceSettings.BasketballManager.BasketballBody.RestoreCollisionWith(PhysicalWorld.RightRimBody);
                        Screen.CachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
                        GameState.States = GameState.GameStates.PracticeScreen;
                    }
                    else if (InterfaceSettings.TitleScreenSelection == 3)
                    {
                        ComputerSettings.CurrentSettingSelection = 0;
                        GameState.States = GameState.GameStates.SettingsScreen;
                    }
                    else if (InterfaceSettings.TitleScreenSelection == 4)
                    {
                        InterfaceSettings.BasketballManager.BasketballBody.IgnoreCollisionWith(PhysicalWorld.BackboardBody);
                        InterfaceSettings.BasketballManager.BasketballBody.IgnoreCollisionWith(PhysicalWorld.LeftRimBody);
                        InterfaceSettings.BasketballManager.BasketballBody.IgnoreCollisionWith(PhysicalWorld.RightRimBody);
                        InterfaceSettings.CurrentTutorialScreen = 0;
                        Screen.CachedRightLeftKeyboardState = Screen.Input.GetKeyboard().GetState();
                        GameState.States = GameState.GameStates.TutorialScreen;
                    }
                    else
                    {
                        Exit();
                    }
                }
            }
            else if (GameState.States == GameState.GameStates.TutorialScreen)
            {
                if (character == 27)
                {
                    ResetPosition();
                    BasketballManager.Basketballs[0].BallEmitter.CleanUpParticles();
                    GameState.States = GameState.GameStates.TitleScreen;
                }
            }
            else if (GameState.States == GameState.GameStates.PracticeScreen)
            {
                if (character == 27)
                {
                    ResetPosition();
                    BasketballManager.Basketballs[0].BallEmitter.CleanUpParticles();
                    GameState.States = GameState.GameStates.TitleScreen;
                }
            }
            else if (GameState.States == GameState.GameStates.SettingsScreen)
            {
                if (character == 13)
                {
                    if (ComputerSettings.CurrentSettingSelection == 4)
                    {
                        if (Screen.DisplayModes[ComputerSettings.CurrentResolution] == ComputerSettings.PreviousDisplayMode && ComputerSettings.FullScreenSetting == ComputerSettings.PreviousFullScreenSetting && ComputerSettings.PreviousMusicSetting == ComputerSettings.MusicVolumeSetting && ComputerSettings.PreviousSoundEffectSetting == ComputerSettings.SoundEffectVolumeSetting)
                        {
                            InterfaceSettings.DisplaySettingsSavedMessage = true;
                        }
                        else
                        {
                            DisplayMode mode = Screen.DisplayModes[ComputerSettings.CurrentResolution];
                            InterfaceSettings.GameSettings = new GameSettings(mode.Width, mode.Height, ComputerSettings.FullScreenSetting, ComputerSettings.MusicVolumeSetting, ComputerSettings.SoundEffectVolumeSetting);

                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            string json = serializer.Serialize(InterfaceSettings.GameSettings);
                            
                            using (FileStream fileStream = File.Create(m_fullSettingsPath))
                            {
                                using(StreamWriter streamWriter = new StreamWriter(fileStream))
                                {
                                    streamWriter.Write(json);
                                }
                            }
                            
                            bool fullScreen;
                            if (InterfaceSettings.GameSettings.FullScreenOption == 0 || InterfaceSettings.GameSettings.FullScreenOption == 2)
                            {
                                fullScreen = false;
                            }
                            else
                            {
                                fullScreen = true;
                            }
                            
                            ResolutionManager.SetResolution(mode.Width, mode.Height, fullScreen);
                            ResolutionManager.ResetViewport();

                            if (InterfaceSettings.GameSettings.FullScreenOption == 2)
                            {
                                MakeGameBorderless();
                            }
                            else
                            {
                                MakeGameWindowed();
                            }

                            AudioCategory category = m_audioEngine.GetCategory("Music");
                            category.SetVolume(InterfaceSettings.GameSettings.MusicVolume / 10f);

                            InterfaceSettings.DisplaySettingsSavedMessage = true;
                            ComputerSettings.PreviousDisplayMode = mode;
                            ComputerSettings.PreviousFullScreenSetting = ComputerSettings.FullScreenSetting;
                            ComputerSettings.PreviousMusicSetting = ComputerSettings.MusicVolumeSetting;
                            ComputerSettings.PreviousSoundEffectSetting = ComputerSettings.SoundEffectVolumeSetting;
                        }
                    }
                    if (ComputerSettings.CurrentSettingSelection == 5)
                    {
                        InterfaceSettings.TitleScreenSelection = 0;
                        GameState.States = GameState.GameStates.TitleScreen;
                    }
                }
                if (character == 27)
                {
                    GameState.States = GameState.GameStates.TitleScreen;
                }
            }
            else if (GameState.States == GameState.GameStates.OptionsScreen)
            {
                if (character == '\b')
                { // backspace
                    if (InterfaceSettings.PlayerName.Length > 0)
                    {
                        InterfaceSettings.PlayerName.Remove(InterfaceSettings.PlayerName.Length - 1, 1);
                    }
                }
                else
                {
                    if (InterfaceSettings.PlayerName.Length != 12)
                    {
                        if (character != 13 && character != '\t')
                            InterfaceSettings.PlayerName.Append(character);
                    }
                }
                //commit options
                if (character == 13)
                {
                    if (InterfaceSettings.PlayerName.Length < 3)
                    {
                        InterfaceSettings.NameToShort = true;
                    }
                    else if (InterfaceSettings.HighScoreManager.LockedBasketballSelection)
                    {
                    }
                    else
                    {
                        InterfaceSettings.PlayerOptions.PlayerName = InterfaceSettings.PlayerName.ToString();
                        Screen.Camera.Limits = new Rectangle(0, 0, 1280, 720);
                        Screen.Camera.ResetCamera();
                        if (GameState.SelectedGameMode == 1)
                        {
                            ArcadeGoalManager.ResetArcadeGoals();
                            ArcadeModeScreenState.CleanUpGameState();
                            ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                            ArcadeModeScreenState.ReadyToFire = true;
                            PhysicalWorld.World.Gravity.Y = 25;
                        }
                        GameState.States = GameState.GameStates.GetReadyState;
                    }
                }
                if (character == 27)
                {
                    SoundManager.SelectedMusic.Pause();
                    MediaPlayer.Resume();
                    GameState.States = GameState.GameStates.TitleScreen;
                    //Exit();
                }
            }
            else if (GameState.States == GameState.GameStates.Playing || GameState.States == GameState.GameStates.ArcadeMode)
            {
                if (character == 27)
                {
                    GameState.States = GameState.GameStates.Paused;
                    SoundManager.MuteSounds();
                    GameTimer.StopGameTimer();
                }
            }
            else if (GameState.States == GameState.GameStates.Paused)
            {
                if (character == 27)
                {
                    if (GameState.SelectedGameMode == 0)
                    {
                        GameState.States = GameState.GameStates.Playing;
                    }
                    else
                    {
                        GameState.States = GameState.GameStates.ArcadeMode;
                    }
                    SoundManager.MuteSounds();
                    GameTimer.StartGameTimer();
                }

                if (character == 113)
                {
                    Exit();
                }

                if (character == 109)
                {
                    if (GameState.SelectedGameMode == 0)
                    {
                        ResetPosition();
                        BasketballManager.SelectedBasketball.BallEmitter.CleanUpParticles();
                        InterfaceSettings.GoalManager.ResetGoalManager();
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        SoundManager.MuteSounds();
                        SoundManager.SelectedMusic.Resume();
                        GameState.States = GameState.GameStates.OptionsScreen;
                        GameTimer.ResetTimer();
                    }
                    else
                    {
                        ArcadeGoalManager.ResetArcadeGoals();
                        ArcadeModeScreenState.CleanUpGameState();
                        ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                        ArcadeModeScreenState.ReadyToFire = true;
                        PhysicalWorld.World.Gravity.Y = 25;
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        SoundManager.MuteSounds();
                        SoundManager.SelectedMusic.Resume();
                        GameState.States = GameState.GameStates.OptionsScreen;
                        GameTimer.ResetTimer();
                    }
                }

                if (character == 114)
                {
                    if (GameState.SelectedGameMode == 0)
                    {
                        ResetPosition();
                        BasketballManager.SelectedBasketball.BallEmitter.CleanUpParticles();
                        InterfaceSettings.GoalManager.ResetGoalManager();
                        Unlocks.HighScoresLoaded = false;
                        SoundManager.MuteSounds();
                        GetReadyScreenState.SoundEffectCounter = 1;
                        GameState.States = GameState.GameStates.GetReadyState;
                        GameTimer.ResetTimer();
                    }
                    else
                    {
                        ArcadeGoalManager.ResetArcadeGoals();
                        ArcadeModeScreenState.CleanUpGameState();
                        ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                        ArcadeModeScreenState.ReadyToFire = true;
                        PhysicalWorld.World.Gravity.Y = 25;
                        Unlocks.HighScoresLoaded = false;
                        SoundManager.MuteSounds();
                        GetReadyScreenState.SoundEffectCounter = 1;
                        GameState.States = GameState.GameStates.GetReadyState;
                        GameTimer.ResetTimer();
                    }
                }
            }
            else if (GameState.States == GameState.GameStates.GameEnd)
            {
                if (character == 113)
                {
                    Exit();
                }


                if (character == 109)
                {
                    if (GameState.SelectedGameMode == 0)
                    {
                        ResetPosition();
                        BasketballManager.SelectedBasketball.BallEmitter.CleanUpParticles();
                        InterfaceSettings.GoalManager.ResetGoalManager();
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        SoundManager.SelectedMusic.Resume();
                        GameState.States = GameState.GameStates.OptionsScreen;
                        GameTimer.ResetTimer();
                    }
                    else
                    {
                        ArcadeGoalManager.ResetArcadeGoals();
                        ArcadeModeScreenState.CleanUpGameState();
                        ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                        ArcadeModeScreenState.ReadyToFire = true;
                        PhysicalWorld.World.Gravity.Y = 25;
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        SoundManager.SelectedMusic.Resume();
                        GameState.States = GameState.GameStates.OptionsScreen;
                        GameTimer.ResetTimer();
                    }
                }

                if (character == 114)
                {
                    if (GameState.SelectedGameMode == 0)
                    {
                        ResetPosition();
                        BasketballManager.SelectedBasketball.BallEmitter.CleanUpParticles();
                        InterfaceSettings.GoalManager.ResetGoalManager();
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        GameState.States = GameState.GameStates.GetReadyState;
                        GameTimer.ResetTimer();
                    }
                    else
                    {
                        ArcadeGoalManager.ResetArcadeGoals();
                        ArcadeModeScreenState.CleanUpGameState();
                        ArcadeModeScreenState.PlayerSelectedBall = BasketballManager.SelectedBasketball;
                        ArcadeModeScreenState.ReadyToFire = true;
                        PhysicalWorld.World.Gravity.Y = 25;
                        Unlocks.HighScoresLoaded = false;
                        GetReadyScreenState.SoundEffectCounter = 1;
                        GameState.States = GameState.GameStates.GetReadyState;
                        GameTimer.ResetTimer();
                    }
                }
            }
        }
        #endregion

        private static void ResetPosition()
        {
            PhysicalWorld.World.Gravity.Y = 0;
            InterfaceSettings.BasketballManager.BasketballBody.Awake = false;
            InterfaceSettings.BasketballManager.BasketballBody.Position = Screen.RandomizePosition();
        }

        private void MakeGameBorderless()
        {
            IntPtr windowHandle = Window.Handle;
            var control = Control.FromHandle(windowHandle);
            var form = control.FindForm();
            if (form != null)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.ClientSize = new Size(InterfaceSettings.GameSettings.DisplayModeWidth, InterfaceSettings.GameSettings.DisplayModeHeight);
            }
        }

        private void MakeGameWindowed()
        {
            IntPtr windowHandle = Window.Handle;
            var control = Control.FromHandle(windowHandle);
            var form = control.FindForm();
            if (form != null)
            {
                form.FormBorderStyle = FormBorderStyle.Fixed3D;
                form.WindowState = FormWindowState.Normal;
                form.ClientSize = new Size(InterfaceSettings.GameSettings.DisplayModeWidth, InterfaceSettings.GameSettings.DisplayModeHeight);
            }
        }

        [DllImport("User32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern IntPtr LoadCursorFromFile(String path);

        private static Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null) fi.SetValue(curs, true);
            return curs;
        }
    }
}
