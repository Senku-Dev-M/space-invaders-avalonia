using CommunityToolkit.Mvvm.ComponentModel;

namespace SpaceInvaders.Models.Entities;

public abstract partial class GameEntity : ObservableObject
{
    protected GameEntity(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Guid Id { get; } = Guid.NewGuid();

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    public double Width { get; }

    public double Height { get; }

    public double Left
    {
        get { return X; }
    }

    public double Right
    {
        get { return X + Width; }
    }

    public double Top
    {
        get { return Y; }
    }

    public double Bottom
    {
        get { return Y + Height; }
    }
}
