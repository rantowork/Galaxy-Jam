using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpoidaGamesArcadeLibrary.Effects.Environment;
using SpoidaGamesArcadeLibrary.Effects._2D;
using SpoidaGamesArcadeLibrary.Interface.GameGoals;
using SpoidaGamesArcadeLibrary.Interface.GameOptions;
using SpoidaGamesArcadeLibrary.Resources.Entities;
using SpoidaGamesArcadeLibrary.Settings;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class InterfaceSettings
    {
        public static bool DisplaySettingsSavedMessage { get; set; }
        public static short TitleScreenSelection { get; set; }

        public static HighScoreManager HighScoreManager { get; set; }
        public static ArcadeHighScoreManager ArcadeHighScoreManager { get; set; }
        public static GameSettings GameSettings { get; set; }
        public static BasketballManager BasketballManager { get; set; }
        public static readonly GoalManager GoalManager = new GoalManager(100, true, new Rectangle(85, 208, 76, 15));
        public static readonly PlayerOptions PlayerOptions = new PlayerOptions(); 

        public static MouseState PreviousMouseClick { get; set; }
        public static bool DownArrowHovered { get; set; }
        public static bool UpArrowHovered { get; set; }
        public static bool LeftArrowHovered { get; set; }
        public static bool RightArrowHovered { get; set; }
        public static int CurrentlySelectedBasketballKey { get; set; }
        public static int CurrentlySelectedSongKey { get; set; }
        public static bool NameToShort { get; set; }
        public static readonly StringBuilder PlayerName = new StringBuilder();

        public static short CurrentTutorialScreen { get; set; }

        public static Vector2 BasketballLocation { get; set; }
        public static Vector2 PointingAt { get; set; }
        public static float Force { get; set; }

        public static List<Texture2D> StarTextures = new List<Texture2D>();
        public static Starfield StarField { get; set; }
        
        public static void LoadEffects()
        {
            StarTextures.Add(Textures.Twopxsolidstar);
            StarTextures.Add(Textures.Fourpxblurstar);
            StarTextures.Add(Textures.Onepxsolidstar);
            StarField = new Starfield(1280, 720, 1000, StarTextures) { StarSpeedModifier = 1 };
        }
    }
}
