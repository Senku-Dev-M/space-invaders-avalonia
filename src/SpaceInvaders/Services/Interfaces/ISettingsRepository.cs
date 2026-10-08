using SpaceInvaders.Models.Game;

namespace SpaceInvaders.Services.Interfaces;

public interface ISettingsRepository
{
    Task<AppSettings> GetAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default);
}
