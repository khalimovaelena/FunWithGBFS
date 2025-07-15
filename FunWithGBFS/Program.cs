using FunWithGBFS.Application.Game;
using FunWithGBFS.Application.Questions;
using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Application.Stations;
using FunWithGBFS.Application.Users;
using FunWithGBFS.Application.Users.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Infrastructure.Gbfs;
using FunWithGBFS.Persistence.Context;
using FunWithGBFS.Presentation.Interfaces;
using FunWithGBFS.Startup;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Load configuration
        var config = AppConfigurator.LoadConfiguration();
        var serviceProvider = ServiceConfigurator.ConfigureServices(config);

        // 2. Ensure database exists
        var db = serviceProvider.GetRequiredService<GameDbContext>();//TODO: separate class
        db.Database.EnsureCreated();

        // 3. Load providers & stations
        var stationFetch = serviceProvider.GetRequiredService<StationFetchService>();
        var providers = ProvidersLoader.LoadProviders(config["GameSettings:ProvidersFile"]); //TODO: separate service
        var stations = await stationFetch.LoadStationsAsync(providers);

        // 4. User login/registration
        var userSession = serviceProvider.GetRequiredService<UserSessionService>();
        var interaction = serviceProvider.GetRequiredService<IUserInteraction>();
        var user = userSession.GetOrCreateUser();

        interaction.ShowMessage($"\nWelcome, {user.Username}!\nStarting game...\n");

        // 5. Run game
        var questions = new List<IQuestionGenerator>
        {
            new AverageBikeAvailabilityQuestionGenerator(),
            new MaxBikeStationQuestionGenerator()
        };

        var gameSettings = serviceProvider.GetRequiredService<GameSettings>();
        var game = new GameEngine(questions, stations, gameSettings, interaction);
        var score = await game.RunGameAsync();

        // 6. Save results
        user.Attempts.Add(new GameAttempt { Score = score });
        var userService = serviceProvider.GetRequiredService<IUserService>();
        userService.SaveUser(user);

        interaction.ShowMessage("\nPrevious attempts:");
        foreach (var attempt in user.Attempts)
            interaction.ShowMessage($"{attempt.Timestamp}: {attempt.Score} points");

        interaction.ShowMessage("Game over. Thank you for playing!");
        interaction.Pause();
    }
}
