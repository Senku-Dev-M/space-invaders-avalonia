using SpaceInvaders.GameLogic.Engine;
using SpaceInvaders.Helpers;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Tests;

public sealed class GameEngineTests
{
    [Fact]
    public void StartNewGame_CreatesClassicFormationAndThreeShields()
    {
        var engine = CreateEngine();

        Assert.Equal(55, engine.Session.Aliens.Count);
        Assert.Equal(11, CountAliens(engine, AlienType.Squid));
        Assert.Equal(22, CountAliens(engine, AlienType.Crab));
        Assert.Equal(22, CountAliens(engine, AlienType.Octopus));
        AssertAlienPoints(engine, AlienType.Squid, 30);
        AssertAlienPoints(engine, AlienType.Crab, 20);
        AssertAlienPoints(engine, AlienType.Octopus, 10);
        Assert.Equal(168, engine.Session.ShieldBlocks.Count);
        Assert.Equal(5, engine.Session.Lives);
        Assert.Equal(1, engine.Session.Wave);
    }

    [Fact]
    public void TryFire_AllowsOnlyOnePlayerProjectile()
    {
        var engine = CreateEngine();

        Assert.True(engine.TryFire());
        Assert.False(engine.TryFire());
        Assert.Equal(1, CountPlayerProjectiles(engine));

        engine.Session.Projectiles.Clear();
        Assert.True(engine.TryFire());
    }

    [Fact]
    public void AlienProjectileHit_RemovesLifeAndStartsInvulnerability()
    {
        var engine = CreateEngine();
        var player = engine.Session.Player;
        engine.Session.Projectiles.Add(new Projectile(ProjectileOwner.Alien, player.X + 10, player.Y, 0));

        engine.Update(0);

        Assert.Equal(4, engine.Session.Lives);
        Assert.True(engine.Session.Player.IsInvulnerable);
        Assert.Empty(engine.Session.Projectiles);
    }

    [Fact]
    public void ReachingFifteenHundredPoints_RestoresLostLifeButDoesNotExceedFive()
    {
        var engine = CreateEngine();
        engine.Session.Lives = 4;
        engine.Session.Score = 1490;
        var target = FindAlien(engine, AlienType.Octopus);

        Assert.True(engine.TryFire());
        var projectile = engine.Session.Projectiles.Single();
        target.X = projectile.X;
        target.Y = 400;
        projectile.Y = 400;

        engine.Update(0);

        Assert.Equal(1500, engine.Session.Score);
        Assert.Equal(5, engine.Session.Lives);

        engine.Session.Score = 2990;
        var secondTarget = FindAlien(engine, AlienType.Octopus);
        Assert.True(engine.TryFire());
        projectile = engine.Session.Projectiles.Single();
        secondTarget.X = projectile.X;
        secondTarget.Y = 400;
        projectile.Y = 400;
        engine.Update(0);

        Assert.Equal(3000, engine.Session.Score);
        Assert.Equal(5, engine.Session.Lives);
    }

    [Fact]
    public void ClearingFormation_StartsNextInfiniteWaveAndRebuildsDefenses()
    {
        var engine = CreateEngine();
        engine.Session.Aliens.Clear();
        engine.Session.ShieldBlocks.Clear();
        engine.Update(0);

        Assert.Equal(2, engine.Session.Wave);
        Assert.Equal(55, engine.Session.Aliens.Count);
        Assert.Equal(168, engine.Session.ShieldBlocks.Count);
    }

    [Fact]
    public void AlienAtDefenseLine_EndsGameImmediately()
    {
        var engine = CreateEngine();
        var alien = engine.Session.Aliens[0];
        alien.Y = GameConstants.DefenseLineY - alien.Height;
        var tracker = new GameEndTracker();
        engine.GameEnded += tracker.OnGameEnded;

        engine.Update(0);

        Assert.True(tracker.HasEnded);
        Assert.Equal(GameStatus.GameOver, engine.Session.Status);
        Assert.Equal(5, engine.Session.Lives);
    }

