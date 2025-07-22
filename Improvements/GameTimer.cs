public class GameTimer : IGameTimer
{
    private readonly int _durationInSeconds;
    private readonly ILogger<IGameTimer> _logger;
    private CancellationTokenSource _cts;

    private int _remainingTime;
    private int _isRunning; // 0 = false, 1 = true

    public event Action TimeExpired;
    public event Action<int> TimeTicked;

    public int RemainingTime => Interlocked.CompareExchange(ref _remainingTime, 0, 0);

    public bool IsRunning => Interlocked.CompareExchange(ref _isRunning, 0, 0) == 1;

    public GameTimer(GameSettings gameSettings, ILogger<IGameTimer> logger)
    {
        _durationInSeconds = gameSettings.GameDurationSeconds;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _remainingTime = _durationInSeconds;
    }

    public async Task StartAsync()
    {
        // Try to set _isRunning to 1. If already 1, return early.
        if (Interlocked.Exchange(ref _isRunning, 1) == 1)
            return;

        Interlocked.Exchange(ref _remainingTime, _durationInSeconds);
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        try
        {
            while (RemainingTime > 0)
            {
                TimeTicked?.Invoke(RemainingTime);

                await Task.Delay(1000, token); // auto-cancels if Stop() is called

                Interlocked.Decrement(ref _remainingTime);
            }

            TimeExpired?.Invoke();
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Game timer was cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in GameTimer.");
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
            Interlocked.Exchange(ref _isRunning, 0);
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        Interlocked.Exchange(ref _isRunning, 0);
    }

    public void Reset()
    {
        Stop();
        Interlocked.Exchange(ref _remainingTime, _durationInSeconds);
    }
}
