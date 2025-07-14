using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Game
{
    public class GameTimer
    {
        private readonly int _durationInSeconds;
        public int RemainingTime { get; private set; }
        private readonly CancellationTokenSource _cts = new();

        public event Action TimeExpired;

        public GameTimer(int durationInSeconds)
        {
            _durationInSeconds = durationInSeconds;
            RemainingTime = durationInSeconds;  // Initialize the remaining time with the duration
        }

        public async Task StartAsync()
        {
            try
            {
                while (RemainingTime > 0)
                {
                    if (_cts.Token.IsCancellationRequested)
                        return;

                    await Task.Delay(1000); // No cancellation token — we check manually
                    RemainingTime--;
                }

                if (!_cts.Token.IsCancellationRequested)
                {
                    TimeExpired?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in timer: {ex.Message}");
            }
        }

        public void Stop()
        {
            _cts.Cancel();  // Manually stop the timer
        }

        public void Reset()
        {
            RemainingTime = _durationInSeconds;  // Reset the timer to the initial duration
        }
    }
}
