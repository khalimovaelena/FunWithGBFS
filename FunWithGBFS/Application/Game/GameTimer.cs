using FunWithGBFS.Application.Game.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunWithGBFS.Application.Game
{
    public class GameTimer : IGameTimer
    {
        private readonly int _durationInSeconds;
        private readonly ILogger<IGameTimer> _logger;
        private readonly object _lock = new();

        private int _remainingTime;
        private CancellationTokenSource _cts;
        private bool _isRunning;

        public int RemainingTime => Volatile.Read(ref _remainingTime);

        public event Action TimeExpired;
        public event Action<int> TimeTicked;

        public GameTimer(GameSettings gameSettings, ILogger<IGameTimer> logger)
        {
            _durationInSeconds = gameSettings.GameDurationSeconds;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _remainingTime = _durationInSeconds;
        }

        public async Task StartAsync()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return;
                }

                _isRunning = true;
                _cts = new CancellationTokenSource();
                Interlocked.Exchange(ref _remainingTime, _durationInSeconds);
            }

            try
            {
                while (RemainingTime > 0)
                {
                    TimeTicked?.Invoke(RemainingTime);

                    await Task.Delay(1000);

                    lock (_lock)
                    {
                        if (_cts == null || _cts.Token.IsCancellationRequested)
                        {
                            return;
                        }
                    }

                    Interlocked.Decrement(ref _remainingTime);
                }

                lock (_lock)
                {
                    if (_cts != null && !_cts.Token.IsCancellationRequested)
                    {
                        TimeExpired?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in timer: {ex.Message}");
            }
            finally
            {
                lock (_lock)
                {
                    _cts?.Dispose();
                    _cts = null;
                    _isRunning = false;
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                _isRunning = false;
            }
        }

        public void Reset()
        {
            Stop();
            Interlocked.Exchange(ref _remainingTime, _durationInSeconds);
        }
    }
}
