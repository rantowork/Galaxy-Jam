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

        public static void PauseBackgroundMusic()
        {
            if (!InterfaceOptions.BackgroundMusicMuted)
            {
                selectedMusic.Pause();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
            else
            {
                selectedMusic.Resume();
                InterfaceOptions.BackgroundMusicMuted = !InterfaceOptions.BackgroundMusicMuted;
            }
        }

        public static void MuteSounds()
        {
            if (!InterfaceOptions.AllSoundsMuted)
            {
                if (!InterfaceOptions.BackgroundMusicMuted)
                {
                    selectedMusic.Pause();
                    InterfaceOptions.BackgroundMusicMuted = true;
                }
                InterfaceOptions.AllSoundsMuted = !InterfaceOptions.AllSoundsMuted;
            }
            else
            {
                if (InterfaceOptions.BackgroundMusicMuted)
                {
                    selectedMusic.Resume();
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
            music.Add(SongTypes.Primary1, soundBank.GetCue("Primary1"));
            music.Add(SongTypes.Primary2, soundBank.GetCue("Primary2"));
            music.Add(SongTypes.DeepSpace1, soundBank.GetCue("DeepSpace1"));
            music.Add(SongTypes.SpaceLoop1, soundBank.GetCue("SpaceLoop2"));
        }
    }

    public enum SongTypes
    {
        Primary1,
        Primary2,
        DeepSpace1,
        SpaceLoop1,
        
    }
}
