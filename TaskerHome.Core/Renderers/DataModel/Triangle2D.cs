using System.Numerics;
using SkiaSharp;

namespace TaskerHome.Core.Renderers.DataModel;

public class Triangle2D
{
    public required Vector2[] Points { get; init; }
    public required SKColor Color { get; init; }
    public required Vector3 Normal { get; init; }
}