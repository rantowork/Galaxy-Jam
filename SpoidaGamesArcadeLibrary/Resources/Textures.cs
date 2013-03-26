using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Resources
{
    public class Textures
    {
        public static Texture2D LoadPersistentTexture(string contentPath, ContentManager content)
        {
            return content.Load<Texture2D>(string.Format(@"{0}", contentPath));
        }
    }
}
