using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Questions
{
    public class VehicleStatsQuestionGenerator : IQuestionGenerator<Vehicle>
    {
        private readonly Random _random = new();
        private const string DisabledQuestionText = "Which provider has the most number of disabled vehicles currently?";
        private const string ReservedQuestionText = "Which provider has the most vehicles currently reserved?";

        public Question Generate(List<Vehicle> vehicles, int optionsCount)
        {
            if (vehicles == null || vehicles.Count == 0 || vehicles.Count < optionsCount)
            {
                return new Question
                {
                    Text = "No vehicle data available.",
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };
            }

            // 50/50 between reserved and disabled
            bool askAboutDisabled = _random.Next(2) == 0;

            return askAboutDisabled
                ? GenerateStatsQuestion(
                    vehicles,
                    v => v.IsDisabled,
                    DisabledQuestionText,
                    optionsCount)
                : GenerateStatsQuestion(
                    vehicles,
                    v => v.IsReserved,
                    ReservedQuestionText,
                    optionsCount);
        }

        public Question Generate(object input, int optionsCount)
        {
            return Generate((List<Vehicle>)input, optionsCount);
        }

        private Question GenerateStatsQuestion(
            List<Vehicle> vehicles,
            Func<Vehicle, bool> predicate,
            string questionText,
            int optionsCount)
        {
            var grouped = vehicles
                .Where(predicate)
                .GroupBy(v => v.ProviderName)
                .Select(g => new { Provider = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (grouped.Count == 0)
            {
                return new Question
                {
                    Text = questionText,
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };
            }

            var correct = grouped.First();

            var options = grouped
                .OrderBy(_ => _random.Next()) // Shuffle
                .Take(optionsCount)
                .Select(g => g.Provider)
                .Distinct()
                .ToList();

            // Ensure the correct answer is present
            if (!options.Contains(correct.Provider))
            {
                options[_random.Next(options.Count)] = correct.Provider;
            }

            var correctIndex = options.IndexOf(correct.Provider);

            return new Question
            {
                Text = questionText,
                Options = options,
                CorrectAnswerIndex = correctIndex
            };
        }
    }
}
