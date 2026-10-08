namespace SpaceInvaders.Models.Entities;

public sealed class ShipOption
{
    public ShipOption(string id, string displayName, string assetUri, string accentColor)
    {
        Id = id;
        DisplayName = displayName;
        AssetUri = assetUri;
        AccentColor = accentColor;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public string AssetUri { get; }
    public string AccentColor { get; }
}
