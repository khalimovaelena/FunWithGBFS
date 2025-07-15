using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Questions
{
    public sealed class MaxVehiclesStationQuestionGenerator: IQuestionGenerator<Station>
    {
        private readonly Random _random = new();

        public Question Generate(List<Station> stations, int optionsCount)
        {
            if (stations == null || stations.Count == 0 || stations.Count < optionsCount)
                return new Question
                {
                    Text = "No stations available to determine max bike station.",
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };

            var allCities = stations.Select(s => s.City).Distinct().ToList();
            var city = allCities[_random.Next(allCities.Count)];

            var topStation = stations.Where(s => s.City.Equals(city)).OrderByDescending(s => s.BikesAvailable).First();
            var options = new HashSet<string> { topStation.Name };

            while (options.Count < optionsCount)
            {
                var s = stations[_random.Next(stations.Count)].Name;
                options.Add(s);
            }

            var optionList = options.OrderBy(_ => _random.Next()).ToList();
            int correctIndex = optionList.IndexOf(topStation.Name);

            return new Question
            {
                Text = $"Which station in {city} has the most vehicles available?",
                Options = optionList,
                CorrectAnswerIndex = correctIndex
            };
        }

        public Question Generate(object input, int optionsCount)
        {
            return Generate((List<Station>)input, optionsCount);
        }
    }
}
