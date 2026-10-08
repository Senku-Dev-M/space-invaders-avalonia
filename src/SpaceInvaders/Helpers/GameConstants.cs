namespace SpaceInvaders.Helpers;

public static class GameConstants
{
    public const double ArenaWidth = 800;
    public const double ArenaHeight = 600;
    public const double DefenseLineY = 535;
    public const double PlayerY = 554;
    public const double PlayerSpeed = 260;
    public const double PlayerProjectileSpeed = -420;
    public const double AlienProjectileBaseSpeed = 210;
    public const double AlienBaseSpeed = 32;
    public const double AlienDropDistance = 18;
    public const int InitialLives = 5;
    public const int ExtraLifeInterval = 1500;
    public const double RespawnInvulnerabilitySeconds = 1.5;

    public static double DifficultyFactor(int wave)
    {
        return Math.Min(2.5, 1 + (0.12 * (wave - 1)));
    }

    public static int MaxAlienProjectiles(int wave)
    {
        return Math.Min(7, 2 + ((wave - 1) / 2));
    }

    public static double AlienFireInterval(int wave)
    {
        return Math.Max(0.28, 0.8 / DifficultyFactor(wave));
    }
}
