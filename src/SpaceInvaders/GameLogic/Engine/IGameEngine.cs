using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Game;

namespace SpaceInvaders.GameLogic.Engine;

public interface IGameEngine
{
    GameSession Session { get; }
    event EventHandler<GameResult>? GameEnded;
    void StartNewGame(string shipId);
    void Update(double deltaSeconds);
    void SetMoveLeft(bool isPressed);
    void SetMoveRight(bool isPressed);
    bool TryFire();
    void TogglePause();
    void Pause();
}
