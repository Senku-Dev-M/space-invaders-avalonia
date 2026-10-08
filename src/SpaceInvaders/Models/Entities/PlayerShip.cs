using CommunityToolkit.Mvvm.ComponentModel;

namespace SpaceInvaders.Models.Entities;

public sealed partial class PlayerShip : GameEntity
{
    public PlayerShip(string shipId, double x, double y)
        : base(x, y, 48, 28)
    {
        ShipId = shipId;
    }

    public string ShipId { get; }

    [ObservableProperty]
    private bool _isInvulnerable;
}
