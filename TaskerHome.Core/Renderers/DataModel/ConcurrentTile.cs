namespace TaskerHome.Core.Renderers.DataModel;

/// <summary>
/// Помощник для параллельного добавления треугольников в тайлы
/// </summary>
public class ConcurrentTile
{
    public Tile2DInfo Tile { get; }
    private readonly List<Triangle2D> _triangles = [];

    public IReadOnlyList<Triangle2D> Triangles
    {
        get
        {
            lock (_triangles)
            {
                return _triangles.ToList();
            }
        }
    }

    public ConcurrentTile(Tile2DInfo tile)
    {
        Tile = tile;
    }

    public void AddTriangle(Triangle2D triangle)
    {
        lock (_triangles)
        {
            _triangles.Add(triangle);
        }
    }
}