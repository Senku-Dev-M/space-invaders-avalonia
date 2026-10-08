# Space Invaders — Avalonia MVVM

A desktop game built with C#, .NET 10, Avalonia 12.1, and CommunityToolkit.Mvvm.

## Screenshots

### Main menu

![Space Invaders main menu](docs/images/space-invaders-menu.png)

### Gameplay

![Space Invaders gameplay](docs/images/space-invaders-gameplay.png)

## Run the game

```powershell
dotnet run --project src/SpaceInvaders/SpaceInvaders.csproj
```

Controls: use the arrow keys or `A/D` to move, `Space` to shoot, and `P` or `Escape` to pause.

## Replace the assets

The graphical assets are located in `src/SpaceInvaders/Assets/Images` and are registered as `AvaloniaResource`.

| Element | Current file | Recommended size |
|---|---|---:|
| Blue ship | `Ships/ship-blue.png` | 64×40 px |
| Red ship | `Ships/ship-red.png` | 64×40 px |
| Green ship | `Ships/ship-green.png` | 64×40 px |
| Squid | `Aliens/squid.png` | 44×32 px |
| Crab | `Aliens/crab.png` | 44×32 px |
| Octopus | `Aliens/octopus.png` | 44×32 px |
| UFO | `Aliens/ufo.png` | 64×28 px |
| Menu logo | `Interface/space-invaders-logo.png` | 900×300 px |
| Available life | `Interface/heart-full.png` | 32×32 px |
| Lost life | `Interface/heart-empty.png` | 32×32 px |
| Resume | `Interface/icon-play.png` | 24×24 px |
| Pause | `Interface/icon-pause.png` | 24×24 px |
| Restart | `Interface/icon-restart.png` | 24×24 px |
| Exit | `Interface/icon-exit.png` | 24×24 px |

The easiest way to change a design is to replace its PNG while keeping the same file name. A transparent background and a similar aspect ratio are recommended; the view automatically scales each sprite to the logical size of the object.

To use JPG files, place the image in the same folder and update its path in `Helpers/AssetPaths.cs`, for example from `ship-blue.png` to `ship-blue.jpg`. Avalonia includes every file under `Assets/**`, so no project configuration changes are required.

## Scores

The top 10 scores and selected ship are stored as JSON files in the application's local data directory:

`%LOCALAPPDATA%\SpaceInvaders\scores.json` stores the high scores on Windows.

`%LOCALAPPDATA%\SpaceInvaders\settings.json` stores the hangar settings.

The game does not use a database or an online service.
