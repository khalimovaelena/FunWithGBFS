using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Questions
{
    public sealed class MaxBikeStationQuestionGenerator: IQuestionGenerator
    {
        private readonly Random _random = new();

        public Question Generate(List<Station> stations, int optionsCount)
        {
            if (stations == null || stations.Count == 0 || stations.Count < optionsCount)
                return new Question
                {
                    Text = "No stations available to determine max bike station.",
                    Options = new List<string> { "None", "None", "None", "None" },
                    CorrectAnswerIndex = 0
                };

            var topStation = stations.OrderByDescending(s => s.BikesAvailable).First();
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
                Text = "Which station has the most bikes available?",
                Options = optionList,
                CorrectAnswerIndex = correctIndex
            };
        }
    }
}
