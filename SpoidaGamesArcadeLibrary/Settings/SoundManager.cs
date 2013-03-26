using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SpoidaGamesArcadeLibrary.Settings
{
    public class SoundManager
    {
        public static void PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan)
        {
            if (!InterfaceOptions.AllSoundsMuted)
            {
                soundEffect.Play(volume, pitch, pan);
            }
        }

        public static void PlayBackgroundMusic(Song backgroundMusic, float volume)
        {
            if (!InterfaceOptions.BackgroundMusicStarted)
            {
                MediaPlayer.Volume = volume;
                MediaPlayer.Play(backgroundMusic);
                InterfaceOptions.BackgroundMusicStarted = true;
            }
        }

        public static void PauseBackgroundMusic()
        {
            if (!InterfaceOptions.BackgroundMusicMuted)
            {
                MediaPlayer.Pause();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
            else
            {
                MediaPlayer.Resume();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
        }

        public static void MuteSounds()
        {
            if (!InterfaceOptions.AllSoundsMuted)
            {
                if (!InterfaceOptions.BackgroundMusicMuted)
                {
                    MediaPlayer.Pause();
                    InterfaceOptions.BackgroundMusicMuted = true;
                }
                InterfaceOptions.AllSoundsMuted = !InterfaceOptions.AllSoundsMuted;
            }
            else
            {
                if (InterfaceOptions.BackgroundMusicMuted)
                {
                    MediaPlayer.Resume();
                    InterfaceOptions.BackgroundMusicMuted = false;
                }
                InterfaceOptions.AllSoundsMuted = !InterfaceOptions.AllSoundsMuted;
            }
        }
    }
}
