using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class MenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly IScoreRepository _scoreRepository;
    private readonly ISettingsRepository _settingsRepository;
    private string _selectedShipId = "blue";

    public MenuViewModel(
        INavigationService navigation,
        IScoreRepository scoreRepository,
        ISettingsRepository settingsRepository)
    {
        _navigation = navigation;
        _scoreRepository = scoreRepository;
        _settingsRepository = settingsRepository;
        _ = LoadAsync();
    }

    [ObservableProperty]
    private int _bestScore;

    [RelayCommand]
    private void Play()
    {
        _navigation.StartGame(_selectedShipId);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        _navigation.ShowSettings();
    }

    [RelayCommand]
    private void ShowScores()
    {
        _navigation.ShowHighScores();
    }

    [RelayCommand]
    private void Exit()
    {
        _navigation.ExitApplication();
    }

    private async Task LoadAsync()
    {
        try
        {
            var scores = await _scoreRepository.GetTopScoresAsync();
            var settings = await _settingsRepository.GetAsync();

            if (scores.Count > 0)
            {
                BestScore = scores[0].Score;
            }
            else
            {
                BestScore = 0;
            }

            _selectedShipId = settings.SelectedShipId;
        }
        catch (IOException)
        {
            UseDefaultValues();
        }
        catch (UnauthorizedAccessException)
        {
            UseDefaultValues();
        }
    }

    private void UseDefaultValues()
    {
        BestScore = 0;
        _selectedShipId = "blue";
    }
}
