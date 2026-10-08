using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SpaceInvaders.ViewModels;
using SpaceInvaders.Views;
using SpaceInvaders.Services;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders;

public partial class App : Application
{
    private IClassicDesktopStyleApplicationLifetime? _desktopLifetime;
    private MainViewModel? _mainViewModel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var desktop = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        if (desktop != null)
        {
            _desktopLifetime = desktop;
            var scoreRepository = new JsonScoreRepository();
            var settingsRepository = new JsonSettingsRepository();
            var assetCatalog = new AssetCatalog();
            _mainViewModel = new MainViewModel(
                scoreRepository,
                assetCatalog,
                settingsRepository,
                CreateGameLoop,
                ExitApplication);

            var mainWindow = new MainWindow();
            mainWindow.DataContext = _mainViewModel;

            mainWindow.Closed += OnMainWindowClosed;
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IGameLoopService CreateGameLoop()
    {
        return new DispatcherGameLoopService();
    }

    private void ExitApplication()
    {
        if (_desktopLifetime != null)
        {
            _desktopLifetime.Shutdown();
        }
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        if (_mainViewModel != null)
        {
            _mainViewModel.Dispose();
        }
    }
}
