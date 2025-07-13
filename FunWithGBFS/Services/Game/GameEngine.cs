using FunWithGBFS.Models;
using FunWithGBFS.Services.Questions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.Game
{
    public class GameEngine
    {
        private readonly List<IQuestionGenerator> _questions;
        private readonly List<Station> _stations;
        private readonly ScoreManager _scoreManager;
        private readonly GameTimer _gameTimer;
        private readonly GameSettings _gameSettings;

        private bool _timeExpired;

        public GameEngine(List<IQuestionGenerator> questions, List<Station> stations, GameSettings gameSettings)
        {
            _questions = questions;
            _stations = stations;
            _gameSettings = gameSettings;

            _scoreManager = new ScoreManager(_gameSettings.InitialScore);
            _gameTimer = new GameTimer(_gameSettings.GameDurationSeconds);

            // Subscribe to the timer's expiration event
            _gameTimer.TimeExpired += OnTimeExpired;
        }

        private void OnTimeExpired()
        {
            _timeExpired = true;
        }

        public async Task<int> RunGameAsync()
        {
            var timerTask = _gameTimer.StartAsync();
            int questionIndex = 0;
            var random = new Random();

            while (!_timeExpired && questionIndex < _gameSettings.NumberOfQuestions)
            {
                var generator = _questions[random.Next(_questions.Count)];
                var question = generator.Generate(_stations);

                Console.Clear();
                Console.WriteLine($"Time remaining: {_gameTimer.RemainingTime} seconds");
                Console.WriteLine($"Current Score: {_scoreManager.Score}\n");
                Console.WriteLine($"Question {questionIndex + 1}: {question.Text}");

                for (int j = 0; j < question.Options.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {question.Options[j]}");
                }

                string answer = null;

                while (string.IsNullOrWhiteSpace(answer) && !_timeExpired)
                {
                    Console.WriteLine("Enter your answer (1, 2, 3, etc.):");
                    answer = Console.ReadLine();
                }

                // If timer expired while waiting for input
                if (_timeExpired)
                {
                    Console.WriteLine("Game over: you are out of time");
                    break;
                }

                if (int.TryParse(answer, out var option) && option >= 1 && option <= question.Options.Count)
                {
                    if (option - 1 == question.CorrectAnswerIndex)
                    {
                        _scoreManager.AddPoints(_gameSettings.PointsPerCorrectAnswer);
                    }
                    else
                    {
                        _scoreManager.SubtractPoints(_gameSettings.PointsPerWrongAnswer);
                    }
                }

                if (_scoreManager.Score <= 0)
                {
                    Console.WriteLine("Game Over: You lost all your points.");
                    _gameTimer.Stop(); // Stop timer early
                    break;
                }

                questionIndex++;
            }

            if (questionIndex >= _gameSettings.NumberOfQuestions && !_timeExpired)
            {
                Console.WriteLine("Congrats! You answered all questions!");
                _gameTimer.Stop(); // Stop timer early
            }

            await timerTask;

            Console.WriteLine($"\nFinal Score: {_scoreManager.Score}");

            return _scoreManager.Score;
        }
    }
}
