using System;
using System.Diagnostics;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class GameTimer
    {
        private static readonly Stopwatch s_stopWatch = new Stopwatch();
        private static string s_elapsedTime;
        public static TimeSpan GameTime { get; set; }

        public static void StartGameTimer()
        {
            s_stopWatch.Start();
        }

        public static void StopGameTimer()
        {
            s_stopWatch.Stop();
        }


        public static string GetElapsedGameTime()
        {
            TimeSpan span = s_stopWatch.Elapsed;
            TimeSpan elapsedSpan = GameTime - span;
            s_elapsedTime = String.Format("{0:00}:{1:00}", elapsedSpan.Minutes, elapsedSpan.Seconds);
            return s_elapsedTime;
        }

        public static TimeSpan GetElapsedTimeSpan()
        {
            return s_stopWatch.Elapsed;
        }

        public static void ResetTimer()
        {
            s_stopWatch.Reset();
            
        }
    }
}
