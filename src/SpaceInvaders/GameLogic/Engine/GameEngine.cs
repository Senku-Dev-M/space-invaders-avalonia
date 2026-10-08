using SpaceInvaders.GameLogic.Collisions;
using SpaceInvaders.GameLogic.Movement;
using SpaceInvaders.Helpers;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;
using SpaceInvaders.Models.Game;

namespace SpaceInvaders.GameLogic.Engine;

public sealed class GameEngine : IGameEngine
{
    private readonly Random _random;
    private readonly Dictionary<int, int> _shieldHits = new Dictionary<int, int>();
    private bool _moveLeft;
    private bool _moveRight;
    private int _formationDirection = 1;
    private int _ufoDirection = 1;
    private double _enemyFireTimer;
    private double _ufoTimer;
    private double _invulnerabilityTimer;
    private int _nextExtraLifeScore = GameConstants.ExtraLifeInterval;
    private string _shipId = "blue";

    public GameEngine(Random? random = null)
    {
        if (random == null)
        {
            _random = Random.Shared;
        }
        else
        {
            _random = random;
        }
    }

    public GameSession Session { get; } = new GameSession();

    public event EventHandler<GameResult>? GameEnded;

    public void StartNewGame(string shipId)
    {
        if (string.IsNullOrWhiteSpace(shipId))
        {
            _shipId = "blue";
        }
        else
        {
            _shipId = shipId;
        }
        _moveLeft = false;
        _moveRight = false;
        _formationDirection = 1;
        _invulnerabilityTimer = 0;
        _nextExtraLifeScore = GameConstants.ExtraLifeInterval;

        Session.Score = 0;
        Session.Wave = 1;
        Session.Lives = GameConstants.InitialLives;
        Session.Player = CreatePlayer();
        Session.Projectiles.Clear();
        BuildWave();
        Session.Status = GameStatus.Running;
    }

    public void Update(double deltaSeconds)
    {
        if (Session.Status != GameStatus.Running)
        {
            return;
        }

        var delta = Math.Clamp(deltaSeconds, 0, 0.05);
        UpdateInvulnerability(delta);
        MovePlayer(delta);
        MoveFormation(delta);

        if (Session.Status != GameStatus.Running)
        {
            return;
        }

        MoveUfo(delta);
        MoveProjectiles(delta);
        ResolveProjectileCollisions();

        if (Session.Status != GameStatus.Running)
        {
            return;
        }

        ErodeShieldsUnderAliens();
        RemoveOutOfBoundsProjectiles();

        if (!HasFormationAliens())
        {
            AdvanceWave();
            return;
        }

        UpdateEnemyFire(delta);
        UpdateUfoSpawn(delta);
    }

    public void SetMoveLeft(bool isPressed)
    {
        _moveLeft = isPressed;
    }

    public void SetMoveRight(bool isPressed)
    {
        _moveRight = isPressed;
    }

    public bool TryFire()
    {
        if (Session.Status != GameStatus.Running || HasPlayerProjectile())
        {
            return false;
        }

        var player = Session.Player;
        Session.Projectiles.Add(new Projectile(
            ProjectileOwner.Player,
            player.X + ((player.Width - 4) / 2),
            player.Y - 14,
            GameConstants.PlayerProjectileSpeed));

        return true;
    }

    public void TogglePause()
    {
        if (Session.Status == GameStatus.Running)
        {
            Session.Status = GameStatus.Paused;
        }
        else if (Session.Status == GameStatus.Paused)
        {
            Session.Status = GameStatus.Running;
        }

        _moveLeft = false;
        _moveRight = false;
    }

    public void Pause()
    {
        if (Session.Status == GameStatus.Running)
        {
            Session.Status = GameStatus.Paused;
            _moveLeft = false;
            _moveRight = false;
        }
    }

    private PlayerShip CreatePlayer()
    {
        return new PlayerShip(_shipId, (GameConstants.ArenaWidth - 48) / 2, GameConstants.PlayerY);
    }

    private void BuildWave()
    {
        Session.Aliens.Clear();
        foreach (var alien in FormationFactory.CreateClassicFormation())
        {
            Session.Aliens.Add(alien);
        }

        Session.ShieldBlocks.Clear();
        _shieldHits.Clear();
        foreach (var block in FormationFactory.CreateShields())
        {
            Session.ShieldBlocks.Add(block);
        }

        Session.Projectiles.Clear();
        _formationDirection = 1;
        _enemyFireTimer = GetEnemyFireInterval();
        _ufoTimer = NextUfoInterval();
    }

    private void AdvanceWave()
    {
        Session.Wave++;
        Session.Player.X = (GameConstants.ArenaWidth - Session.Player.Width) / 2;
        BuildWave();
    }

    private void MovePlayer(double delta)
    {
        var direction = 0;
        if (_moveRight)
        {
            direction++;
        }

        if (_moveLeft)
        {
            direction--;
        }
        Session.Player.X = Math.Clamp(
            Session.Player.X + (direction * GameConstants.PlayerSpeed * delta),
            12,
            GameConstants.ArenaWidth - Session.Player.Width - 12);
    }

