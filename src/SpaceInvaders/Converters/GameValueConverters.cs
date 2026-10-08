using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SpaceInvaders.Models.Enums;

namespace SpaceInvaders.Converters;

public sealed class ProjectileOwnerToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ProjectileOwner && (ProjectileOwner)value == ProjectileOwner.Player)
        {
            return Brushes.White;
        }

        return new SolidColorBrush(Color.Parse("#FF4B71"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}

public sealed class InvulnerabilityToOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool && (bool)value)
        {
            return 0.45;
        }

        return 1d;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
