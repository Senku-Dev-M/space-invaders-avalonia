namespace SpaceInvaders.Models.Entities;

public sealed class ShieldBlock : GameEntity
{
    public ShieldBlock(int bunkerIndex, int damageStage, double x, double y)
        : base(x, y, 6, 6)
    {
        BunkerIndex = bunkerIndex;
        DamageStage = damageStage;
    }

    public int BunkerIndex { get; }
    public int DamageStage { get; }
}
