using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class HighScoreManager
    {
        private List<HighScore> highScores = new List<HighScore>();
        public List<HighScore> HighScores
        {
            get { return highScores; }
            set { highScores = value; }
        }

        public string EncodeHighScores(string scores)
        {
            byte[] toBytes = Encoding.ASCII.GetBytes(scores);
            return Convert.ToBase64String(toBytes);
        }

        public string DecodeHighScores(string encodedScores)
        {
            byte[] data = Convert.FromBase64String(encodedScores);
            return Encoding.ASCII.GetString(data);
        }

        [Serializable]
        public struct HighScoreData
        {
            public string[] playerName;
            public double[] score;
            public int[] streak;

            public int count;

            public HighScoreData(int count)
            {
                playerName = new string[count];
                score = new double[count];
                streak = new int[count];

                this.count = count;
            }
        }

        public static void SaveHighScores(HighScoreData data, string filename)
        {
            // Get the path of the save game
            string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            // Open the file, creating it if necessary
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            try
            {
                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                serializer.Serialize(stream, data);
            }
            finally
            {
                // Close the file
                stream.Close();
            }
        }

        public static HighScoreData LoadHighScores(string filename)
        {
            HighScoreData data;

            // Get the path of the save game
            string fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            // Open the file
            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
                data = (HighScoreData)serializer.Deserialize(stream);
            }
            finally
            {
                // Close the file
                stream.Close();
            }

            return (data);
        }

        public static void SaveHighScore(double score, string highScoreFileName, string playerName, int streak)
        {
            // Create the data to save
            HighScoreData data = LoadHighScores(highScoreFileName);

            int scoreIndex = -1;
            for (int i = 0; i < data.count; i++)
            {
                if (score > data.score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.count - 1; i > scoreIndex; i--)
                {
                    data.playerName[i] = data.playerName[i - 1];
                    data.score[i] = data.score[i - 1];
                    data.streak[i] = data.streak[i - 1];
                }

                data.playerName[scoreIndex] = playerName; //Retrieve User Name Here
                data.score[scoreIndex] = score;
                data.streak[scoreIndex] = streak;

                SaveHighScores(data, highScoreFileName);
            }
        }
    }
}
