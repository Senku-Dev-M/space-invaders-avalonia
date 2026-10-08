namespace SpaceInvaders.Models.Entities;

public sealed class ScoreEntry
{
    public ScoreEntry(string playerName, int score, int wave, DateTimeOffset playedAt)
    {
        PlayerName = playerName;
        Score = score;
        Wave = wave;
        PlayedAt = playedAt;
    }

    public string PlayerName { get; }
    public int Score { get; }
    public int Wave { get; }
    public DateTimeOffset PlayedAt { get; }
}
