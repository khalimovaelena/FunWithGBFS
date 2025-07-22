namespace FunWithGBFS.Application.Game
{
    //TODO: use interface to be able to change logic of calculating score in the future
    public class ScoreManager
    {
        public int Score { get; private set; }
        public int NumberOfWrongAnswers { get; private set; } = 0;

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

        public void AddWrongAnswer()
        {
            NumberOfWrongAnswers++;
        }
    }
}
