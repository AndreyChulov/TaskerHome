namespace TaskerHome.Core.Renderers.DataModel;

public class Tile2DInfo
{
    private readonly Lazy<int> _bottom;
    private readonly Lazy<int> _right;
    
    public required int Left { get; init; }
    public required int Top { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    
    public int Right => _right.Value;
    public int Bottom => _bottom.Value;
    
    public List<Triangle2D> Triangles { get; } = [];

    public static Tile2DInfo Create(int left, int top, int width, int height)
    {
        return new Tile2DInfo()
        {
            Left = left,
            Top = top,
            Width = width,
            Height = height,
        };
    }
    
    private Tile2DInfo()
    {
        _bottom = new Lazy<int>(ConstructBottom);
        _right = new Lazy<int>(ConstructRight);
    }

    private int ConstructRight()
    {
        ValidateIsInitialized();

        return Left + Width;
    }

    private int ConstructBottom()
    {
        ValidateIsInitialized();

        return Top + Height;
    }

    private void ValidateIsInitialized()
    {
        if (Left == 0 && Top == 0 && Width == 0 && Height == 0)
            throw new InvalidOperationException("Tile2DInfo must be initialized before it can be used.");
    }
}