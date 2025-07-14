using FunWithGBFS;
using FunWithGBFS.Models;
using FunWithGBFS.Repository;
using FunWithGBFS.Repository.Interfaces;
using FunWithGBFS.Services.Game;
using FunWithGBFS.Services.GbfsAPI;
using FunWithGBFS.Services.Questions;
using FunWithGBFS.Services.Questions.Interfaces;
using FunWithGBFS.Services.Users;
using FunWithGBFS.Services.Users.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 0. Read configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var gameSettings = config.GetSection("GameSettings").Get<GameSettings>();
        var connectionString = config.GetConnectionString("DefaultConnection");

        // 1. Setup dependency injection
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton(gameSettings);
        services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(connectionString));

        // Add repository and service
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, DbUserService>();

        var serviceProvider = services.BuildServiceProvider();

        // 2. Ensure database is created
        var db = serviceProvider.GetRequiredService<GameDbContext>();
        var isDbCreated = db.Database.EnsureCreated(); // Creates DB and tables if not exist

        // 3. Fetch providers and stations
        var providers = ProvidersLoader.LoadProviders(gameSettings.ProvidersFile);
        var stations = new List<Station>();

        foreach (var provider in providers)
        {
            Console.WriteLine($"\nFetching data for {provider.Name}...");
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

        // 4. Register or login user
        var userService = serviceProvider.GetRequiredService<IUserService>();
        User? currentUser = null;

        while (currentUser == null)
        {
            Console.WriteLine("Choose an option for user:");
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

        // 5. Generate questions
        List<IQuestionGenerator> questions = new List<IQuestionGenerator>
    {
        new AverageBikeAvailabilityQuestionGenerator(),
        new MaxBikeStationQuestionGenerator()
    };

        var gameEngine = new GameEngine(questions, stations, gameSettings);
        var score = await gameEngine.RunGameAsync();

        // 6. Save result
        currentUser.Attempts.Add(new GameAttempt { Score = score });
        userService.SaveUser(currentUser);

        Console.WriteLine("\nPrevious attempts:");
        foreach (var attempt in currentUser.Attempts)
        {
            Console.WriteLine($"{attempt.Timestamp}: {attempt.Score} points");
        }

        Console.WriteLine("Game over. Thank you for playing!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
