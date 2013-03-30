using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public static class StaticEntity
    {
        public static Body CreateStaticRectangleBody(World world, Vector2 position, float meterInPixel, float height, float width, float density, float restitution, float friction)
        {
            Body staticBody = BodyFactory.CreateRectangle(world, height/meterInPixel, width/meterInPixel, density, position);
            staticBody.BodyType = BodyType.Static;
            staticBody.Restitution = restitution;
            staticBody.Friction = friction;
            return staticBody;
        }
    }
}
