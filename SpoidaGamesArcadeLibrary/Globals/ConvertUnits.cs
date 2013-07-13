using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public static class ConvertUnits
    {
        private static float s_displayUnitsToSimUnitsRatio = 100f;
        private static float s_simUnitsToDisplayUnitsRatio = 1 / s_displayUnitsToSimUnitsRatio;

        public static void SetDisplayUnitToSimUnitRatio(float displayUnitsPerSimUnit)
        {
            s_displayUnitsToSimUnitsRatio = displayUnitsPerSimUnit;
            s_simUnitsToDisplayUnitsRatio = 1 / displayUnitsPerSimUnit;
        }

        public static float ToDisplayUnits(float simUnits)
        {
            return simUnits * s_displayUnitsToSimUnitsRatio;
        }

        public static float ToDisplayUnits(int simUnits)
        {
            return simUnits * s_displayUnitsToSimUnitsRatio;
        }

        public static Vector2 ToDisplayUnits(Vector2 simUnits)
        {
            return simUnits * s_displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(ref Vector2 simUnits, out Vector2 displayUnits)
        {
            Vector2.Multiply(ref simUnits, s_displayUnitsToSimUnitsRatio, out displayUnits);
        }

        public static Vector3 ToDisplayUnits(Vector3 simUnits)
        {
            return simUnits * s_displayUnitsToSimUnitsRatio;
        }

        public static Vector2 ToDisplayUnits(float x, float y)
        {
            return new Vector2(x, y) * s_displayUnitsToSimUnitsRatio;
        }

        public static void ToDisplayUnits(float x, float y, out Vector2 displayUnits)
        {
            displayUnits = Vector2.Zero;
            displayUnits.X = x * s_displayUnitsToSimUnitsRatio;
            displayUnits.Y = y * s_displayUnitsToSimUnitsRatio;
        }

        public static float ToSimUnits(float displayUnits)
        {
            return displayUnits * s_simUnitsToDisplayUnitsRatio;
        }

        public static float ToSimUnits(double displayUnits)
        {
            return (float)displayUnits * s_simUnitsToDisplayUnitsRatio;
        }

        public static float ToSimUnits(int displayUnits)
        {
            return displayUnits * s_simUnitsToDisplayUnitsRatio;
        }

        public static Vector2 ToSimUnits(Vector2 displayUnits)
        {
            return displayUnits * s_simUnitsToDisplayUnitsRatio;
        }

        public static Vector3 ToSimUnits(Vector3 displayUnits)
        {
            return displayUnits * s_simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(ref Vector2 displayUnits, out Vector2 simUnits)
        {
            Vector2.Multiply(ref displayUnits, s_simUnitsToDisplayUnitsRatio, out simUnits);
        }

        public static Vector2 ToSimUnits(float x, float y)
        {
            return new Vector2(x, y) * s_simUnitsToDisplayUnitsRatio;
        }

        public static Vector2 ToSimUnits(double x, double y)
        {
            return new Vector2((float)x, (float)y) * s_simUnitsToDisplayUnitsRatio;
        }

        public static void ToSimUnits(float x, float y, out Vector2 simUnits)
        {
            simUnits = Vector2.Zero;
            simUnits.X = x * s_simUnitsToDisplayUnitsRatio;
            simUnits.Y = y * s_simUnitsToDisplayUnitsRatio;
        }
    }
}
