using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Questions
{
    public class AverageBikeAvailabilityQuestionGenerator: IQuestionGenerator
    {
        private readonly Random _random = new();

        public Question Generate(List<Station> stations)
        {
            if (stations == null || stations.Count == 0)
                return new Question
                {
                    Text = "No station data available to calculate average availability.",
                    Options = new List<string> { "0", "0", "0", "0" },
                    CorrectAnswerIndex = 0
                };

            double average = stations.Average(s => s.BikesAvailable);
            int roundedAverage = (int)Math.Round(average);

            var options = GenerateOptions(roundedAverage);
            int correctIndex = options.IndexOf(roundedAverage.ToString());

            return new Question
            {
                Text = "What is the average number of bikes available per station?",
                Options = options,
                CorrectAnswerIndex = correctIndex
            };
        }

        private List<string> GenerateOptions(int correct)
        {
            var options = new HashSet<int> { correct };
            while (options.Count < 4)
            {
                int offset = _random.Next(1, 10);
                options.Add(correct + offset * (_random.Next(2) == 0 ? -1 : 1));
            }
            return options.Select(o => o.ToString()).OrderBy(_ => _random.Next()).ToList();
        }
    }
}
