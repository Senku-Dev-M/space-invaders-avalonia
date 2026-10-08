using SpaceInvaders.Models.Game;

namespace SpaceInvaders.Services.Interfaces;

public interface INavigationService
{
    void ShowMenu();
    void StartGame(string shipId);
    void ShowHighScores();
    void ShowSettings();
    void ShowGameOver(GameResult result);
    void ExitApplication();
}
