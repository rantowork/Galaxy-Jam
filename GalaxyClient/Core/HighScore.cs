using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GalaxyClient.Core
{
    public class HighScore
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
    }

    internal class HighScoreCollection : ObservableCollection<HighScore>
    {
        public void CopyFrom(IEnumerable<HighScore> highScores)
        {
            Items.Clear();
            foreach (var score in highScores)
            {
                Items.Add(score);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
