using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using GalaxyJam.Particles;
using GalaxyJam.Screen;
using GalaxyJam.Starfield;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nuclex.Input;

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
        private Vector2 screenCenter;

        //Game State
        private enum GameStates
        {
            StartScreen,
            Playing,
            Paused
        } ;
        private GameStates gameState = GameStates.StartScreen;

        //Textures
        private Texture2D basketBallSprite;
        private Texture2D lineSprite;
        private Texture2D backboardSprite;
        private Texture2D rimSprite;
        private Texture2D galaxyJamLogo;

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

        //Starfield
        private Stars starField;

        //Music
        private Song bgm;
        private bool songStart;
        private bool songPaused;

        //Sounds
        private SoundEffect basketBallShotSoundEffect;
        private SoundEffect basketScored;
        private bool muteSounds;

        //Particles
        private ParticleEngine basketballFlameParticleEngine;

        //Goal stuff move outta here soon
        private Rectangle basket = new Rectangle(85, 208, 76,1);
        private bool goalScored;
        private int score;

        private float xOffset;
        private float yOffset;
        private double shakeTimer;
        private const double SHAKE_TIME = 200;
        private const int SHAKE_OFFSET = 20;
        private bool shaking;
        private bool shakeDireciton;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this) {PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720};

            Content.RootDirectory = "Content";
            input = new InputManager(Services, Window.Handle);
            Components.Add(input);

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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width/2f,
                                       graphics.GraphicsDevice.Viewport.Height/2f);

            galaxyJamLogo = Content.Load<Texture2D>("Textures/GalaxyJamLogo");

            basketBallSprite = Content.Load<Texture2D>("Textures/BasketBall"); //32x32 => .5m x .5m
            Vector2 basketBallPosition = new Vector2((rand.Next(370,1230))/METER_IN_PIXEL,(rand.Next(310,680))/METER_IN_PIXEL);
            basketBallBody = BodyFactory.CreateCircle(world, 32f/(2f*METER_IN_PIXEL), 1f, basketBallPosition);
            basketBallBody.BodyType = BodyType.Dynamic;
            basketBallBody.Restitution = 0.3f;
            basketBallBody.Friction = 0.5f;

            backboardSprite = Content.Load<Texture2D>("Textures/Backboard");
            Vector2 backboardPosition = new Vector2(64f/METER_IN_PIXEL, 116f/METER_IN_PIXEL);
            backboardBody = BodyFactory.CreateRectangle(world, 6f/METER_IN_PIXEL, 140f/METER_IN_PIXEL, 1f, backboardPosition);
            backboardBody.BodyType = BodyType.Static;
            backboardBody.Restitution = 0.3f;
            backboardBody.Friction = 0.1f;

            rimSprite = Content.Load<Texture2D>("Textures/Rim");
            Vector2 leftRimPosition = new Vector2(80f/METER_IN_PIXEL, 206/METER_IN_PIXEL);
            leftRimBody = BodyFactory.CreateRectangle(world, 10f/METER_IN_PIXEL, 16f/METER_IN_PIXEL, 1f, leftRimPosition);
            leftRimBody.BodyType = BodyType.Static;
            leftRimBody.Restitution = 0.3f;
            leftRimBody.Friction = 0.1f;

            Vector2 rightRimPosition = new Vector2(166/METER_IN_PIXEL, 206/METER_IN_PIXEL);
            rightRimBody = BodyFactory.CreateRectangle(world, 10f/METER_IN_PIXEL, 16f/METER_IN_PIXEL, 1f, rightRimPosition);
            rightRimBody.BodyType = BodyType.Static;
            rightRimBody.Restitution = 0.3f;
            rightRimBody.Friction = 0.1f;

            segoe = Content.Load<SpriteFont>("Fonts/Segoe");

            lineSprite = Content.Load<Texture2D>("Textures/LineSprite");
            starField = new Stars(Window.ClientBounds.Width, Window.ClientBounds.Height, 300, lineSprite, new Rectangle(0,0,2,2));

            bgm = Content.Load<Song>("Music/bgm");
            MediaPlayer.IsRepeating = true;

            basketBallShotSoundEffect = Content.Load<SoundEffect>(@"SoundEffects/BasketballShot");
            basketScored = Content.Load<SoundEffect>(@"SoundEffects/BasketScored");

            List<Texture2D> particleTextures = new List<Texture2D> {Content.Load<Texture2D>("Textures/ExampleFire")};
            List<Color> flamingBasketballColors = new List<Color>
                                                      {
                                                          Color.DarkRed,
                                                          Color.DarkOrange
                                                      };
            basketballFlameParticleEngine = new ParticleEngine(particleTextures, new Vector2(-40, -40), flamingBasketballColors);

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
                case GameStates.Playing:
                    if (!songStart)
                    {
                        MediaPlayer.Volume = .8f;
                        MediaPlayer.Play(bgm);
                        songStart = true;
                    }
                    starField.Update(gameTime);
                    world.Step((float) gameTime.ElapsedGameTime.TotalMilliseconds*0.001f);
                    HandleInput();
                    HandlePosition();
                    basketballFlameParticleEngine.EmitterLocation = basketBallBody.Position*METER_IN_PIXEL;
                    basketballFlameParticleEngine.Update();

                    Vector2 basketballCenter = basketBallBody.WorldCenter*METER_IN_PIXEL;
                    Rectangle basketballCenterRectangle = new Rectangle((int)basketballCenter.X-8, (int)basketballCenter.Y-8, 16, 16);
                    if (GoalScored(basketballCenterRectangle) && !goalScored)
                    {
                        goalScored = true; 
                        if (!muteSounds)
                        {
                            basketScored.Play(1.0f, 0.0f, 0.0f);
                        }
                        score += 100;
                        shaking = true;
                    }

                    if (shaking)
                    {
                        ShakeCamera(gameTime);
                    }
                    else
                    {
                        camera.Position = Vector2.Zero;
                    }

                    break;
                case GameStates.Paused:
                    break;
            }
            base.Update(gameTime);
        }

        protected bool GoalScored(Rectangle basketball)
        {
            return basket.Intersects(basketball);
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
                case GameStates.Playing:

                    Vector2 basketBallPosition = basketBallBody.Position*METER_IN_PIXEL;
                    float basketBallRotation = basketBallBody.Rotation;
                    Vector2 basketBallOrigin = new Vector2(basketBallSprite.Width/2f, basketBallSprite.Height/2f);

                    Vector2 backboardPosition = backboardBody.Position*METER_IN_PIXEL;
                    Vector2 backboardOrigin = new Vector2(backboardSprite.Width/2f,backboardSprite.Height/2f);

                    Vector2 leftRimPosition = leftRimBody.Position*METER_IN_PIXEL;
                    Vector2 leftRimOrigin = new Vector2(rimSprite.Width/2f, rimSprite.Height/2f);

                    Vector2 rightRimPosition = rightRimBody.Position*METER_IN_PIXEL;
                    Vector2 rightRimOrigin = new Vector2(rimSprite.Width/2f, rimSprite.Height/2f);

                    //draw starfield separate from other draw methods to keep it simple
                    spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.ViewMatrix);
                    spriteBatch.Draw(lineSprite, new Rectangle(0,0,1280,720),Color.Black);
                    starField.Draw(spriteBatch);
                    spriteBatch.Draw(lineSprite, basket, Color.White);
                    spriteBatch.End();

                    //draw particle engine separate from other draw methods so that we can take advantage of additive blending
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera.ViewMatrix);
                    basketballFlameParticleEngine.Draw(spriteBatch);
                    spriteBatch.End();

                    //draw objects which contain a body that can have forces applied to it
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.ViewMatrix);
                    //draw basketball
                    spriteBatch.Draw(basketBallSprite, basketBallPosition, null, Color.White, basketBallRotation, basketBallOrigin, 1f, SpriteEffects.None, 0f);
                    //draw backboard
                    spriteBatch.Draw(backboardSprite, backboardPosition, null, Color.White, 0f, backboardOrigin, 1f, SpriteEffects.None, 0f);
                    //draw left rim
                    spriteBatch.Draw(rimSprite, leftRimPosition, null, Color.White, 0f, leftRimOrigin, 1f, SpriteEffects.None, 0f);
                    //draw right rim
                    spriteBatch.Draw(rimSprite, rightRimPosition, null, Color.White, 0f, rightRimOrigin, 1f, SpriteEffects.None, 0f);
                    string currentScore = String.Format("Player Score: {0}", score);
                    spriteBatch.DrawString(segoe, currentScore, new Vector2(10, 10), Color.White);
                    spriteBatch.End();

                    break;
                case GameStates.Paused:
                    break;
            }

            base.Draw(gameTime);
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
                    if (!muteSounds)
                    {
                        basketBallShotSoundEffect.Play(1.0f, 0.0f, 0.0f);
                    }
                }
            }
        }

        private void HandlePosition()
        {
            if (basketBallBody.Position.Y > 720/METER_IN_PIXEL)
            {
                world.Gravity.Y = 0;
                basketBallBody.Awake = false;
                basketBallBody.Position = RandomizePosition();
                goalScored = false;
            }
        }

        private Vector2 RandomizePosition()
        {
            return new Vector2((rand.Next(370, 1230)) / METER_IN_PIXEL, (rand.Next(310, 680)) / METER_IN_PIXEL);
        }

        private void HandleShotAngle(MouseState state)
        {
            Vector2 basketballLocation = new Vector2(basketBallBody.Position.X*METER_IN_PIXEL,
                                                     basketBallBody.Position.Y*METER_IN_PIXEL);
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
                    camera.Limits = new Rectangle(0, 0, 1280, 720);
                    ResetCamera();
                    gameState = GameStates.Playing;
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
                    Exit();
                }
                if (character == 112 && !songPaused)
                {
                    MediaPlayer.Pause();
                    songPaused = true;
                }
                else if (character == 112 && songPaused)
                {
                    MediaPlayer.Resume();
                    songPaused = false;
                }
                if (character == 109 && !muteSounds)
                {
                    MediaPlayer.IsMuted = true;
                    muteSounds = true;
                }
                else
                {
                    MediaPlayer.IsMuted = false;
                    muteSounds = false;
                }
            }
            else if (gameState == GameStates.Paused)
            {
                if (character == 27)
                {
                    Exit();
                }
            }
        }

        private void ResetCamera()
        {
            camera.Zoom = 1f;
            camera.Position = Vector2.Zero;
        }

        private void ShakeCamera(GameTime gameTime)
        {
            if (shakeTimer == 0)
            {
                camera.Position = Vector2.Zero;
            }

            shakeTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (shakeTimer > SHAKE_TIME)
            {
                shakeTimer = 0;
                shaking = false;
                xOffset = 0;
                yOffset = 0;
            }
            else
            {
                ApplyCameraShake(gameTime);
            }
        }

        private void ApplyCameraShake(GameTime gameTime)
        {
            if (shakeDireciton)
            {
                xOffset -= 1.5f*gameTime.ElapsedGameTime.Milliseconds;
                if (xOffset < -SHAKE_OFFSET)
                {
                    xOffset = -SHAKE_OFFSET;
                    shakeDireciton = !shakeDireciton;
                }
                yOffset = xOffset;
            }
            else
            {
                xOffset += 1.5f*gameTime.ElapsedGameTime.Milliseconds;
                if (xOffset > SHAKE_OFFSET)
                {
                    xOffset = SHAKE_OFFSET;
                    shakeDireciton = !shakeDireciton;
                }
                yOffset = xOffset;
            }
            camera.Position = new Vector2(xOffset, yOffset);
        }
    }
}
