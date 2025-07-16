using FunWithGBFS.Application.Game;

namespace FunWithGBFSUnitTests
{
    public class ScoreManagerTests
    {
        [Fact]
        public void Constructor_SetsInitialScore()
        {
            var manager = new ScoreManager(10);
            Assert.Equal(10, manager.Score);
        }

        [Fact]
        public void AddPoints_IncreasesScore()
        {
            var manager = new ScoreManager(5);
            manager.AddPoints(7);
            Assert.Equal(12, manager.Score);
        }

        [Fact]
        public void SubtractPoints_DecreasesScore()
        {
            var manager = new ScoreManager(20);
            manager.SubtractPoints(8);
            Assert.Equal(12, manager.Score);
        }

        [Fact]
        public void AddPoints_RaisesScoreUpdatedEvent()
        {
            var manager = new ScoreManager(0);
            int updatedValue = -1;

            manager.ScoreUpdated += score => updatedValue = score;

            manager.AddPoints(5);

            Assert.Equal(5, updatedValue);
        }

        [Fact]
        public void SubtractPoints_RaisesScoreUpdatedEvent()
        {
            var manager = new ScoreManager(10);
            int updatedValue = -1;

            manager.ScoreUpdated += score => updatedValue = score;

            manager.SubtractPoints(3);

            Assert.Equal(7, updatedValue);
        }
    }

}
