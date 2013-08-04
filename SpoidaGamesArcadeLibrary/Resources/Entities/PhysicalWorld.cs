using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SpoidaGamesArcadeLibrary.Globals;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Resources.Entities
{
    public static class PhysicalWorld
    {
        public const float MetersInPixels = 64f;

        private const double GLOWTIME = 200;
        public static bool BackboardCollisionHappened { get; set; }
        public static double BackboardGlowTimer { get; set; }

        public static bool LeftRimCollisionHappened { get; set; }
        public static double LeftrimGlowTimer { get; set; }

        public static bool RightRimCollisionHappened { get; set; }
        public static double RightrimGlowTimer { get; set; }

        public static World World { get; set; }
        public static Body BackboardBody { get; set; }
        public static Body LeftRimBody { get; set; }
        public static Body RightRimBody { get; set; }

        public static Body CreateStaticRectangleBody(Vector2 position, float height, float width, float density, float restitution, float friction)
        {
            Body staticBody = BodyFactory.CreateRectangle(World, width / MetersInPixels, height / MetersInPixels, density, position);
            staticBody.BodyType = BodyType.Static;
            staticBody.Restitution = restitution;
            staticBody.Friction = friction;
            return staticBody;
        }

        public static void LoadPhysicalWorldEntities()
        {
            BackboardBody = CreateStaticRectangleBody(new Vector2(68f / MetersInPixels, 116f / MetersInPixels), 140f, 6f, 1f, .3f, .1f);
            BackboardBody.OnCollision += BackboardCollision;

            LeftRimBody = CreateStaticRectangleBody(new Vector2(80f / MetersInPixels, 206 / MetersInPixels), 16f, 10f, 1f, .3f, .1f);
            LeftRimBody.OnCollision += LeftRimCollision;

            RightRimBody = CreateStaticRectangleBody(new Vector2(188 / MetersInPixels, 206 / MetersInPixels), 16f, 54f, 1f, .3f, .1f);
            RightRimBody.OnCollision += RightRimCollision;
        }

        public static bool BackboardCollision(Fixture f1, Fixture f2, Contact contact)
        {
            BackboardCollisionHappened = true;
            InterfaceSettings.GoalManager.BackboardHit = true;
            SoundManager.PlaySoundEffect(Sounds.CollisionSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }

        public static bool LeftRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            LeftRimCollisionHappened = true;
            InterfaceSettings.GoalManager.RimHit = true;
            SoundManager.PlaySoundEffect(Sounds.CollisionSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }

        public static bool RightRimCollision(Fixture f1, Fixture f2, Contact contact)
        {
            RightRimCollisionHappened = true;
            InterfaceSettings.GoalManager.RimHit = true;
            SoundManager.PlaySoundEffect(Sounds.CollisionSoundEffect, (float)InterfaceSettings.GameSettings.SoundEffectVolume / 10, 0.0f, 0.0f);
            return true;
        }

        public static void GlowBackboard(GameTime gameTime)
        {
            BackboardGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (BackboardGlowTimer > GLOWTIME)
            {
                BackboardCollisionHappened = false;
                BackboardGlowTimer = 0;
            }
        }

        public static void GlowLeftRim(GameTime gameTime)
        {
            LeftrimGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (LeftrimGlowTimer > GLOWTIME)
            {
                LeftRimCollisionHappened = false;
                LeftrimGlowTimer = 0;
            }
        }

        public static void GlowRightRim(GameTime gameTime)
        {
            RightrimGlowTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (RightrimGlowTimer > GLOWTIME)
            {
                RightRimCollisionHappened = false;
                RightrimGlowTimer = 0;
            }
        }
    }
}
