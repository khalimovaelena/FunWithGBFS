namespace FunWithGBFS.Application.Game
{
    public class GameSettings
    {
        public int NumberOfQuestions { get; set; } = 5;
        public int NumberOfOptions { get; set; } = 4;
        public int GameDurationSeconds { get; set; } = 60;

        public int InitialScore { get; set; } = 50;
        public int PointsPerCorrectAnswer { get; set; } = 50;
        public int PointsPerWrongAnswer { get; set; } = 20;

        public string ProvidersFile { get; set; } = "Config/providers.json";
    }
}
