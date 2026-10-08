using SpaceInvaders.Models.Entities;
using SpaceInvaders.Services;

namespace SpaceInvaders.Tests;

public sealed class JsonScoreRepositoryTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), $"SpaceInvadersTests-{Guid.NewGuid():N}");

    [Fact]
    public async Task AddScore_KeepsOnlyTopTenInDescendingOrder()
    {
        var path = Path.Combine(_directory, "scores.json");
        var repository = new JsonScoreRepository(path);

        for (var index = 1; index <= 12; index++)
        {
            await repository.AddScoreAsync(new ScoreEntry($"P{index}", index * 100, index, DateTimeOffset.UtcNow.AddMinutes(index)));
        }

        var scores = await repository.GetTopScoresAsync();

        Assert.Equal(10, scores.Count);
        Assert.Equal(1200, scores[0].Score);
        Assert.Equal(300, scores[scores.Count - 1].Score);
        for (var index = 1; index < scores.Count; index++)
        {
            Assert.True(scores[index - 1].Score >= scores[index].Score);
        }
    }

    [Fact]
    public async Task CorruptJson_IsPreservedAndReturnsEmptyTable()
    {
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, "scores.json");
        await File.WriteAllTextAsync(path, "{not-json");
        var repository = new JsonScoreRepository(path);

        var scores = await repository.GetTopScoresAsync();

        Assert.Empty(scores);
        Assert.False(File.Exists(path));
        Assert.Single(Directory.GetFiles(_directory, "*.corrupt.json"));
    }

    [Fact]
    public async Task MissingFile_ReturnsEmptyTable()
    {
        var repository = new JsonScoreRepository(Path.Combine(_directory, "scores.json"));

        var scores = await repository.GetTopScoresAsync();

        Assert.Empty(scores);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, true);
        }
    }
}
