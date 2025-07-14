using FunWithGBFS.Application.Game;
using FunWithGBFS.Application.Questions;
using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Application.Stations.Interfaces;
using FunWithGBFS.Application.Users;
using FunWithGBFS.Application.Users.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Infrastructure.Gbfs;
using FunWithGBFS.Infrastructure.Http;
using FunWithGBFS.Infrastructure.Http.Interfaces;
using FunWithGBFS.Persistence.Context;
using FunWithGBFS.Persistence.Repository;
using FunWithGBFS.Persistence.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 0. Read configuration
        var config = new ConfigurationBuilder()
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var gameSettings = config.GetSection("GameSettings").Get<GameSettings>();
        var connectionString = config.GetConnectionString("DefaultConnection");

        // 1. Setup dependency injection
        var services = new ServiceCollection();

        services.AddLogging(builder => builder.AddConsole());
        services.AddSingleton(gameSettings);

        services.AddDbContext<GameDbContext>(options =>
            options.UseSqlite(connectionString));

        // Add repositories and services
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, DbUserService>();
        services.AddScoped<IHttpJsonFetcher, HttpJsonFetcher>();
        services.AddScoped<IStationDataMapper, GbfsStationDataMapper>();
        services.AddScoped<IStationProvider, GbfsStationProvider>();
        services.AddHttpClient(); // Enables IHttpClientFactory

        var serviceProvider = services.BuildServiceProvider();

        // 2. Ensure database is created
        var db = serviceProvider.GetRequiredService<GameDbContext>();
        var isDbCreated = db.Database.EnsureCreated();

        // 3. Load providers from JSON and fetch stations via IStationProvider
        var stationProvider = serviceProvider.GetRequiredService<IStationProvider>();
        var providers = ProvidersLoader.LoadProviders(gameSettings.ProvidersFile);

        var stations = new List<Station>();
        foreach (var provider in providers)
        {
            Console.WriteLine($"\nFetching data for {provider.Name}...");
            var providerStations = await stationProvider.GetStationsAsync(provider);

            Console.WriteLine($"Stations found: {providerStations.Count}");

            if (providerStations.Count > 0)
                stations.AddRange(providerStations);
            else
                Console.WriteLine($"No stations found for {provider.Name}.");
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
                    Console.WriteLine("Login failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine($"\nWelcome, {currentUser.Username}!");
        Console.WriteLine("Starting game...\n");

        // 5. Generate questions
        var questions = new List<IQuestionGenerator>
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
            Console.WriteLine($"{attempt.Timestamp}: {attempt.Score} points");

        Console.WriteLine("Game over. Thank you for playing!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
