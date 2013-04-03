using System.Collections.Generic;

namespace SpoidaGamesArcadeLibrary.Interface.GameOptions
{
    public class PlayerOptions
    {
        //TODO: Consider better object model for basketballs and music.  This one is really weak..  The basketball object can also have the physical object.  If i get the options page done tonight i'll add the object also.
        private Dictionary<BasketBalls, string> basketballs = new Dictionary<BasketBalls, string>
                                                                  {
                                                                      {BasketBalls.RedGlowBall, "RedGlowBall"},
                                                                      {BasketBalls.GreenGlowBall, "GreenGlowBall"},
                                                                      {BasketBalls.YellowGlowBall, "YellowGlowBall"}
                                                                  };

        private Dictionary<Music, string> music = new Dictionary<Music, string>
                                                      {
                                                          {Music.SpaceLoop1, "SpaceLoop1"},
                                                          {Music.SpaceLoop2, "SpaceLoop2"},
                                                          {Music.BouncyLoop1, "BouncyLoop1"}
                                                      };
        
        private BasketBalls selectedBasketBall;
        public BasketBalls SelectedBasketBall
        {
            get { return selectedBasketBall; }
            set { selectedBasketBall = value; }
        }

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
            selectedBasketBall = BasketBalls.RedGlowBall;
            selectedMusic = Music.SpaceLoop1;
        }

        public string GetSelectedBasketball()
        {
            string basketball;
            if (basketballs.TryGetValue(selectedBasketBall, out basketball))
            {
                return basketball;
            }
            return "RedGlowBall";
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

        public void SetSelectedBasketball(BasketBalls ball)
        {
            SelectedBasketBall = ball;
        }

        public void SetSelectedMusic(Music songName)
        {
            SelectedMusic = songName;
        }
    }

    public enum BasketBalls
    {
        RedGlowBall,
        GreenGlowBall,
        YellowGlowBall
    }

   public enum Music
   {
       BouncyLoop1,
       SpaceLoop1,
       SpaceLoop2
   }
}