    private void MoveFormation(double delta)
    {
        var formation = new List<Alien>();
        foreach (var alien in Session.Aliens)
        {
            if (alien.Type != AlienType.Ufo)
            {
                formation.Add(alien);
            }
        }

        if (formation.Count == 0)
        {
            return;
        }

        var difficulty = GameConstants.DifficultyFactor(Session.Wave);
        var remainingAcceleration = 1 + (1.5 * (1 - (formation.Count / 55d)));
        var movement = _formationDirection * GameConstants.AlienBaseSpeed * difficulty * remainingAcceleration * delta;
        var leftEdge = formation[0].Left;
        var rightEdge = formation[0].Right;
        foreach (var alien in formation)
        {
            if (alien.Left < leftEdge)
            {
                leftEdge = alien.Left;
            }

            if (alien.Right > rightEdge)
            {
                rightEdge = alien.Right;
            }
        }

        var reachesEdge = leftEdge + movement < 16 ||
                          rightEdge + movement > GameConstants.ArenaWidth - 16;

        if (reachesEdge)
        {
            _formationDirection *= -1;
            foreach (var alien in formation)
            {
                alien.Y += GameConstants.AlienDropDistance;
            }
        }
        else
        {
            foreach (var alien in formation)
            {
                alien.X += movement;
            }
        }

        foreach (var alien in formation)
        {
            if (alien.Bottom >= GameConstants.DefenseLineY)
            {
                EndGame();
                break;
            }
        }
    }

    private void MoveUfo(double delta)
    {
        Alien? ufo = null;
        foreach (var alien in Session.Aliens)
        {
            if (alien.Type == AlienType.Ufo)
            {
                ufo = alien;
                break;
            }
        }
        if (ufo == null)
        {
            return;
        }

        ufo.X += _ufoDirection * 92 * GameConstants.DifficultyFactor(Session.Wave) * delta;
        if (ufo.Right < 0 || ufo.Left > GameConstants.ArenaWidth)
        {
            Session.Aliens.Remove(ufo);
        }
    }

    private void MoveProjectiles(double delta)
    {
        foreach (var projectile in Session.Projectiles)
        {
            projectile.Y += projectile.VelocityY * delta;
        }
    }

    private void ResolveProjectileCollisions()
    {
        for (var projectileIndex = Session.Projectiles.Count - 1; projectileIndex >= 0; projectileIndex--)
        {
            var projectile = Session.Projectiles[projectileIndex];
            var shield = FindCollidingShield(projectile);
            if (shield != null)
            {
                DamageShield(shield.BunkerIndex);
                Session.Projectiles.RemoveAt(projectileIndex);
                continue;
            }

            if (projectile.Owner == ProjectileOwner.Player)
            {
                var alien = FindCollidingAlien(projectile);
                if (alien != null)
                {
                    Session.Aliens.Remove(alien);
                    Session.Projectiles.RemoveAt(projectileIndex);
                    AwardPoints(alien.Points);
                }

                continue;
            }

            if (CollisionDetector.Intersects(projectile, Session.Player))
            {
                Session.Projectiles.RemoveAt(projectileIndex);
                if (!Session.Player.IsInvulnerable)
                {
                    LoseLife();
                    return;
                }
            }
        }
    }

    private void ErodeShieldsUnderAliens()
    {
        for (var index = Session.ShieldBlocks.Count - 1; index >= 0; index--)
        {
            var block = Session.ShieldBlocks[index];
            var collidedWithAlien = false;
            foreach (var alien in Session.Aliens)
            {
                if (alien.Type != AlienType.Ufo && CollisionDetector.Intersects(alien, block))
                {
                    collidedWithAlien = true;
                    break;
                }
            }

            if (collidedWithAlien)
            {
                Session.ShieldBlocks.RemoveAt(index);
            }
        }
    }

    private void DamageShield(int bunkerIndex)
    {
        var hitCount = _shieldHits.GetValueOrDefault(bunkerIndex) + 1;
        _shieldHits[bunkerIndex] = hitCount;

        for (var index = Session.ShieldBlocks.Count - 1; index >= 0; index--)
        {
            var block = Session.ShieldBlocks[index];
            if (block.BunkerIndex == bunkerIndex && (block.DamageStage == hitCount || hitCount >= 5))
            {
                Session.ShieldBlocks.RemoveAt(index);
            }
        }
    }

    private void RemoveOutOfBoundsProjectiles()
    {
        for (var index = Session.Projectiles.Count - 1; index >= 0; index--)
        {
            var projectile = Session.Projectiles[index];
            if (projectile.Bottom < 0 || projectile.Top > GameConstants.ArenaHeight)
            {
                Session.Projectiles.RemoveAt(index);
            }
        }
    }

