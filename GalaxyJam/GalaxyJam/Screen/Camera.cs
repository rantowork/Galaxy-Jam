using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyJam.Screen
{
    class Camera
    {
        private const float MIN_ZOOM = 0.1f;

        private readonly Viewport viewport;
        private readonly Vector2 origin;

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                //ValidatePosition();
            }
        }

        private float zoom = 1f;
        public float Zoom
        {
            get { return zoom; }
            set
            {
                zoom = MathHelper.Max(value, MIN_ZOOM);
                ValidateZoom();
                ValidatePosition();
            }
        }

        private Rectangle? limits;
        public Rectangle? Limits
        {
            set
            {
                limits = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        public Camera(Viewport port)
        {
            viewport = port;
            viewport.Width = 1280;
            viewport.Height = 720;
            origin = new Vector2(port.Width / 2.0f, port.Height / 2.0f);
        }

        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-position, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-origin, 0f)) *
                       Matrix.CreateScale(zoom, zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(origin, 0f));
            }
        }

        public Matrix InverseViewMatrix
        {
            get
            {
                Matrix matrix = Matrix.CreateTranslation(new Vector3(-position, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-origin, 0f)) *
                       Matrix.CreateScale(zoom, zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(origin, 0f));
                return Matrix.Invert(matrix);
            }
        }

        private void ValidatePosition()
        {
            if (limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(ViewMatrix));
                Vector2 cameraSize = new Vector2(viewport.Width, viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(limits.Value.Left, limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(limits.Value.Right, limits.Value.Bottom);
                Vector2 positionOffset = position - cameraWorldMin;
                position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }

        private void ValidateZoom()
        {
            if (limits.HasValue)
            {
                float minZoomX = (float)viewport.Width / limits.Value.Width;
                float minZoomY = (float)viewport.Height / limits.Value.Height;
                zoom = MathHelper.Max(zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }
    }
}
