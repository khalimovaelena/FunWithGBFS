using FunWithGBFS.Application.Questions.Interfaces;
using FunWithGBFS.Core.Models;
using FunWithGBFS.Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Game
{
    public class GameEngine
    {
        private readonly List<IQuestionGenerator> _questions;
        private readonly List<Station> _stations;
        private readonly ScoreManager _scoreManager;
        private readonly GameTimer _gameTimer;
        private readonly GameSettings _gameSettings;
        public readonly IUserInteraction _userInteraction;

        private bool _timeExpired;

        public GameEngine(
            List<IQuestionGenerator> questions, 
            List<Station> stations, 
            GameSettings gameSettings,
            IUserInteraction userInteraction)
        {
            _questions = questions;
            _stations = stations;
            _gameSettings = gameSettings;
            _userInteraction = userInteraction;

            _scoreManager = new ScoreManager(_gameSettings.InitialScore);
            _gameTimer = new GameTimer(_gameSettings.GameDurationSeconds, _userInteraction);

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

            using var cts = new CancellationTokenSource();

            while (!_timeExpired && questionIndex < _gameSettings.NumberOfQuestions)
            {
                var generator = _questions[random.Next(_questions.Count)];
                var question = generator.Generate(_stations);

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
                    }
                }

                if (_scoreManager.Score <= 0)
                {
                    _userInteraction.ShowMessage("Game Over: You lost all your points.");
                    _gameTimer.Stop(); // Stop the timer early
                    break;
                }

                questionIndex++;
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
