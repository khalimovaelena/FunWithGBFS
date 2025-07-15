using FunWithGBFS.Presentation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Game
{
    public class GameTimer
    {
        //TODO: add flag that timer is running to prevent starting the same timer multiple times
        private readonly int _durationInSeconds;
        private readonly IUserInteraction _userInteraction;

        private int _remainingTime;
        public int RemainingTime => Volatile.Read(ref _remainingTime);
        private CancellationTokenSource _cts;

        public event Action TimeExpired;
        //TODO: add event TimeTicked so UI can update the remaining time on display

        public GameTimer(int durationInSeconds, IUserInteraction userInteraction)
        {
            _durationInSeconds = durationInSeconds;
            _userInteraction = userInteraction ?? throw new ArgumentNullException(nameof(userInteraction));

            _remainingTime = durationInSeconds;  // Initialize the remaining time with the duration
        }

        public async Task StartAsync()
        {
            try
            {
                if (_cts == null)
                { 
                    _cts = new CancellationTokenSource();
                }

                if (_remainingTime <= 0)
                {
                    Interlocked.Exchange(ref _remainingTime, _durationInSeconds);
                }

                while (_remainingTime > 0)
                {
                    if (_cts.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    // No cancellation token in Delay — we check manually to avoid throwing an exception when the timer is cancelled, because it's normal behavior and not and exception
                    await Task.Delay(1000);
                    Interlocked.Decrement(ref _remainingTime);
                }

                if (!_cts.Token.IsCancellationRequested)
                {
                    TimeExpired?.Invoke();
                }
            }
            catch (Exception ex)
            {
                _userInteraction.ShowError($"Error in timer: {ex.Message}");
            }
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Reset()
        {
            Stop();
            _cts = new CancellationTokenSource();
            Interlocked.Exchange(ref _remainingTime, _durationInSeconds);  // Reset the timer to the initial duration
        }
    }
}
