using Microsoft.Xna.Framework.Audio;

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

        public static void PlayBackgroundMusic(Cue cueToPlay)
        {
            if (!InterfaceOptions.BackgroundMusicStarted)
            {
                cueToPlay.Play();
                InterfaceOptions.BackgroundMusicStarted = true;
            }
        }

        public static void PauseBackgroundMusic(Cue cueToPause)
        {
            if (!InterfaceOptions.BackgroundMusicMuted)
            {
                cueToPause.Pause();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
            else
            {
                cueToPause.Resume();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
        }

        public static void MuteSounds(Cue cueToPause)
        {
            if (!InterfaceOptions.AllSoundsMuted)
            {
                if (!InterfaceOptions.BackgroundMusicMuted)
                {
                    cueToPause.Pause();
                    InterfaceOptions.BackgroundMusicMuted = true;
                }
                InterfaceOptions.AllSoundsMuted = !InterfaceOptions.AllSoundsMuted;
            }
            else
            {
                if (InterfaceOptions.BackgroundMusicMuted)
                {
                    cueToPause.Resume();
                    InterfaceOptions.BackgroundMusicMuted = false;
                }
                InterfaceOptions.AllSoundsMuted = !InterfaceOptions.AllSoundsMuted;
            }
        }
    }
}
