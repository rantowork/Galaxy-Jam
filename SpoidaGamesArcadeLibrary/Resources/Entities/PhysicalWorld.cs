using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public static class PhysicalWorld
    {
        public const float MetersInPixels = 64f;

        private static World world;
        public static World World
        {
            get { return world; }
            set { world = value; }
        }
        
        public static Body CreateStaticRectangleBody(Vector2 position, float height, float width, float density, float restitution, float friction)
        {
            Body staticBody = BodyFactory.CreateRectangle(world, height / MetersInPixels, width / MetersInPixels, density, position);
            staticBody.BodyType = BodyType.Static;
            staticBody.Restitution = restitution;
            staticBody.Friction = friction;
            return staticBody;
        }
    }
}
