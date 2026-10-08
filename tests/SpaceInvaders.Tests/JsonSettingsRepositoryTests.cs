using SpaceInvaders.Models.Game;
using SpaceInvaders.Services;

namespace SpaceInvaders.Tests;

public sealed class JsonSettingsRepositoryTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"SpaceInvadersSettingsTests-{Guid.NewGuid():N}");

    [Fact]
    public async Task MissingFile_ReturnsBlueShipDefault()
    {
        var repository = new JsonSettingsRepository(Path.Combine(_directory, "settings.json"));

        var settings = await repository.GetAsync();

        Assert.Equal("blue", settings.SelectedShipId);
    }

    [Fact]
    public async Task SaveAndLoad_PreservesSelectedShip()
    {
        var path = Path.Combine(_directory, "settings.json");
        var repository = new JsonSettingsRepository(path);

        await repository.SaveAsync(new AppSettings("green"));
        var settings = await repository.GetAsync();

        Assert.Equal("green", settings.SelectedShipId);
    }

    [Fact]
    public async Task CorruptFile_ReturnsSafeDefault()
    {
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, "settings.json");
        await File.WriteAllTextAsync(path, "invalid-json");
        var repository = new JsonSettingsRepository(path);

        var settings = await repository.GetAsync();

        Assert.Equal("blue", settings.SelectedShipId);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }
    }
}