    private void UpdateEnemyFire(double delta)
    {
        _enemyFireTimer -= delta;
        var activeAlienProjectiles = 0;
        foreach (var projectile in Session.Projectiles)
        {
            if (projectile.Owner == ProjectileOwner.Alien)
            {
                activeAlienProjectiles++;
            }
        }

        if (_enemyFireTimer > 0 || activeAlienProjectiles >= GameConstants.MaxAlienProjectiles(Session.Wave))
        {
            return;
        }

        var shooters = GetPossibleShooters();

        if (shooters.Count > 0)
        {
            var shooter = shooters[_random.Next(shooters.Count)];
            var speedFactor = Math.Min(1.8, GameConstants.DifficultyFactor(Session.Wave));
            Session.Projectiles.Add(new Projectile(
                ProjectileOwner.Alien,
                shooter.X + ((shooter.Width - 5) / 2),
                shooter.Bottom,
                GameConstants.AlienProjectileBaseSpeed * speedFactor));
        }

        _enemyFireTimer = GetEnemyFireInterval() * (0.85 + (_random.NextDouble() * 0.3));
    }

    private void UpdateUfoSpawn(double delta)
    {
        if (HasUfo())
        {
            return;
        }

        _ufoTimer -= delta;
        if (_ufoTimer > 0)
        {
            return;
        }

        int[] values = new int[] { 50, 100, 150, 300 };
        if (_random.Next(2) == 0)
        {
            _ufoDirection = 1;
        }
        else
        {
            _ufoDirection = -1;
        }

        double startX;
        if (_ufoDirection == 1)
        {
            startX = -54;
        }
        else
        {
            startX = GameConstants.ArenaWidth + 2;
        }
        Session.Aliens.Add(new Alien(AlienType.Ufo, -1, values[_random.Next(values.Length)], startX, 28, 52, 22));
        _ufoTimer = NextUfoInterval();
    }

    private void LoseLife()
    {
        Session.Lives--;
        if (Session.Lives <= 0)
        {
            EndGame();
            return;
        }

        Session.Projectiles.Clear();
        Session.Player.X = (GameConstants.ArenaWidth - Session.Player.Width) / 2;
        Session.Player.IsInvulnerable = true;
        _invulnerabilityTimer = GameConstants.RespawnInvulnerabilitySeconds;
    }

    private void UpdateInvulnerability(double delta)
    {
        if (!Session.Player.IsInvulnerable)
        {
            return;
        }

        _invulnerabilityTimer -= delta;
        if (_invulnerabilityTimer <= 0)
        {
            Session.Player.IsInvulnerable = false;
        }
    }

    private void AwardPoints(int points)
    {
        Session.Score += points;
        while (Session.Score >= _nextExtraLifeScore)
        {
            if (Session.Lives < GameConstants.InitialLives)
            {
                Session.Lives++;
            }

            _nextExtraLifeScore += GameConstants.ExtraLifeInterval;
        }
    }

    private void EndGame()
    {
        if (Session.Status == GameStatus.GameOver)
        {
            return;
        }

        Session.Status = GameStatus.GameOver;
        _moveLeft = false;
        _moveRight = false;
        var gameEndedHandler = GameEnded;
        if (gameEndedHandler != null)
        {
            gameEndedHandler(this, new GameResult(Session.Score, Session.Wave, _shipId));
        }
    }

    private bool HasFormationAliens()
    {
        foreach (var alien in Session.Aliens)
        {
            if (alien.Type != AlienType.Ufo)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasPlayerProjectile()
    {
        foreach (var projectile in Session.Projectiles)
        {
            if (projectile.Owner == ProjectileOwner.Player)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasUfo()
    {
        foreach (var alien in Session.Aliens)
        {
            if (alien.Type == AlienType.Ufo)
            {
                return true;
            }
        }

        return false;
    }

    private ShieldBlock? FindCollidingShield(Projectile projectile)
    {
        foreach (var block in Session.ShieldBlocks)
        {
            if (CollisionDetector.Intersects(projectile, block))
            {
                return block;
            }
        }

        return null;
    }

    private Alien? FindCollidingAlien(Projectile projectile)
    {
        foreach (var alien in Session.Aliens)
        {
            if (CollisionDetector.Intersects(projectile, alien))
            {
                return alien;
            }
        }

        return null;
    }

    private List<Alien> GetPossibleShooters()
    {
        var bottomAlienByColumn = new Dictionary<int, Alien>();

        foreach (var alien in Session.Aliens)
        {
            if (alien.Type == AlienType.Ufo)
            {
                continue;
            }

            Alien? currentBottomAlien;
            if (!bottomAlienByColumn.TryGetValue(alien.Column, out currentBottomAlien) || alien.Y > currentBottomAlien.Y)
            {
                bottomAlienByColumn[alien.Column] = alien;
            }
        }

        return new List<Alien>(bottomAlienByColumn.Values);
    }

    private double GetEnemyFireInterval()
    {
        return GameConstants.AlienFireInterval(Session.Wave);
    }

    private double NextUfoInterval()
    {
        return 18 + (_random.NextDouble() * 12);
    }
}
