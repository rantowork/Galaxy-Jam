using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace SpoidaGamesArcadeLibrary.Settings
{
    public class SoundManager
    {
        public static Dictionary<SongTypes,Cue> music = new Dictionary<SongTypes, Cue>();
        public static Dictionary<int, SongTypes> musicSelection = new Dictionary<int, SongTypes>();
        public static SoundBank soundBank;

        public static void PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan)
        {
            if (!InterfaceOptions.AllSoundsMuted)
            {
                soundEffect.Play(volume, pitch, pan);
            }
        }

        public static void PlayBackgroundMusic()
        {
            if (SelectedMusic.IsStopped)
            {
                SelectedMusic.Play();
            }
            else if (!SelectedMusic.IsPlaying)
            {
                SelectedMusic.Play();
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

        private static Cue selectedMusic;
        public static Cue SelectedMusic
        {
            get { return selectedMusic; }
            set { selectedMusic = value; }
        }

        public void SelectMusic(SongTypes type)
        {
            Cue selectedCue;
            if (music.TryGetValue(type, out selectedCue))
            {
                selectedMusic = selectedCue;
            }
            else
            {
                selectedMusic = music[0];
            }
        }

        public SoundManager(SoundBank bank  )
        {
            soundBank = bank;
            LoadMusic();
        }

        private static void LoadMusic()
        {
            music.Add(SongTypes.BouncyLoop1, soundBank.GetCue("BouncyLoop1"));
            music.Add(SongTypes.SpaceLoop1, soundBank.GetCue("SpaceLoop1"));
            music.Add(SongTypes.SpaceLoop2, soundBank.GetCue("SpaceLoop2"));
        }
    }

    public enum SongTypes
    {
        BouncyLoop1,
        SpaceLoop1,
        SpaceLoop2
    }
}
