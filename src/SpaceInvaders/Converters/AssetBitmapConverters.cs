using System.Collections.Concurrent;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SpaceInvaders.Helpers;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Converters;

internal static class AssetBitmapCache
{
    private static readonly ConcurrentDictionary<string, Bitmap> Cache =
        new ConcurrentDictionary<string, Bitmap>(StringComparer.OrdinalIgnoreCase);

    public static Bitmap? Get(string? assetUri)
    {
        if (string.IsNullOrWhiteSpace(assetUri))
        {
            return null;
        }

        Bitmap? cachedBitmap;
        if (Cache.TryGetValue(assetUri, out cachedBitmap))
        {
            return cachedBitmap;
        }

        using var stream = AssetLoader.Open(new Uri(assetUri));
        var newBitmap = new Bitmap(stream);
        Cache.TryAdd(assetUri, newBitmap);
        return newBitmap;
    }
}

public sealed class AssetUriToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var path = value as string;
        if (path == null)
        {
            return null;
        }

        return AssetBitmapCache.Get(path);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}

public sealed class AlienTypeToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!(value is AlienType))
        {
            return null;
        }

        var alienType = (AlienType)value;
        return AssetBitmapCache.Get(AssetPaths.ForAlien(alienType));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}

public sealed class ShipIdToBitmapConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var shipId = value as string;
        if (shipId == null)
        {
            return null;
        }

        return AssetBitmapCache.Get(AssetPaths.ForShip(shipId));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
