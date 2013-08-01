using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Interface.Screen
{
    public class _3DCamera
    {
        /// <summary>
        /// View Reference Point - The camera's position in the 3D environment.
        /// <para>This is a Free Camera variable.</para>
        /// </summary>
        public Vector3 SVrp;

        /// <summary>
        /// View Plane Normal - The direction the camera is facing.
        /// <para>This is a Free Camera variable.</para>
        /// </summary>
        public Vector3 CVpn;

        /// <summary>
        /// View Up - The Up direction of the camera.
        /// <para>This is a Free Camera variable.</para>
        /// </summary>
        public Vector3 CVup;

        /// <summary>
        /// View Left - The Left direction of the camera.
        /// <para>This is a Free Camera variable.</para>
        /// </summary>
        public Vector3 CvLeft;

        public float FCameraArc, FCameraRotation, FCameraDistance;	// Fixed Camera Variables.

        /// <summary>
        /// The Position that the Fixed Camera should rotate around.
        /// <para>This is a Fixed Camera variable.</para>
        /// </summary>
        public Vector3 SFixedCameraLookAtPosition;

        /// <summary>
        /// Indicates whether to use a Fixed Camera or a Free Camera.
        /// </summary>
        public bool BUsingFixedCamera;

        /// <summary>
        /// Explicit constructor
        /// </summary>
        /// <param name="bUseFixedCamera">True to use the Fixed Camera, false to use the Free Camera</param>
        public _3DCamera(bool bUseFixedCamera)
        {
            // Initialize variables with dummy values so we can call the Reset functions
            SVrp = CVpn = CVup = CvLeft = SFixedCameraLookAtPosition = Vector3.Zero;
            FCameraArc = FCameraRotation = FCameraDistance = 0.0f;

            // Use the specified Camera type
            BUsingFixedCamera = bUseFixedCamera;

            // Initialize the variables with their proper values
            ResetFreeCameraVariables();
            ResetFixedCameraVariables();
        }

        /// <summary>
        /// Get the current Position of the Camera
        /// </summary>
        public Vector3 Position
        {
            get
            {
                // If we are using the Fixed Camera
                if (BUsingFixedCamera)
                {
                    // Calculate the View Matrix
                    Matrix cViewMatrix = Matrix.CreateTranslation(SFixedCameraLookAtPosition) *
                                         Matrix.CreateRotationY(MathHelper.ToRadians(FCameraRotation)) *
                                         Matrix.CreateRotationX(MathHelper.ToRadians(FCameraArc)) *
                                         Matrix.CreateLookAt(new Vector3(0, 0, -FCameraDistance),
                                                             new Vector3(0, 0, 0), Vector3.Up);

                    // Invert the View Matrix
                    cViewMatrix = Matrix.Invert(cViewMatrix);

                    // Pull and return the Camera Coordinates from the inverted View Matrix
                    return cViewMatrix.Translation;
                }
                // Else we are using the Free Camera
                return SVrp;
            }
        }

        /// <summary>
        /// Reset the Fixed Camera Variables to their default values
        /// </summary>
        public void ResetFixedCameraVariables()
        {
            FCameraArc = 0.0f;
            FCameraRotation = 0f;
            FCameraDistance = 300f;
            SFixedCameraLookAtPosition = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Reset the Free Camera Variables to their default values
        /// </summary>
        public void ResetFreeCameraVariables()
        {
            SVrp = new Vector3(0.0f, 50.0f, 300.0f);
            CVpn = Vector3.Forward;
            CVup = Vector3.Up;
            CvLeft = Vector3.Left;
        }

        /// <summary>
        /// Normalize the Camera Directions and maintain proper Right and Up directions
        /// </summary>
        public void NormalizeCameraAndCalculateProperUpAndRightDirections()
        {
            // Calculate the new Right and Up directions
            CVpn.Normalize();
            CvLeft = Vector3.Cross(CVup, CVpn);
            CvLeft.Normalize();
            CVup = Vector3.Cross(CVpn, CvLeft);
            CVup.Normalize();
        }

        /// <summary>
        /// Move the Camera Forward or Backward
        /// </summary>
        /// <param name="fAmountToMove">The distance to Move</param>
        public void MoveCameraForwardOrBackward(float fAmountToMove)
        {
            CVpn.Normalize();
            SVrp += (CVpn * fAmountToMove);
        }

        /// <summary>
        /// Move the Camera Horizontally
        /// </summary>
        /// <param name="fAmountToMove">The distance to move horizontally</param>
        public void MoveCameraHorizontally(float fAmountToMove)
        {
            CvLeft.Normalize();
            SVrp += (CvLeft * fAmountToMove);
        }

        /// <summary>
        /// Move the Camera Vertically
        /// </summary>
        /// <param name="fAmountToMove">The distance to move Vertically</param>
        public void MoveCameraVertically(float fAmountToMove)
        {
            // Move the Camera along the global Y axis
            SVrp.Y += fAmountToMove;
        }

        /// <summary>
        /// Rotate the Camera Horizontally
        /// </summary>
        /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
        public void RotateCameraHorizontally(float fAmountToRotateInRadians)
        {
            // Rotate the Camera about the global Y axis
            Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, fAmountToRotateInRadians);
            CVpn = Vector3.Transform(CVpn, cRotationMatrix);
            CVup = Vector3.Transform(CVup, cRotationMatrix);

            // Normalize all of the Camera directions since they have changed
            NormalizeCameraAndCalculateProperUpAndRightDirections();
        }

        /// <summary>
        /// Rotate the Camera Vertically
        /// </summary>
        /// <param name="fAmountToRotateInRadians">The amount to Rotate in radians</param>
        public void RotateCameraVertically(float fAmountToRotateInRadians)
        {
            // Rotate the Camera
            Matrix cRotationMatrix = Matrix.CreateFromAxisAngle(CvLeft, fAmountToRotateInRadians);
            CVpn = Vector3.Transform(CVpn, cRotationMatrix);
            CVup = Vector3.Transform(CVup, cRotationMatrix);

            // Normalize all of the Camera directions since they have changed
            NormalizeCameraAndCalculateProperUpAndRightDirections();
        }
    }
}
