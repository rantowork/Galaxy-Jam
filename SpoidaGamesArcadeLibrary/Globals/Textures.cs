using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpoidaGamesArcadeLibrary.Globals
{
    public class Textures
    {
        //Textures
        public static Texture2D LineSprite { get; set; }

        public static Texture2D Backboard1 { get; set; }
        public static Texture2D Backboard1Glow { get; set; }
        public static Texture2D Backboard2 { get; set; }
        public static Texture2D Backboard2Glow { get; set; }
        public static Texture2D Backboard3 { get; set; }
        public static Texture2D Backboard3Glow { get; set; }
        public static Texture2D Backboard4 { get; set; }
        public static Texture2D Backboard4Glow { get; set; }
        public static Texture2D Backboard5 { get; set; }
        public static Texture2D Backboard5Glow { get; set; }

        public static Texture2D LeftRim1 { get; set; }
        public static Texture2D LeftRim1Glow { get; set; }
        public static Texture2D RightRim1 { get; set; }
        public static Texture2D RightRim1Glow { get; set; }
        public static Texture2D LeftRim2 { get; set; }
        public static Texture2D LeftRim2Glow { get; set; }
        public static Texture2D RightRim2 { get; set; }
        public static Texture2D RightRim2Glow { get; set; }
        public static Texture2D LeftRim3 { get; set; }
        public static Texture2D LeftRim3Glow { get; set; }
        public static Texture2D RightRim3 { get; set; }
        public static Texture2D RightRim3Glow { get; set; }
        public static Texture2D LeftRim4 { get; set; }
        public static Texture2D LeftRim4Glow { get; set; }
        public static Texture2D RightRim4 { get; set; }
        public static Texture2D RightRim4Glow { get; set; }
        public static Texture2D LeftRim5 { get; set; }
        public static Texture2D LeftRim5Glow { get; set; }
        public static Texture2D RightRim5 { get; set; }
        public static Texture2D RightRim5Glow { get; set; }

        public static Texture2D Hoop1 { get; set; }

        public static Texture2D Twopxsolidstar { get; set; }
        public static Texture2D Fourpxblurstar { get; set; }
        public static Texture2D Onepxsolidstar { get; set; }
        public static Texture2D Explosion { get; set; }

        public static Texture2D Cursor { get; set; }

        public static Texture2D MenuHull { get; set; }
        public static Texture2D ArrowKey { get; set; }
        public static Texture2D ArrowKeyHover { get; set; }

        public static Texture2D GalaxyJamText { get; set; }
        public static Texture2D GalaxyJamLogo { get; set; }

        public static void LoadTextures(ContentManager content)
        {
            //Title Screen
            ArrowKey = content.Load<Texture2D>(@"Textures/Interface/Arrow");
            ArrowKeyHover = content.Load<Texture2D>(@"Textures/Interface/ArrowHover");
            MenuHull = content.Load<Texture2D>(@"Textures/Interface/SelectionPanel");
            GalaxyJamText = content.Load<Texture2D>(@"Textures/Interface/GalaxyJamTitle");
            GalaxyJamLogo = content.Load<Texture2D>(@"Textures/GalaxyJamConcept");

            //Backboards
            Backboard1 = content.Load<Texture2D>(@"Textures/Backboard/RedOrangeBoard2");
            Backboard1Glow = content.Load<Texture2D>(@"Textures/Backboard/RedOrangeBoardContact2");
            Backboard2 = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumBackboard2");
            Backboard2Glow = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumBackboardContact2");
            Backboard3 = content.Load<Texture2D>(@"Textures/Backboard/TealLimeGreen2");
            Backboard3Glow = content.Load<Texture2D>(@"Textures/Backboard/TealLimeGreenContact2");
            Backboard4 = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRed2");
            Backboard4Glow = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedContact2");
            Backboard5 = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletBackBoard2");
            Backboard5Glow = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContact2");
            LeftRim1 = content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrange");
            LeftRim1Glow = content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeContact");
            RightRim1 = content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeFlip");
            RightRim1Glow = content.Load<Texture2D>(@"Textures/Backboard/RimBaseRedOrangeContactFlip");
            LeftRim2 = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumRegular");
            LeftRim2Glow = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumRegularContact");
            RightRim2 = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumFlip");
            RightRim2Glow = content.Load<Texture2D>(@"Textures/Backboard/PurplePlumContactFlip");
            LeftRim3 = content.Load<Texture2D>(@"Textures/Backboard/GreenLime");
            LeftRim3Glow = content.Load<Texture2D>(@"Textures/Backboard/GreenLimeConract");
            RightRim3 = content.Load<Texture2D>(@"Textures/Backboard/GreenLimeFlip");
            RightRim3Glow = content.Load<Texture2D>(@"Textures/Backboard/GreenLimeConractFlip");
            LeftRim4 = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRed3");
            LeftRim4Glow = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedContact3");
            RightRim4 = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedFlip3");
            RightRim4Glow = content.Load<Texture2D>(@"Textures/Backboard/DarkRedRedFlipContact3");
            LeftRim5 = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueViolet");
            LeftRim5Glow = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContact");
            RightRim5 = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletFlip");
            RightRim5Glow = content.Load<Texture2D>(@"Textures/Backboard/RoyalBlueVioletContactFlip");
            Hoop1 = content.Load<Texture2D>(@"Textures/Backboard/RedOrangeHoop");

            //Empty Sprite 1px
            LineSprite = content.Load<Texture2D>(@"Textures/LineSprite");

            //Stars
            Twopxsolidstar = content.Load<Texture2D>(@"Textures/2x2SolidStar");
            Fourpxblurstar = content.Load<Texture2D>(@"Textures/4x4BlurStar");
            Onepxsolidstar = content.Load<Texture2D>(@"Textures/1x1SolidStar");
            Explosion = content.Load<Texture2D>(@"Textures/Explosion");

            //Keyboard Curso
            Cursor = content.Load<Texture2D>(@"Textures/Cursor");
        }
    }
}
