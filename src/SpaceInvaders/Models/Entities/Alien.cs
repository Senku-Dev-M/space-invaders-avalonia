using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Models.Entities;

public sealed class Alien : GameEntity
{
    public Alien(AlienType type, int column, int points, double x, double y, double width = 32, double height = 24)
        : base(x, y, width, height)
    {
        Type = type;
        Column = column;
        Points = points;
    }

    public AlienType Type { get; }
    public int Column { get; }
    public int Points { get; }
}
