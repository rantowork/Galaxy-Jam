using System.Collections.Generic;

namespace SpoidaGamesArcadeLibrary.Interface.GameOptions
{
    public class PlayerOptions
    {
        //TODO: Consider better object model for basketballs and music.  This one is really weak..  The basketball object can also have the physical object.  If i get the options page done tonight i'll add the object also.
        private Dictionary<Music, string> music = new Dictionary<Music, string>
                                                      {
                                                          {Music.SpaceLoop1, "SpaceLoop1"},
                                                          {Music.SpaceLoop2, "SpaceLoop2"},
                                                          {Music.BouncyLoop1, "BouncyLoop1"}
                                                      };


        private Music selectedMusic;
        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set { selectedMusic = value; }
        }

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        public PlayerOptions()
        {
            selectedMusic = Music.SpaceLoop1;
        }

        public string GetSelectedMusic()
        {
            string songName;
            if (music.TryGetValue(selectedMusic, out songName))
            {
                return songName;
            }
            return "SpaceLoop1";
        }

        public void SetSelectedMusic(Music songName)
        {
            SelectedMusic = songName;
        }
    }

   public enum Music
   {
       BouncyLoop1,
       SpaceLoop1,
       SpaceLoop2
   }
}
