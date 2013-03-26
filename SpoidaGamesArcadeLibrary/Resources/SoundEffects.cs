using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace SpoidaGamesArcadeLibrary.Resources
{
    public class SoundEffects
    {
        public static SoundEffect LoadPersistentSoundEffect(string soundEffectPath, ContentManager content)
        {
            return content.Load<SoundEffect>(String.Format(@"{0}", soundEffectPath));
        }
    }
}
