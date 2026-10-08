using System.Text.Json;
using SpaceInvaders.Models.Game;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.Services;

public sealed class JsonSettingsRepository : ISettingsRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

    public JsonSettingsRepository(string? filePath = null)
    {
        if (filePath == null)
        {
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SpaceInvaders",
                "settings.json");
        }
        else
        {
            _filePath = filePath;
        }
    }

    public async Task<AppSettings> GetAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_filePath))
            {
                return AppSettings.CreateDefault();
            }

            try
            {
                await using var stream = File.OpenRead(_filePath);
                var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, _jsonOptions, cancellationToken);
                if (settings == null)
                {
                    return AppSettings.CreateDefault();
                }

                return settings;
            }
            catch (JsonException)
            {
                return AppSettings.CreateDefault();
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var directory = Path.GetDirectoryName(_filePath)!;
            Directory.CreateDirectory(directory);
            var temporaryPath = _filePath + ".tmp";

            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, settings, _jsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, _filePath, true);
        }
        finally
        {
            _gate.Release();
        }
    }
}
