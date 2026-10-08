namespace SpaceInvaders.Models.Game;

public sealed class GameResult : EventArgs
{
    public GameResult(int score, int wave, string shipId)
    {
        Score = score;
        Wave = wave;
        ShipId = shipId;
    }

    public int Score { get; }
    public int Wave { get; }
    public string ShipId { get; }
}
