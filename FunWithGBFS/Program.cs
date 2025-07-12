using FunWithGBFS.Models;
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

        // Instantiate generators
        List<IQuestionGenerator> generators = new List<IQuestionGenerator>
        {
            new AverageBikeAvailabilityQuestionGenerator(),
            new MaxBikeStationQuestionGenerator()
        };

        // Randomize generator usage
        Random random = new();
        for (int i = 0; i < 3; i++)
        {
            var generator = generators[random.Next(generators.Count)];
            var question = generator.Generate(stations);

            Console.WriteLine($"\nQuestion {i + 1}: {question.Text}");
            for (int j = 0; j < question.Options.Count; j++)
            {
                Console.WriteLine($"{j + 1}. {question.Options[j]}");
            }
        }
    }
}