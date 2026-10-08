using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Services.Interfaces;

public interface IAssetCatalog
{
    IReadOnlyList<ShipOption> Ships { get; }
    string GetShipAsset(string shipId);
    string GetAlienAsset(AlienType type);
}
