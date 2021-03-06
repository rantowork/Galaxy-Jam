﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SpoidaGamesArcadeLibrary.Interface.GameGoals
{
    public class ArcadeHighScoreManager
    {
        public List<HighScore> HighScores { get; set; }

        public string HighScoreFilePath { get; set; }

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

        public void LoadHighScoresFromDisk()
        {
            using (FileStream fileStream = File.Open(HighScoreFilePath, FileMode.Open))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string encryptedScoreData = streamReader.ReadToEnd();
                    string decryptedScoreData = DecodeHighScores(encryptedScoreData);
                    List<HighScore> scores = serializer.Deserialize<List<HighScore>>(decryptedScoreData);
                    HighScores = scores.OrderByDescending(o => o.PlayerScore).ToList();
                }
            }
        }

        private void SaveHighScoresToDisk()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(HighScores);
            string encryptedJson = EncodeHighScores(json);

            using (FileStream fileStream = File.Create(HighScoreFilePath))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(encryptedJson);
                }
            }
        }

        public void SaveHighScore(string name, double score, int streak, int multiplier)
        {
            HighScore playerScore = new HighScore(name, score, streak, multiplier);
            if (HighScores.Count < 10)
            {
                HighScores.Add(playerScore);
                HighScores = HighScores.OrderByDescending(o => o.PlayerScore).ToList();
                SaveHighScoresToDisk();
            }
            else
            {
                HighScores.Add(playerScore);
                HighScores = HighScores.OrderByDescending(o => o.PlayerScore).ToList();
                HighScores.RemoveAt(10);
                SaveHighScoresToDisk();
            }
        }

        public ArcadeHighScoreManager(string highScoreFilePath)
        {
            HighScores = new List<HighScore>();
            HighScoreFilePath = highScoreFilePath;
        }

        public double BestScore()
        {
            return HighScores[0].PlayerScore;
        }

        public bool LockedBasketballSelection { get; set; }

        public bool CanChangeBasketballSelection { get; set; }
    }
}
