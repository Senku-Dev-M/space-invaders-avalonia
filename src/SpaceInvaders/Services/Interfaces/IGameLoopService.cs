namespace SpaceInvaders.Services.Interfaces;

public interface IGameLoopService : IDisposable
{
    bool IsRunning { get; }
    void Start(Action<double> update);
    void Stop();
}
