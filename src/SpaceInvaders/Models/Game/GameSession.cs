using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Models.Game;

public sealed partial class GameSession : ObservableObject
{
    public ObservableCollection<Alien> Aliens { get; } = new ObservableCollection<Alien>();
    public ObservableCollection<Projectile> Projectiles { get; } = new ObservableCollection<Projectile>();
    public ObservableCollection<ShieldBlock> ShieldBlocks { get; } = new ObservableCollection<ShieldBlock>();

    [ObservableProperty]
    private PlayerShip _player = new PlayerShip("blue", 376, 548);

    [ObservableProperty]
    private int _score;

    [ObservableProperty]
    private int _wave = 1;

    [ObservableProperty]
    private int _lives = 5;

    [ObservableProperty]
    private GameStatus _status = GameStatus.Ready;
}
