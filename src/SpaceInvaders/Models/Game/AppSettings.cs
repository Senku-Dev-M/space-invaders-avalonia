namespace SpaceInvaders.Models.Game;

public sealed class AppSettings
{
    public AppSettings(string selectedShipId)
    {
        SelectedShipId = selectedShipId;
    }

    public string SelectedShipId { get; }

    public static AppSettings CreateDefault()
    {
        return new AppSettings("blue");
    }
}
