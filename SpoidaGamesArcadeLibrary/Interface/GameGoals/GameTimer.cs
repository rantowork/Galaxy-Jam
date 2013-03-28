using System;
using System.Diagnostics;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class GameTimer
    {
        private static Stopwatch stopWatch = new Stopwatch();
        private static string elapsedTime;

        public static void StartGameTimer()
        {
            stopWatch.Start();
        }

        public static void StopGameTimer()
        {
            stopWatch.Stop();
        }


        public static string GetElapsedGameTime()
        {
            TimeSpan span = stopWatch.Elapsed;
            TimeSpan elapsedSpan = new TimeSpan(0,0,2,0) - span;
            elapsedTime = String.Format("{0:00}:{1:00}", elapsedSpan.Minutes, elapsedSpan.Seconds);
            return elapsedTime;
        }

        public static TimeSpan GetElapsedTimeSpan()
        {
            return stopWatch.Elapsed;
        }
    }
}
