using SpaceInvaders.Helpers;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;
using SpaceInvaders.Services.Interfaces;

namespace SpaceInvaders.Services;

public sealed class AssetCatalog : IAssetCatalog
{
    public IReadOnlyList<ShipOption> Ships { get; } = new List<ShipOption>
    {
        new ShipOption("blue", "AZURE-01", AssetPaths.BlueShip, "#42D9FF"),
        new ShipOption("red", "NOVA-X", AssetPaths.RedShip, "#FF4B71"),
        new ShipOption("green", "VIPER-7", AssetPaths.GreenShip, "#8BFF6A"),
    };

    public string GetShipAsset(string shipId)
    {
        return AssetPaths.ForShip(shipId);
    }

    public string GetAlienAsset(AlienType type)
    {
        return AssetPaths.ForAlien(type);
    }
}
