using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SpoidaGamesArcadeLibrary.Resources
{
    public class Music
    {
        public static Song LoadPersistentSong(string songPath, ContentManager content)
        {
            return content.Load<Song>(String.Format(@"{0}", songPath));
        }
    }
}
