using System.Numerics;
using SkiaSharp;

namespace TaskerHome.Core.Renderers.DataModel;

public class Face
{
    public required Vector3[] Vertices { get; init; }
    public required SKColor Color { get; init; }
}