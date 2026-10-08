using System.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceInvaders.GameLogic.Engine;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;
using SpaceInvaders.Models.Game;
using SpaceInvaders.Helpers;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class GameViewModel : ViewModelBase, IDisposable
{
    private readonly IGameLoopService _gameLoop;
    private readonly INavigationService _navigation;
    private readonly string _shipId;
    private bool _disposed;

    public GameViewModel(
        IGameEngine engine,
        IGameLoopService gameLoop,
        INavigationService navigation,
        string shipId)
    {
        Engine = engine;
        _gameLoop = gameLoop;
        _navigation = navigation;
        _shipId = shipId;
        Engine.GameEnded += OnGameEnded;
        Engine.Session.PropertyChanged += OnSessionPropertyChanged;
        RefreshLifeIcons();
        _gameLoop.Start(Engine.Update);
    }

    public IGameEngine Engine { get; }
    public GameSession Session
    {
        get { return Engine.Session; }
    }

    public bool IsPaused
    {
        get { return Session.Status == GameStatus.Paused; }
    }

    public string PauseButtonText
    {
        get
        {
            if (IsPaused)
            {
                return "REANUDAR";
            }

            return "PAUSA";
        }
    }

    public string PauseIconAssetUri
    {
        get
        {
            if (IsPaused)
            {
                return AssetPaths.PlayIcon;
            }

            return AssetPaths.PauseIcon;
        }
    }

    public ObservableCollection<LifeIconViewModel> LifeIcons { get; } = new ObservableCollection<LifeIconViewModel>();

    [ObservableProperty]
    private bool _isExitConfirmationVisible;

    public void PauseIfRunning()
    {
        Engine.Pause();
    }

    [RelayCommand]
    private void SetMoveLeft(bool isPressed)
    {
        Engine.SetMoveLeft(isPressed);
    }

    [RelayCommand]
    private void SetMoveRight(bool isPressed)
    {
        Engine.SetMoveRight(isPressed);
    }

    [RelayCommand]
    private void Fire()
    {
        Engine.TryFire();
    }

    [RelayCommand]
    private void TogglePause()
    {
        Engine.TogglePause();
    }

    [RelayCommand]
    private void Restart()
    {
        Engine.StartNewGame(_shipId);
    }

    [RelayCommand]
    private void RequestBackToMenu()
    {
        Engine.Pause();
        IsExitConfirmationVisible = true;
    }

    [RelayCommand]
    private void CancelBackToMenu()
    {
        IsExitConfirmationVisible = false;
        Engine.TogglePause();
    }

    [RelayCommand]
    private void ConfirmBackToMenu()
    {
        Engine.Pause();
        _navigation.ShowMenu();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Engine.GameEnded -= OnGameEnded;
        Engine.Session.PropertyChanged -= OnSessionPropertyChanged;
        _gameLoop.Dispose();
    }

    private void OnGameEnded(object? sender, GameResult result)
    {
        _gameLoop.Stop();
        _navigation.ShowGameOver(result);
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameSession.Status))
        {
            OnPropertyChanged(nameof(IsPaused));
            OnPropertyChanged(nameof(PauseButtonText));
            OnPropertyChanged(nameof(PauseIconAssetUri));
        }

        if (e.PropertyName == nameof(GameSession.Lives))
        {
            RefreshLifeIcons();
        }
    }

    private void RefreshLifeIcons()
    {
        LifeIcons.Clear();
        for (var slot = 1; slot <= GameConstants.InitialLives; slot++)
        {
            var isFilled = slot <= Session.Lives;
            string assetUri;
            if (isFilled)
            {
                assetUri = AssetPaths.FullHeart;
            }
            else
            {
                assetUri = AssetPaths.EmptyHeart;
            }

            LifeIcons.Add(new LifeIconViewModel(slot, isFilled, assetUri));
        }
    }
}

public sealed class LifeIconViewModel
{
    public LifeIconViewModel(int slot, bool isFilled, string assetUri)
    {
        Slot = slot;
        IsFilled = isFilled;
        AssetUri = assetUri;
    }

    public int Slot { get; }
    public bool IsFilled { get; }
    public string AssetUri { get; }
}
