using SpaceInvaders.Models.Entities;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.GameLogic.Movement;

public static class FormationFactory
{
    public static List<Alien> CreateClassicFormation()
    {
        var aliens = new List<Alien>();
        const double startX = 120;
        const double startY = 74;
        const double columnGap = 50;
        const double rowGap = 38;

        for (var row = 0; row < 5; row++)
        {
            AlienType type;
            int points;

            if (row == 0)
            {
                type = AlienType.Squid;
                points = 30;
            }
            else if (row == 1 || row == 2)
            {
                type = AlienType.Crab;
                points = 20;
            }
            else
            {
                type = AlienType.Octopus;
                points = 10;
            }

            for (var column = 0; column < 11; column++)
            {
                aliens.Add(new Alien(
                    type,
                    column,
                    points,
                    startX + (column * columnGap),
                    startY + (row * rowGap)));
            }
        }

        return aliens;
    }

    public static List<ShieldBlock> CreateShields()
    {
        var shieldBlocks = new List<ShieldBlock>();
        double[] bunkerStarts = new double[] { 145, 370, 595 };

        for (var bunkerIndex = 0; bunkerIndex < bunkerStarts.Length; bunkerIndex++)
        {
            var startX = bunkerStarts[bunkerIndex];
            for (var row = 0; row < 6; row++)
            {
                for (var column = 0; column < 12; column++)
                {
                    var isTopCorner = row == 0 && (column < 2 || column > 9);
                    var isLowerOpening = row >= 3 && column >= 4 && column <= 7;

                    if (!isTopCorner && !isLowerOpening)
                    {
                        var damageStage = Math.Min(5, 1 + ((column * 5) / 12));
                        shieldBlocks.Add(new ShieldBlock(
                            bunkerIndex,
                            damageStage,
                            startX + (column * 6),
                            478 + (row * 6)));
                    }
                }
            }
        }

        return shieldBlocks;
    }
}
