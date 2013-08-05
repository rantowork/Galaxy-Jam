namespace SpoidaGamesArcadeLibrary.Globals
{
    public class PowerUp
    {
        public string PowerUpName { get; set; }
        public bool IsActive { get; set; }
        public double TimeRemaining { get; set; }
        public int AvailableInventory { get; set; }

        public PowerUp(string name)
        {
            PowerUpName = name;
            if (PowerUpName == "Homing Ball")
            {
                IsActive = true;
                AvailableInventory = 3;
            }
            else
            {
                IsActive = false;
            }
        }
    }
}
