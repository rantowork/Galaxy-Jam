using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public static class PhysicalEntity
    {
        public static Body CreateStaticRectangleBody(World world, Vector2 position, float meterInPixel, float height, float width, float density, float restitution, float friction)
        {
            Body staticBody = BodyFactory.CreateRectangle(world, height/meterInPixel, width/meterInPixel, density, position);
            staticBody.BodyType = BodyType.Static;
            staticBody.Restitution = restitution;
            staticBody.Friction = friction;
            return staticBody;
        }

        public static Body CreateDynamicCircularBody(World world, Vector2 position, float meterInPixel, float radius, float density, float restitution, float friction)
        {
            Body dynamicBody = BodyFactory.CreateCircle(world, radius/(2f*meterInPixel), density, position);
            dynamicBody.BodyType = BodyType.Dynamic;
            dynamicBody.Restitution = restitution;
            dynamicBody.Friction = friction;
            return dynamicBody;
        }
    }
}
