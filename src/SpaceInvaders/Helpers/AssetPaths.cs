using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Helpers;

public static class AssetPaths
{
    public const string BlueShip = "avares://SpaceInvaders/Assets/Images/Ships/ship-blue.png";
    public const string RedShip = "avares://SpaceInvaders/Assets/Images/Ships/ship-red.png";
    public const string GreenShip = "avares://SpaceInvaders/Assets/Images/Ships/ship-green.png";
    public const string Squid = "avares://SpaceInvaders/Assets/Images/Aliens/squid.png";
    public const string Crab = "avares://SpaceInvaders/Assets/Images/Aliens/crab.png";
    public const string Octopus = "avares://SpaceInvaders/Assets/Images/Aliens/octopus.png";
    public const string Ufo = "avares://SpaceInvaders/Assets/Images/Aliens/ufo.png";
    public const string FullHeart = "avares://SpaceInvaders/Assets/Images/Interface/heart-full.png";
    public const string EmptyHeart = "avares://SpaceInvaders/Assets/Images/Interface/heart-empty.png";
    public const string PlayIcon = "avares://SpaceInvaders/Assets/Images/Interface/icon-play.png";
    public const string PauseIcon = "avares://SpaceInvaders/Assets/Images/Interface/icon-pause.png";
    public const string RestartIcon = "avares://SpaceInvaders/Assets/Images/Interface/icon-restart.png";
    public const string ExitIcon = "avares://SpaceInvaders/Assets/Images/Interface/icon-exit.png";

    public static string ForShip(string shipId)
    {
        switch (shipId)
        {
            case "red":
                return RedShip;
            case "green":
                return GreenShip;
            default:
                return BlueShip;
        }
    }

    public static string ForAlien(AlienType type)
    {
        switch (type)
        {
            case AlienType.Squid:
                return Squid;
            case AlienType.Crab:
                return Crab;
            case AlienType.Octopus:
                return Octopus;
            default:
                return Ufo;
        }
    }
}
