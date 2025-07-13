using FunWithGBFS.Models;
using FunWithGBFS.Services.Game;
using FunWithGBFS.Services.GbfsAPI;
using FunWithGBFS.Services.Questions;
using FunWithGBFS.Services.Questions.Interfaces;
using FunWithGBFS.Services.Users;
using FunWithGBFS.Services.Users.Interfaces;

internal class Program
{
    //TODO: try-catch for all classes
    public static async Task Main(string[] args)
    {
        //1. Read providers
        //TODO: separate service
        
        //TODO: read providers from file, filepath is in appsettings
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

        //2. Register or login user
        IUserService userService = new FileUserService();
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
        var gameEngine = new GameEngine(questions, stations, numberOfQuestions: 5);
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