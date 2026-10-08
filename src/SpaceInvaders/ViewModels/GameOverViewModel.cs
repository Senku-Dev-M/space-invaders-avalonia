using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Game;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class GameOverViewModel : ViewModelBase
{
    private readonly GameResult _result;
    private readonly IScoreRepository _scoreRepository;
    private readonly INavigationService _navigation;

    public GameOverViewModel(
        GameResult result,
        IScoreRepository scoreRepository,
        INavigationService navigation)
    {
        _result = result;
        _scoreRepository = scoreRepository;
        _navigation = navigation;
    }

    public int Score
    {
        get { return _result.Score; }
    }

    public int Wave
    {
        get { return _result.Wave; }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveScoreCommand))]
    private string _playerName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveScoreCommand))]
    private bool _isSaving;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveScoreCommand))]
    private bool _isSaved;

    [ObservableProperty]
    private string? _errorMessage;

    private bool CanSaveScore()
    {
        if (IsSaving || IsSaved || string.IsNullOrWhiteSpace(PlayerName))
        {
            return false;
        }

        var nameLength = PlayerName.Trim().Length;
        return nameLength >= 1 && nameLength <= 12;
    }

    [RelayCommand(CanExecute = nameof(CanSaveScore))]
    private async Task SaveScoreAsync()
    {
        IsSaving = true;
        ErrorMessage = null;

        try
        {
            var entry = new ScoreEntry(PlayerName.Trim(), Score, Wave, DateTimeOffset.Now);
            await _scoreRepository.AddScoreAsync(entry);
            IsSaved = true;
            _navigation.ShowHighScores();
        }
        catch (IOException)
        {
            ErrorMessage = "No se pudo guardar el puntaje. Inténtalo nuevamente.";
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "No se pudo guardar el puntaje. Inténtalo nuevamente.";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Retry()
    {
        _navigation.StartGame(_result.ShipId);
    }

    [RelayCommand]
    private void BackToMenu()
    {
        _navigation.ShowMenu();
    }
}
