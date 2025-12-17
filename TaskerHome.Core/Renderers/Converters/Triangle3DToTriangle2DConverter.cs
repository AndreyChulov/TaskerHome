using System.Collections.Concurrent;
using TaskerHome.Core.Renderers.Converters.ProjectionStrategies;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters;

public class Triangle3DToTriangle2DConverter(
    IProjectionStrategy projectionStrategy,
    Camera camera)
{
    public Triangle2D Convert(Triangle3D triangle)
    {
        // Проверка на вырожденность
        if (triangle.IsDegenerate)
        {
            throw new ArgumentException("Cannot convert degenerate triangle");
        }

        // Проекция всех вершин
        var points2D = triangle.Points
            .Select(p => projectionStrategy.ProjectPoint(p, triangle.Normal, camera))
            .ToArray();

        return new Triangle2D
        {
            Points = points2D,
            Color = triangle.Color,
            Normal = triangle.Normal
        };
    }
    
    public async Task<Triangle2D> ConvertAsync(Triangle3D triangle, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
    
        if (triangle.IsDegenerate)
        {
            throw new ArgumentException("Cannot convert degenerate triangle");
        }

        var points2D = await Task.Run(() => 
                triangle.Points
                    .Select(p => projectionStrategy.ProjectPoint(p, triangle.Normal, camera))
                    .ToArray(), 
            ct
        );

        return new Triangle2D
        {
            Points = points2D,
            Color = triangle.Color,
            Normal = triangle.Normal
        };
    }
    
    public async Task<List<Triangle2D>> ConvertRangeAsync(
        IEnumerable<Triangle3D> triangles,
        CancellationToken ct = default,
        ParallelOptions? parallelOptions = null)
    {
        ArgumentNullException.ThrowIfNull(triangles);

        parallelOptions ??= new ParallelOptions { CancellationToken = ct, MaxDegreeOfParallelism = Environment.ProcessorCount };

        var results = new ConcurrentBag<Triangle2D>();

        try
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(triangles, parallelOptions, triangle =>
                {
                    ct.ThrowIfCancellationRequested();
                    var converted = Convert(triangle); // Синхронная конвертация
                    results.Add(converted);
                });
            }, ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Логика на случай отмены
            return new List<Triangle2D>();
        }

        return results.ToList();
    }
}