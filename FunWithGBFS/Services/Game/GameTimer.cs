using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Services.Game
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
            while (RemainingTime > 0)
            {
                await Task.Delay(1000);  // Wait for 1 second
                RemainingTime--;  // Decrease remaining time by 1 second

                // Stop if the game timer is manually canceled
                if (_cts.Token.IsCancellationRequested)
                {
                    break;
                }
            }

            // When time expires, trigger the event to notify the game logic
            TimeExpired?.Invoke();
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
