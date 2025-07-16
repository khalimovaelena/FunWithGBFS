using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Questions
{
    public sealed class AverageVehicleAvailabilityQuestionGenerator: IQuestionGenerator<Station>
    {
        private readonly Random _random = new();
        private const int _maxOffset = 10; //TODO: config in appsettings

        public Question Generate(List<Station> stations, int optionsCount)
        {
            if (stations == null || stations.Count == 0 || stations.Count < optionsCount)
                return new Question
                {
                    Text = "No station data available to calculate average availability.",
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };

            var allCities = stations.Select(s => s.City).Distinct().ToList();
            var city = allCities[_random.Next(allCities.Count)];

            double average = stations.Where(s => s.City.Equals(city)).Average(s => s.BikesAvailable);
            int roundedAverage = Convert.ToInt32(Math.Round(average));

            var options = GenerateOptions(roundedAverage, optionsCount);
            int correctIndex = options.IndexOf(roundedAverage.ToString());

            return new Question
            {
                Text = $"What is the average number of vehicles available per station in {city}?",
                Options = options,
                CorrectAnswerIndex = correctIndex
            };
        }

        private List<string> GenerateOptions(int correct, int optionsCount)
        {
            var options = new HashSet<int> { correct };

            while (options.Count < optionsCount)
            {
                int offset = _random.Next(1, _maxOffset + 1); // 1 to maxOffset
                bool add = _random.NextDouble() < 0.5;// Randomly decide to add or subtract the offset
                int candidate = add ? correct + offset : correct - offset;

                if (candidate < 0)
                {
                    continue; // Avoid negative bike counts
                }

                options.Add(candidate);
            }

            return options
                .OrderBy(_ => _random.Next())
                .Select(o => o.ToString())
                .ToList();
        }

        public Question Generate(object input, int optionsCount)
        {
            return Generate((List<Station>) input, optionsCount);
        }
    }
}
