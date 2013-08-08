using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class Sounds
    {
        public static SoundEffect BasketBallShotSoundEffect { get; set; }
        public static SoundEffect BasketScoredSoundEffect { get; set; }
        public static SoundEffect CollisionSoundEffect { get; set; }
        public static SoundEffect CountdownGoSoundEffect { get; set; }
        public static SoundEffect CountdownBeepSoundEffect { get; set; }
        public static SoundEffect StreakWubSoundEffect { get; set; }
        public static SoundEffect HighScoreSwooshSoundEffect { get; set; }
        public static SoundEffect UnlockedSoundEffect { get; set; }
        public static SoundEffect EngageHomingBall { get; set; }
        public static SoundEffect HomingMissleBallShot { get; set; }
        public static SoundEffect ArcadeModeStreak { get; set; }

        public static Song AmbientSpaceSong { get; set; }

        public static void LoadSounds(ContentManager content)
        {
            BasketBallShotSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/pulse");
            BasketScoredSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/BasketScored");
            CollisionSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/Thud");
            CountdownBeepSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/Countdown");
            CountdownGoSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/Go");
            StreakWubSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/wub");
            HighScoreSwooshSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/HighScoreSwoosh");
            UnlockedSoundEffect = content.Load<SoundEffect>(@"Audio/SoundEffects/UnlockSound");
            EngageHomingBall = content.Load<SoundEffect>(@"Audio/SoundEffects/engagehoming");
            HomingMissleBallShot = content.Load<SoundEffect>(@"Audio/SoundEffects/laser_loop_tail");
            ArcadeModeStreak = content.Load<SoundEffect>(@"Audio/SoundEffects/streak_up");
        }

        public static void LoadSongs(ContentManager content, GameSettings settings)
        {
            AmbientSpaceSong = content.Load<Song>(@"Audio/Music/IntroAmbientCreativeZero");
            MediaPlayer.Play(AmbientSpaceSong);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = (float)settings.MusicVolume / 10;
        }
    }
}
