using SpaceInvaders.Models.Entities;

namespace SpaceInvaders.Services.Interfaces;

public interface IScoreRepository
{
    Task<IReadOnlyList<ScoreEntry>> GetTopScoresAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ScoreEntry>> AddScoreAsync(ScoreEntry entry, CancellationToken cancellationToken = default);
}
