namespace SpoidaGamesArcadeLibrary.Globals
{
    public class PowerUp
    {
        public string PowerUpName { get; set; }
        public bool IsActive { get; set; }
        public double TimeRemaining { get; set; }

        public PowerUp(string name)
        {
            PowerUpName = name;
            IsActive = false;
        }
    }
}
