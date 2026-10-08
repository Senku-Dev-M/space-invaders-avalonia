using CommunityToolkit.Mvvm.ComponentModel;
using SpaceInvaders.GameLogic.Engine;
using SpaceInvaders.Models.Game;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class MainViewModel : ViewModelBase, INavigationService, IDisposable
{
    private readonly IScoreRepository _scoreRepository;
    private readonly IAssetCatalog _assetCatalog;
    private readonly ISettingsRepository _settingsRepository;
    private readonly Func<IGameLoopService> _gameLoopFactory;
    private readonly Action _exitApplication;

    public MainViewModel(
        IScoreRepository scoreRepository,
        IAssetCatalog assetCatalog,
        ISettingsRepository settingsRepository,
        Func<IGameLoopService> gameLoopFactory,
        Action exitApplication)
    {
        _scoreRepository = scoreRepository;
        _assetCatalog = assetCatalog;
        _settingsRepository = settingsRepository;
        _gameLoopFactory = gameLoopFactory;
        _exitApplication = exitApplication;
        ShowMenu();
    }

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    public void ShowMenu()
    {
        SetCurrent(new MenuViewModel(this, _scoreRepository, _settingsRepository));
    }

    public void StartGame(string shipId)
    {
        var engine = new GameEngine();
        engine.StartNewGame(shipId);
        SetCurrent(new GameViewModel(engine, _gameLoopFactory(), this, shipId));
    }

    public void ShowHighScores()
    {
        SetCurrent(new HighScoresViewModel(_scoreRepository, this));
    }

    public void ShowSettings()
    {
        SetCurrent(new SettingsViewModel(this, _settingsRepository, _assetCatalog));
    }

    public void ShowGameOver(GameResult result)
    {
        SetCurrent(new GameOverViewModel(result, _scoreRepository, this));
    }

    public void ExitApplication()
    {
        _exitApplication();
    }

    public void PauseActiveGame()
    {
        var game = CurrentViewModel as GameViewModel;
        if (game != null)
        {
            game.PauseIfRunning();
        }
    }

    public void Dispose()
    {
        var disposable = CurrentViewModel as IDisposable;
        if (disposable != null)
        {
            disposable.Dispose();
        }
    }

    private void SetCurrent(ViewModelBase next)
    {
        var disposable = CurrentViewModel as IDisposable;
        if (disposable != null)
        {
            disposable.Dispose();
        }

        CurrentViewModel = next;
    }
}
