using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Interface.Screen
{
    public static class ResolutionManager
    {
        static private GraphicsDeviceManager device;

        static private int width = 800;
        static private int height = 600;
        static private int virtualWidth = 1280;
        static private int virtualHeight = 720;
        static private Matrix scaleMatrix;
        static private bool fullScreen;
        static private bool dirtyMatrix = true;

        public static void Init(ref GraphicsDeviceManager deviceManager)
        {
            width = deviceManager.PreferredBackBufferWidth;
            height = deviceManager.PreferredBackBufferHeight;
            device = deviceManager;
            dirtyMatrix = true;
            ApplyResolutionSettings();
        }

        public static int GetBackBufferWidth
        {
            get { return device.GraphicsDevice.Viewport.Width; }
        }

        public static int GetBackBufferHeight
        {
            get { return device.GraphicsDevice.Viewport.Height; }
        }

        public static int GetViewportX
        {
            get { return device.GraphicsDevice.Viewport.X; }
        }

        public static int GetViewportY
        {
            get { return device.GraphicsDevice.Viewport.Y; }
        }

        public static Matrix GetTransformationMatrix()
        {
            if (dirtyMatrix) RecreateScaleMatrix();

            return scaleMatrix;
        }

        public static void SetResolution(int screenWidth, int screenHeight, bool isFullScreen)
        {
            width = screenWidth;
            height = screenHeight;
            fullScreen = isFullScreen;

            ApplyResolutionSettings();
        }

        public static void SetVirtualResolution(int screenWidth, int screenHeight)
        {
            virtualWidth = screenWidth;
            virtualHeight = screenHeight;
            dirtyMatrix = true;
        }

        private static void ApplyResolutionSettings()
        {
            if (!fullScreen)
            {
                if ((width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                        && (height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    device.PreferredBackBufferWidth = width;
                    device.PreferredBackBufferHeight = height;
                    device.IsFullScreen = fullScreen;
                    device.ApplyChanges();
                }
            }
            else
            {
                foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if ((displayMode.Width == width) && (displayMode.Height == height))
                    {
                        device.PreferredBackBufferWidth = width;
                        device.PreferredBackBufferHeight = height;
                        device.IsFullScreen = fullScreen;
                        device.ApplyChanges();
                    }
                }
            }

            dirtyMatrix = true;
            width = device.PreferredBackBufferWidth;
            height = device.PreferredBackBufferHeight;
        }

        public static void BeginDraw()
        {
            FullViewport();
            device.GraphicsDevice.Clear(Color.Black);
            ResetViewport();
            device.GraphicsDevice.Clear(Color.Black);
        }

        private static void RecreateScaleMatrix()
        {
            dirtyMatrix = false;
            scaleMatrix = Matrix.CreateScale(
                           (float)device.GraphicsDevice.Viewport.Width / virtualWidth,
                           (float)device.GraphicsDevice.Viewport.Width / virtualWidth,
                           1f);
        }

        public static void FullViewport()
        {
            Viewport viewPort = new Viewport();
            viewPort.X = viewPort.Y = 0;
            viewPort.Width = width;
            viewPort.Height = height;
            device.GraphicsDevice.Viewport = viewPort;
        }

        public static float GetVirtualAspectRatio()
        {
            return (float)virtualWidth/virtualHeight;
        }

        public static void ResetViewport()
        {
            float targetAspectRatio = GetVirtualAspectRatio();
            int viewWidth = device.PreferredBackBufferWidth;
            int viewHeight = (int) (viewWidth/targetAspectRatio + .5f);
            bool changed = false;

            if (viewHeight > device.PreferredBackBufferHeight)
            {
                viewHeight = device.PreferredBackBufferHeight;
                viewWidth = (int) (viewHeight*targetAspectRatio + .5f);
                changed = true;
            }

            Viewport viewport = new Viewport();
            viewport.X = (device.PreferredBackBufferWidth/2) - (viewWidth/2);
            viewport.Y = (device.PreferredBackBufferHeight/2) - (viewHeight/2);
            viewport.Width = viewWidth;
            viewport.Height = viewHeight;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                dirtyMatrix = true;
            }

            device.GraphicsDevice.Viewport = viewport;
        }
    }
}
