using FunWithGBFS;

internal class Program
{
    public static async Task Main(string[] args)
    {
        //TODO: appsettings
        var providers = new List<Provider>
            {
                new Provider("BlueBike_BE", "https://api.delijn.be/gbfs/gbfs.json"),
                new Provider("Check_Rotterdam", "https://api.ridecheck.app/gbfs/v3/rotterdam/gbfs.json"),
                new Provider("Felyx_Delft", "https://maas.zeus.cooltra.com/gbfs/delft/3.0/en/gbfs.json")
            };

        foreach (var provider in providers)
        {
            Console.WriteLine($"\nFetching data for {provider.City}...");
            var stations = await GbfsFetcher.FetchGbfsData(provider);
            Console.WriteLine($"Stations found: {stations.Count}");
        }
    }
}