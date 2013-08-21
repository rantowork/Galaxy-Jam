using Microsoft.Xna.Framework;

namespace SpoidaGamesArcadeLibrary.Effects._2D
{
    public class AdvancedParticle
    {
        // constructor
        public AdvancedParticle()
        {
            Scale = 1.0f;
            Active = false;
            LifeTimeStart = 0.0f;
            LifeTime = 0.0f;
            Position = new Vector2();
            Velocity = new Vector2();
        }

        /// <summary>
        /// Initialize the particle
        /// </summary>
        /// <param name="pos">initial position of the particle</param>
        /// <param name="vel">initial velocity of the particle</param>
        /// <param name="lifeTime">how long the particle will live</param>
        /// <param name="scale">how big will the particle be</param>
        /// <param name="active">is the particle active (or alive)?</param>
        public void Initialize(Vector2 pos, Vector2 vel, float lifeTime, float scale, bool active)
        {
            Position = pos;
            Velocity = vel;
            LifeTime = lifeTime;
            Scale = scale;
            Active = active;
            LifeTimeStart = 0.0f;
        }

        /// <summary>
        /// Update the particles position and update the particles life time
        /// </summary>
        /// <param name="deltaTime">time between function calls</param>
        public void Update(float deltaTime)
        {
            // add the current velocity to the position
            Position += Velocity * deltaTime;

            // update the life time of this particle
            LifeTimeStart += deltaTime;

            // if the life of the particle has expired
            // this particle is no longer active
            if (LifeTimeStart > LifeTime)
                Active = false;
        }

        //*********************
        // Properties
        //*********************

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float LifeTime { get; set; }
        public float LifeTimeStart { get; set; }
        public bool Active { get; set; }
        public float Scale { get; set; }
    }
}
