using FunWithGBFS;
using FunWithGBFS.Models;
using FunWithGBFS.Services.Game;
using FunWithGBFS.Services.GbfsAPI;
using FunWithGBFS.Services.Questions;
using FunWithGBFS.Services.Questions.Interfaces;
using FunWithGBFS.Services.Users;
using FunWithGBFS.Services.Users.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class Program
{
    //TODO: try-catch for all classes
    public static async Task Main(string[] args)
    {
        //0. Read configuration + initiate logger
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Use the service provider to get the configuration
        var gameSettings = config.GetSection("GameSettings").Get<GameSettings>();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Game starting...");

        //1. Read providers
        var providers = ProvidersLoader.LoadProviders(gameSettings.ProvidersFile);

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

        //2. Register or login user
        IUserService userService = new FileUserService(gameSettings.UsersFile);
        User? currentUser = null;

        while (currentUser == null)
        {
            Console.WriteLine("1. Register\n2. Login");
            var choice = Console.ReadLine();

            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            try
            {
                currentUser = choice == "1"
                    ? userService.Register(username!, password!)
                    : userService.Login(username!, password!);

                if (currentUser == null)
                {
                    Console.WriteLine("Login failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine($"\nWelcome, {currentUser.Username}!");
        Console.WriteLine("Starting game...\n");

        //3. Generate questions
        //TODO: separate service
        List<IQuestionGenerator> questions = new List<IQuestionGenerator>
        {
            new AverageBikeAvailabilityQuestionGenerator(),
            new MaxBikeStationQuestionGenerator()
        };

        // Game loop
        var gameEngine = new GameEngine(questions, stations, gameSettings);
        var score = await gameEngine.RunGameAsync();

        // Save result
        currentUser.Attempts.Add(new GameAttempt { Score = score });
        userService.SaveUser(currentUser);

        Console.WriteLine("\nPrevious attempts:");
        foreach (var attempt in currentUser.Attempts)
        {
            Console.WriteLine($"{attempt.Timestamp}: {attempt.Score} points");
        }
    }
}