using FunWithGBFS.Application.Game.Interfaces;
using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Domain.Models;
using FunWithGBFS.Presentation.Interfaces;

namespace FunWithGBFS.Application.Game
{
    public class GameEngine
    {
        private readonly List<IQuestionGenerator> _questions;
        private readonly List<Station> _stations;
        private readonly List<Vehicle> _vehicles;
        private readonly ScoreManager _scoreManager;
        private readonly IGameTimer _gameTimer;
        private readonly GameSettings _gameSettings;
        public readonly IUserInteraction _userInteraction;
        private readonly User _user;

        private bool _timeExpired;
        private int _optionsCount;

        public GameEngine(
            List<IQuestionGenerator> questions, 
            List<Station> stations, 
            List<Vehicle> vehicles,
            GameSettings gameSettings,
            IUserInteraction userInteraction,
            IGameTimer timer,
            User user)
        {
            _questions = questions;
            _stations = stations;
            _vehicles = vehicles;
            _gameSettings = gameSettings;
            _userInteraction = userInteraction;
            _user = user ?? throw new ArgumentNullException(nameof(user));

            _optionsCount = _gameSettings.NumberOfOptions;
            _scoreManager = new ScoreManager(_gameSettings.InitialScore);
            _gameTimer = timer;

            // Subscribe to the timer's expiration event
            _gameTimer.TimeExpired += OnTimeExpired;
            //_gameTimer.TimeTicked += OnTimeTicked;
        }

        private void OnTimeExpired()
        {
            _timeExpired = true;
        }

        //TODO: use it if UI is not Console
        private void OnTimeTicked(int remainingTime)
        {
            _userInteraction.ShowMessage($"Time remaining: {remainingTime} seconds");
        }

        public async Task<int> RunGameAsync()
        {
            var timerTask = _gameTimer.StartAsync();
            int questionIndex = 0;
            var random = new Random();

            using var cts = new CancellationTokenSource();

            while (!_timeExpired && questionIndex < _gameSettings.NumberOfQuestions)
            {
                var question = GenerateRandomQuestion();

                if (question == null || question.Options.Count == 0)
                {
                    continue;
                }

                _userInteraction.ClearScreen();
                _userInteraction.ShowMessage($"Time remaining: {_gameTimer.RemainingTime} seconds");
                _userInteraction.ShowMessage($"Current Score: {_scoreManager.Score}\n");
                _userInteraction.ShowMessage($"Question {questionIndex + 1}: {question.Text}");

                for (int j = 0; j < question.Options.Count; j++)
                {
                    _userInteraction.ShowMessage($"{j + 1}. {question.Options[j]}");
                }

                _userInteraction.ShowMessage("Enter your answer (1, 2, 3, etc.):");

                var inputTask = ReadLineWithTimeout(cts.Token);
                var completedTask = await Task.WhenAny(inputTask, timerTask);

                if (_timeExpired || completedTask == timerTask)
                {
                    _userInteraction.ShowMessage("Game over: You ran out of time.");
                    break;
                }

                var answer = inputTask.Result;

                if (int.TryParse(answer, out var option))
                {
                    if (option - 1 == question.CorrectAnswerIndex)
                    {
                        _scoreManager.AddPoints(_gameSettings.PointsPerCorrectAnswer);
                    }
                    else
                    {
                        _scoreManager.SubtractPoints(_gameSettings.PointsPerWrongAnswer);
                        _scoreManager.AddWrongAnswer();
                    }
                }

                if (_scoreManager.NumberOfWrongAnswers >= 2)
                {
                    _userInteraction.ShowMessage("You have made too many wrong answers. Game over.");
                    _gameTimer.Stop(); // Stop the timer early
                    break;
                }

                if (_scoreManager.Score > 0)
                {
                    _user.Level++;
                }

                questionIndex++;
            }

            if (_user.Level > 0)
            {
                _userInteraction.ShowMessage("Congrats! You won the game!");
                _gameTimer.Stop(); // Stop the timer early
            }
            else
            {
                _userInteraction.ShowMessage("Sorry! You have not reached Level 1, you lost the game");
                _gameTimer.Stop(); // Stop the timer early
            }

            if (questionIndex >= _gameSettings.NumberOfQuestions && !_timeExpired)
            {
                _userInteraction.ShowMessage("Congrats! You answered all questions!");
                _gameTimer.Stop(); // Stop the timer early
            }

            await timerTask;

            _userInteraction.ShowMessage($"\nFinal Score: {_scoreManager.Score}");

            return _scoreManager.Score;
        }

        private Question GenerateRandomQuestion()
        {
            var random = new Random();
            var randomIndex = random.Next(_questions.Count);
            var selectedGenerator = _questions[randomIndex];

            if (selectedGenerator is IQuestionGenerator<Station> stationGenerator)
            {
                return stationGenerator.Generate(_stations, _optionsCount);
            }
            else if (selectedGenerator is IQuestionGenerator<Vehicle> vehicleGenerator)
            {
                return vehicleGenerator.Generate(_vehicles, _optionsCount);
            }
            //Add your new types of questions here

            return new Question { Text = "Random question" };
        }

        private async Task<string?> ReadLineWithTimeout(CancellationToken token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return _userInteraction.Ask("");
                }
                catch
                {
                    return null;
                }
            }, token);
        }
    }
}
