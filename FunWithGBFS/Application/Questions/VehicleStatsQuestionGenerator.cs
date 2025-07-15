using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Questions
{
    public class VehicleStatsQuestionGenerator : IQuestionGenerator<Vehicle>
    {
        private readonly Random _random = new();

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

            if (askAboutDisabled)
            {
                return GenerateDisabledVehicleQuestion(vehicles);
            }
            else
            {
                return GenerateReservedVehicleQuestion(vehicles);
            }
        }

        public Question Generate(object input, int optionsCount)
        {
            return Generate((List<Vehicle>)input, optionsCount);
        }

        private Question GenerateDisabledVehicleQuestion(List<Vehicle> vehicles)
        {
            const string questionDisabledText = "Which provider has the most number of disabled vehicles currently?";

            var grouped = vehicles
                .Where(v => v.IsDisabled)
                .GroupBy(v => v.ProviderName)
                .Select(g => new { Provider = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (grouped.Count == 0)
            {
                return new Question
                {
                    Text = questionDisabledText,
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };
            }

            var correct = grouped.First();

            var options = grouped
                .OrderBy(_ => _random.Next()) // Shuffle before taking
                .Take(3)
                .Select(g => g.Provider)
                .ToList();

            if (!options.Contains(correct.Provider))
            {
                options[_random.Next(options.Count)] = correct.Provider;
            }

            var correctIndex = options.IndexOf(correct.Provider);

            return new Question
            {
                Text = questionDisabledText,
                Options = options,
                CorrectAnswerIndex = correctIndex
            };
        }

        private Question GenerateReservedVehicleQuestion(List<Vehicle> vehicles)
        {
            const string questionReservedText = "Which provider has the most vehicles currently reserved?";
            var grouped = vehicles
                .Where(v => v.IsReserved)
                .GroupBy(v => v.ProviderName)
                .Select(g => new { Provider = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            if (grouped.Count == 0)
            {
                return new Question
                {
                    Text = questionReservedText,
                    Options = new List<string> { "None" },
                    CorrectAnswerIndex = 0
                };
            }

            var correct = grouped.First();

            var options = grouped
                .OrderBy(_ => _random.Next())
                .Take(3)
                .Select(g => g.Provider)
                .ToList();

            if (!options.Contains(correct.Provider))
            {
                options[_random.Next(options.Count)] = correct.Provider;
            }

            var correctIndex = options.IndexOf(correct.Provider);

            return new Question
            {
                Text = questionReservedText,
                Options = options,
                CorrectAnswerIndex = correctIndex
            };
        }
    }
}
