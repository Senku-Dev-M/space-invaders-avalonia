using Avalonia.Controls;
using SpaceInvaders.ViewModels;

namespace SpaceInvaders.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Deactivated += OnWindowDeactivated;
    }

    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        var mainViewModel = DataContext as MainViewModel;
        if (mainViewModel != null)
        {
            mainViewModel.PauseActiveGame();
        }
    }
}
