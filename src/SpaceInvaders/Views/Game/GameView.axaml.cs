using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using SpaceInvaders.ViewModels;

namespace SpaceInvaders.Views.Game;

public partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        var viewModel = DataContext as GameViewModel;
        if (viewModel == null)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.A:
                viewModel.SetMoveLeftCommand.Execute(true);
                e.Handled = true;
                break;
            case Key.Right:
            case Key.D:
                viewModel.SetMoveRightCommand.Execute(true);
                e.Handled = true;
                break;
            case Key.Space:
                viewModel.FireCommand.Execute(null);
                e.Handled = true;
                break;
            case Key.P:
            case Key.Escape:
                if (viewModel.IsExitConfirmationVisible)
                {
                    viewModel.CancelBackToMenuCommand.Execute(null);
                }
                else
                {
                    viewModel.TogglePauseCommand.Execute(null);
                }

                e.Handled = true;
                break;
        }
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        var viewModel = DataContext as GameViewModel;
        if (viewModel == null)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.A:
                viewModel.SetMoveLeftCommand.Execute(false);
                e.Handled = true;
                break;
            case Key.Right:
            case Key.D:
                viewModel.SetMoveRightCommand.Execute(false);
                e.Handled = true;
                break;
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Dispatcher.UIThread.Post(FocusGameView);
    }

    private void FocusGameView()
    {
        Focus();
    }
}
