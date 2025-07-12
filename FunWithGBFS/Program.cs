using FunWithGBFS.Models;
using FunWithGBFS.Services.Game;
using FunWithGBFS.Services.GbfsAPI;
using FunWithGBFS.Services.Questions;

internal class Program
{
    //TODO: try-catch for all classes
    public static async Task Main(string[] args)
    {
        //TODO: appsettings
        var providers = new List<Provider>
            {
                new Provider("BlueBike_BE", "https://api.delijn.be/gbfs/gbfs.json"),
                new Provider("Check_Rotterdam", "https://api.ridecheck.app/gbfs/v3/rotterdam/gbfs.json"),
                new Provider("Felyx_Delft", "https://maas.zeus.cooltra.com/gbfs/delft/3.0/en/gbfs.json")
            };

        var stations = new List<Station>();

        foreach (var provider in providers)
        {
            Console.WriteLine($"\nFetching data for {provider.Name}..."); //TODO: use logger
            var providerStations = await GbfsFetcher.FetchStations(provider);
            Console.WriteLine($"Stations found: {providerStations.Count}");
            if (providerStations.Count > 0)
            {
                stations.AddRange(providerStations);
            }
            else
            {
                Console.WriteLine($"No stations found for {provider.Name}.");
            }
        }

        // Instantiate generators
        List<IQuestionGenerator> generators = new List<IQuestionGenerator>
        {
            new AverageBikeAvailabilityQuestionGenerator(),
            new MaxBikeStationQuestionGenerator()
        };

        var scoreManager = new ScoreManager();
        var gameTimer = new GameTimer(60); //TODO: appsetting

        gameTimer.TimeExpired += () =>
        {
            Console.WriteLine("\nTime's up! The game is over.");
            Environment.Exit(0); // End the game when time is up
        };

        // Score display update
        scoreManager.ScoreUpdated += score =>
        {
            Console.Clear();
            Console.WriteLine($"Current Score: {score}");
        };

        // Start the timer in the background
        var timerTask = gameTimer.StartAsync();

        // Game loop
        //TODO: separate classes for game logic
        Random random = new();
        for (int i = 0; i < 3; i++) //TODO: appsettings for number of questions
        {
            var generator = generators[random.Next(generators.Count)];
            var question = generator.Generate(stations);

            Console.Clear();
            Console.WriteLine($"Time remaining: {gameTimer.RemainingTime} seconds");
            Console.WriteLine($"Current Score: {scoreManager.Score}\n");
            Console.WriteLine($"Question {i + 1}: {question.Text}");

            for (int j = 0; j < question.Options.Count; j++)
            {
                Console.WriteLine($"{j + 1}. {question.Options[j]}");
            }

            // Prompt for user input and wait for response
            string answer = null;

            while (string.IsNullOrEmpty(answer))
            {
                Console.WriteLine("\nEnter your answer (1, 2, 3, etc.):");
                answer = Console.ReadLine();
            }

            // Check answer (this part can be customized based on how you validate answers)
            if (int.TryParse(answer, out var option) && option >= 1 && option <= question.Options.Count)
            {
                int correctAnswerIndex = question.CorrectAnswerIndex;
                if (option - 1 == correctAnswerIndex)
                {
                    scoreManager.AddPoints(50);  // Correct answer, add points //TODO: appsettings
                }
                else
                {
                    scoreManager.SubtractPoints(20);  // Wrong answer, subtract points //TODO: appsettings
                }
            }

            if (scoreManager.Score <= 0) // End game if the score is zero or negative
            {
                Console.WriteLine("Game Over: You lost all your points.");
                break;
            }
        }

        // Wait for the timer to finish if it hasn't already
        await timerTask;
    }
}