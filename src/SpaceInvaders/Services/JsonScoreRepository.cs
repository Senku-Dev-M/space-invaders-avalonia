using System.Text.Json;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.Services;

public sealed class JsonScoreRepository : IScoreRepository
{
    private const int MaximumScores = 10;
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

    public JsonScoreRepository(string? filePath = null)
    {
        if (filePath == null)
        {
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SpaceInvaders",
                "scores.json");
        }
        else
        {
            _filePath = filePath;
        }
    }

    public async Task<IReadOnlyList<ScoreEntry>> GetTopScoresAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return await ReadScoresUnsafeAsync(cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<ScoreEntry>> AddScoreAsync(
        ScoreEntry entry,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entry);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var savedScores = await ReadScoresUnsafeAsync(cancellationToken);
            var scores = new List<ScoreEntry>(savedScores);
            var cleanEntry = new ScoreEntry(
                entry.PlayerName.Trim(),
                entry.Score,
                entry.Wave,
                entry.PlayedAt);

            scores.Add(cleanEntry);
            scores.Sort(CompareScores);

            if (scores.Count > MaximumScores)
            {
                scores.RemoveRange(MaximumScores, scores.Count - MaximumScores);
            }

            await WriteScoresUnsafeAsync(scores, cancellationToken);
            return scores;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<IReadOnlyList<ScoreEntry>> ReadScoresUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
        {
            return new List<ScoreEntry>();
        }

        try
        {
            await using var stream = File.OpenRead(_filePath);
            var scores = await JsonSerializer.DeserializeAsync<List<ScoreEntry>>(stream, _jsonOptions, cancellationToken);
            if (scores == null)
            {
                scores = new List<ScoreEntry>();
            }

            var validScores = new List<ScoreEntry>();
            foreach (var score in scores)
            {
                if (!string.IsNullOrWhiteSpace(score.PlayerName))
                {
                    validScores.Add(score);
                }
            }

            validScores.Sort(CompareScores);
            if (validScores.Count > MaximumScores)
            {
                validScores.RemoveRange(MaximumScores, validScores.Count - MaximumScores);
            }

            return validScores;
        }
        catch (JsonException)
        {
            PreserveCorruptFile();
            return new List<ScoreEntry>();
        }
        catch (NotSupportedException)
        {
            PreserveCorruptFile();
            return new List<ScoreEntry>();
        }
    }

    private async Task WriteScoresUnsafeAsync(IReadOnlyList<ScoreEntry> scores, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        Directory.CreateDirectory(directory);
        var temporaryPath = _filePath + ".tmp";

        await using (var stream = File.Create(temporaryPath))
        {
            await JsonSerializer.SerializeAsync(stream, scores, _jsonOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        File.Move(temporaryPath, _filePath, true);
    }

    private void PreserveCorruptFile()
    {
        var directory = Path.GetDirectoryName(_filePath)!;
        var fileName = Path.GetFileNameWithoutExtension(_filePath);
        var extension = Path.GetExtension(_filePath);
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff");
        var corruptPath = Path.Combine(directory, $"{fileName}.{timestamp}.corrupt{extension}");
        File.Move(_filePath, corruptPath, true);
    }

    private static int CompareScores(ScoreEntry first, ScoreEntry second)
    {
        var scoreComparison = second.Score.CompareTo(first.Score);
        if (scoreComparison != 0)
        {
            return scoreComparison;
        }

        return first.PlayedAt.CompareTo(second.PlayedAt);
    }
}
