using SpaceInvaders.Models.Entities;

namespace SpaceInvaders.GameLogic.Collisions;

public static class CollisionDetector
{
    public static bool Intersects(GameEntity first, GameEntity second)
    {
        return first.Left < second.Right &&
               first.Right > second.Left &&
               first.Top < second.Bottom &&
               first.Bottom > second.Top;
    }
}
