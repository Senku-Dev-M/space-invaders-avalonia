using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Models.Entities;

public sealed class Projectile : GameEntity
{
    public Projectile(ProjectileOwner owner, double x, double y, double velocityY)
        : base(x, y, GetWidth(owner), GetHeight(owner))
    {
        Owner = owner;
        VelocityY = velocityY;
    }

    public ProjectileOwner Owner { get; }
    public double VelocityY { get; }

    private static double GetWidth(ProjectileOwner owner)
    {
        if (owner == ProjectileOwner.Player)
        {
            return 4;
        }

        return 5;
    }

    private static double GetHeight(ProjectileOwner owner)
    {
        if (owner == ProjectileOwner.Player)
        {
            return 14;
        }

        return 12;
    }
}
