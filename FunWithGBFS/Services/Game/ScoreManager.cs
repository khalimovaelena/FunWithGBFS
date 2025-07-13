using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.Game
{
    public class ScoreManager
    {
        public int Score { get; private set; }

        public event Action<int> ScoreUpdated; //TODO: do we need it?

        public ScoreManager(int initialScore)
        {
            Score = initialScore;
        }

        public void AddPoints(int points)
        {
            Score += points;
            ScoreUpdated?.Invoke(Score);
        }

        public void SubtractPoints(int points)
        {
            Score -= points;
            ScoreUpdated?.Invoke(Score);
        }
    }
}
