using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Resources
{
    public class Fonts
    {
        public static SpriteFont LoadPersistentFont(string fontPath, ContentManager content)
        {
            return content.Load<SpriteFont>(String.Format(@"{0}", fontPath));
        }
    }
}
