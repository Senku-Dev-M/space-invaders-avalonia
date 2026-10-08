using System.Diagnostics;
using Avalonia.Threading;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.Services;

public sealed class DispatcherGameLoopService : IGameLoopService
{
    private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private Action<double>? _update;
    private TimeSpan _previousElapsed;

    public DispatcherGameLoopService()
    {
        _timer.Tick += OnTick;
    }

    public bool IsRunning
    {
        get { return _timer.IsEnabled; }
    }

    public void Start(Action<double> update)
    {
        ArgumentNullException.ThrowIfNull(update);
        _update = update;
        _previousElapsed = TimeSpan.Zero;
        _stopwatch.Restart();
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
        _stopwatch.Stop();
    }

    public void Dispose()
    {
        Stop();
        _timer.Tick -= OnTick;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var elapsed = _stopwatch.Elapsed;
        var delta = (elapsed - _previousElapsed).TotalSeconds;
        _previousElapsed = elapsed;
        if (_update != null)
        {
            _update(delta);
        }
    }
}
