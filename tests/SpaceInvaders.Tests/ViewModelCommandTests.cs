using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Game;
using SpaceInvaders.GameLogic.Engine;
using SpaceInvaders.Helpers;
using SpaceInvaders.Services;
using SpaceInvaders.Services.Interfaces;
using SpaceInvaders.ViewModels;

namespace SpaceInvaders.Tests;

public sealed class ViewModelCommandTests
{
    [Fact]
    public void MenuPlayCommand_StartsGameWithSelectedShip()
    {
        var navigation = new NavigationSpy();
        var viewModel = new MenuViewModel(
            navigation,
            new EmptyScoreRepository(),
            new MemorySettingsRepository(new AppSettings("green")));

        viewModel.PlayCommand.Execute(null);

        Assert.Equal("green", navigation.StartedShipId);
    }

    [Fact]
    public void GameOverSaveCommand_RequiresValidName()
    {
        var viewModel = new GameOverViewModel(
            new GameResult(900, 3, "blue"),
            new EmptyScoreRepository(),
            new NavigationSpy());

        Assert.False(viewModel.SaveScoreCommand.CanExecute(null));
        viewModel.PlayerName = "PILOTO";
        Assert.True(viewModel.SaveScoreCommand.CanExecute(null));
        viewModel.PlayerName = "NOMBRE-DEMASIADO-LARGO";
        Assert.False(viewModel.SaveScoreCommand.CanExecute(null));
    }

    [Fact]
    public void GameHud_AlwaysShowsFiveHeartSlotsMatchingCurrentLives()
    {
        var engine = new GameEngine(new Random(7));
        engine.StartNewGame("blue");
        using var viewModel = new GameViewModel(engine, new GameLoopStub(), new NavigationSpy(), "blue");

        Assert.Equal(5, viewModel.LifeIcons.Count);
        foreach (var icon in viewModel.LifeIcons)
        {
            Assert.True(icon.IsFilled);
        }

        engine.Session.Lives = 3;

        var filledHearts = 0;
        var emptyHearts = 0;
        foreach (var icon in viewModel.LifeIcons)
        {
            if (icon.IsFilled)
            {
                filledHearts++;
            }
            else
            {
                emptyHearts++;
            }
        }

        Assert.Equal(3, filledHearts);
        Assert.Equal(2, emptyHearts);
        Assert.Equal(AssetPaths.PauseIcon, viewModel.PauseIconAssetUri);

        engine.TogglePause();

        Assert.Equal(AssetPaths.PlayIcon, viewModel.PauseIconAssetUri);
    }

    private sealed class EmptyScoreRepository : IScoreRepository
    {
        public Task<IReadOnlyList<ScoreEntry>> GetTopScoresAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ScoreEntry> scores = new List<ScoreEntry>();
            return Task.FromResult(scores);
        }

        public Task<IReadOnlyList<ScoreEntry>> AddScoreAsync(ScoreEntry entry, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ScoreEntry> scores = new List<ScoreEntry> { entry };
            return Task.FromResult(scores);
        }
    }

    private sealed class MemorySettingsRepository : ISettingsRepository
    {
        private readonly AppSettings _settings;

        public MemorySettingsRepository(AppSettings settings)
        {
            _settings = settings;
        }

        public Task<AppSettings> GetAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_settings);
        }

        public Task SaveAsync(AppSettings updatedSettings, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class NavigationSpy : INavigationService
    {
        public string? StartedShipId { get; private set; }
        public void ShowMenu() { }
        public void StartGame(string shipId)
        {
            StartedShipId = shipId;
        }
        public void ShowHighScores() { }
        public void ShowSettings() { }
        public void ShowGameOver(GameResult result) { }
        public void ExitApplication() { }
    }

    private sealed class GameLoopStub : IGameLoopService
    {
        public bool IsRunning { get; private set; }
        public void Start(Action<double> update)
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
