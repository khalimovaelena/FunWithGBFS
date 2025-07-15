namespace FunWithGBFS.Application.Game
{
    public class GameSettings
    {
        public int NumberOfQuestions { get; set; }
        public int GameDurationSeconds { get; set; }

        public int InitialScore { get; set; }
        public int PointsPerCorrectAnswer { get; set; }
        public int PointsPerWrongAnswer { get; set; }

        public string ProvidersFile { get; set; } = "Config/providers.json";
    }
}
