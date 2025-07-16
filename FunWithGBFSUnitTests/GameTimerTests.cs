namespace FunWithGBFSUnitTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FunWithGBFS.Application.Game;
    using FunWithGBFS.Application.Game.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class GameTimerTests
    {
        private readonly Mock<ILogger<IGameTimer>> _loggerMock = new();
        private readonly GameSettings _settings = new() { GameDurationSeconds = 3 };

        [Fact]
        public async Task Timer_TicksAndExpires()
        {
            var timer = new GameTimer(_settings, _loggerMock.Object);

            var ticks = new List<int>();
            bool expired = false;

            timer.TimeTicked += t => ticks.Add(t);
            timer.TimeExpired += () => expired = true;

            await timer.StartAsync();

            Assert.Equal(new List<int> { 3, 2, 1 }, ticks);
            Assert.True(expired);
        }

        [Fact]
        public async Task Timer_StopsBeforeExpiration()
        {
            var timer = new GameTimer(_settings, _loggerMock.Object);

            bool expired = false;
            timer.TimeExpired += () => expired = true;

            var task = timer.StartAsync();

            await Task.Delay(1500); // Let it tick once
            timer.Stop();
            await Task.Delay(1000); // Ensure time for possible expired event

            Assert.False(expired);
            Assert.True(timer.RemainingTime < _settings.GameDurationSeconds);
        }

        [Fact]
        public async Task Timer_ResetsCorrectly()
        {
            var timer = new GameTimer(_settings, _loggerMock.Object);

            var task = timer.StartAsync();

            await Task.Delay(1500);
            timer.Reset();

            Assert.Equal(_settings.GameDurationSeconds, timer.RemainingTime);
        }

        [Fact]
        public async Task Timer_StartMultipleTimes_ShouldNotDoubleStart()
        {
            var timer = new GameTimer(_settings, _loggerMock.Object);
            int tickCount = 0;
            timer.TimeTicked += _ => tickCount++;

            var task1 = timer.StartAsync();
            var task2 = timer.StartAsync();

            await Task.WhenAll(task1, task2);

            Assert.InRange(tickCount, 3, 4); // Should tick 3 times, possibly 4 if timing is loose
        }
    }
}