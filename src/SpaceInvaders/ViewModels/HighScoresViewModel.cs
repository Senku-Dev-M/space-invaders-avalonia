using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class HighScoresViewModel : ViewModelBase
{
    private readonly IScoreRepository _scoreRepository;
    private readonly INavigationService _navigation;

    public HighScoresViewModel(IScoreRepository scoreRepository, INavigationService navigation)
    {
        _scoreRepository = scoreRepository;
        _navigation = navigation;
        _ = LoadAsync();
    }

    public ObservableCollection<ScoreRowViewModel> Scores { get; } = new ObservableCollection<ScoreRowViewModel>();

    [ObservableProperty]
    private bool _isEmpty = true;

    [ObservableProperty]
    private string? _errorMessage;

    [RelayCommand]
    private void Back()
    {
        _navigation.ShowMenu();
    }

    private async Task LoadAsync()
    {
        try
        {
            var scores = await _scoreRepository.GetTopScoresAsync();
            Scores.Clear();
            var rank = 1;
            foreach (var score in scores)
            {
                Scores.Add(new ScoreRowViewModel(rank++, score));
            }

            IsEmpty = Scores.Count == 0;
        }
        catch (IOException)
        {
            ShowLoadError();
        }
        catch (UnauthorizedAccessException)
        {
            ShowLoadError();
        }
    }

    private void ShowLoadError()
    {
        ErrorMessage = "No se pudo leer el archivo de puntajes.";
        IsEmpty = true;
    }
}

public sealed class ScoreRowViewModel
{
    public ScoreRowViewModel(int rank, ScoreEntry entry)
    {
        Rank = rank;
        Entry = entry;
    }

    public int Rank { get; }
    public ScoreEntry Entry { get; }
}