    [Fact]
    public void FormationAtEdge_ChangesDirectionAndDescends()
    {
        var engine = CreateEngine();
        var originalPositions = new Dictionary<Guid, double>();
        var rightmost = engine.Session.Aliens[0];
        foreach (var alien in engine.Session.Aliens)
        {
            originalPositions[alien.Id] = alien.Y;
            if (alien.Right > rightmost.Right)
            {
                rightmost = alien;
            }
        }

        rightmost.X = GameConstants.ArenaWidth - rightmost.Width - 10;

        engine.Update(0.016);

        foreach (var alien in engine.Session.Aliens)
        {
            if (alien.Type != AlienType.Ufo)
            {
                Assert.Equal(originalPositions[alien.Id] + GameConstants.AlienDropDistance, alien.Y);
            }
        }
    }

    [Fact]
    public void Shield_ProtectsExactlyFiveProjectiles()
    {
        var engine = CreateEngine();
        const int bunkerIndex = 0;

        for (var hit = 1; hit <= 5; hit++)
        {
            var block = FindShieldBlock(engine, bunkerIndex);
            engine.Session.Projectiles.Add(new Projectile(ProjectileOwner.Player, block.X, block.Y, 0));
            engine.Update(0);

            if (hit < 5)
            {
                Assert.True(HasShieldBlocks(engine, bunkerIndex));
            }
        }

        Assert.False(HasShieldBlocks(engine, bunkerIndex));
        Assert.True(HasShieldBlocks(engine, 1));
    }

    [Fact]
    public void DifficultyAndEnemyShotCount_AreCapped()
    {
        Assert.Equal(1, GameConstants.DifficultyFactor(1));
        Assert.Equal(2.5, GameConstants.DifficultyFactor(100));
        Assert.Equal(2, GameConstants.MaxAlienProjectiles(1));
        Assert.Equal(7, GameConstants.MaxAlienProjectiles(100));
        Assert.Equal(0.8, GameConstants.AlienFireInterval(1));
        Assert.Equal(0.32, GameConstants.AlienFireInterval(100), 3);
    }

    private static GameEngine CreateEngine()
    {
        var engine = new GameEngine(new Random(1234));
        engine.StartNewGame("blue");
        return engine;
    }

    private static int CountAliens(GameEngine engine, AlienType type)
    {
        var count = 0;
        foreach (var alien in engine.Session.Aliens)
        {
            if (alien.Type == type)
            {
                count++;
            }
        }

        return count;
    }

    private static void AssertAlienPoints(GameEngine engine, AlienType type, int expectedPoints)
    {
        foreach (var alien in engine.Session.Aliens)
        {
            if (alien.Type == type)
            {
                Assert.Equal(expectedPoints, alien.Points);
            }
        }
    }

    private static int CountPlayerProjectiles(GameEngine engine)
    {
        var count = 0;
        foreach (var projectile in engine.Session.Projectiles)
        {
            if (projectile.Owner == ProjectileOwner.Player)
            {
                count++;
            }
        }

        return count;
    }

    private static Alien FindAlien(GameEngine engine, AlienType type)
    {
        foreach (var alien in engine.Session.Aliens)
        {
            if (alien.Type == type)
            {
                return alien;
            }
        }

        throw new InvalidOperationException("No se encontró el alien solicitado.");
    }

    private static ShieldBlock FindShieldBlock(GameEngine engine, int bunkerIndex)
    {
        foreach (var block in engine.Session.ShieldBlocks)
        {
            if (block.BunkerIndex == bunkerIndex)
            {
                return block;
            }
        }

        throw new InvalidOperationException("No se encontró el escudo solicitado.");
    }

    private static bool HasShieldBlocks(GameEngine engine, int bunkerIndex)
    {
        foreach (var block in engine.Session.ShieldBlocks)
        {
            if (block.BunkerIndex == bunkerIndex)
            {
                return true;
            }
        }

        return false;
    }

    private sealed class GameEndTracker
    {
        public bool HasEnded { get; private set; }

        public void OnGameEnded(object? sender, Models.Game.GameResult result)
        {
            HasEnded = true;
        }
    }
}
