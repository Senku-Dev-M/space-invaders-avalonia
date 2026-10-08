using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Game;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly ISettingsRepository _settingsRepository;

    public SettingsViewModel(
        INavigationService navigation,
        ISettingsRepository settingsRepository,
        IAssetCatalog assetCatalog)
    {
        _navigation = navigation;
        _settingsRepository = settingsRepository;
        Ships = assetCatalog.Ships;
        SelectedShip = Ships[0];
        _ = LoadAsync();
    }

    public IReadOnlyList<ShipOption> Ships { get; }

    [ObservableProperty]
    private ShipOption? _selectedShip;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string? _errorMessage;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedShip == null || IsSaving)
        {
            return;
        }

        IsSaving = true;
        ErrorMessage = null;
        try
        {
            await _settingsRepository.SaveAsync(new AppSettings(SelectedShip.Id));
            _navigation.ShowMenu();
        }
        catch (IOException)
        {
            ErrorMessage = "No se pudo guardar la configuración.";
        }
        catch (UnauthorizedAccessException)
        {
            ErrorMessage = "No se pudo guardar la configuración.";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Back()
    {
        _navigation.ShowMenu();
    }

    private async Task LoadAsync()
    {
        try
        {
            var settings = await _settingsRepository.GetAsync();
            SelectedShip = FindShipById(settings.SelectedShipId);
        }
        catch (IOException)
        {
            UseDefaultShip();
        }
        catch (UnauthorizedAccessException)
        {
            UseDefaultShip();
        }
    }

    private ShipOption FindShipById(string shipId)
    {
        foreach (var ship in Ships)
        {
            if (ship.Id == shipId)
            {
                return ship;
            }
        }

        return Ships[0];
    }

    private void UseDefaultShip()
    {
        SelectedShip = Ships[0];
        ErrorMessage = "Se cargó la nave predeterminada.";
    }
}
