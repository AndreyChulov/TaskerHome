using System.Numerics;
//using Avalonia;
using SkiaSharp;
using TaskerHome.Core.Renderers.DataModel;
using TaskerHome.Core.Unsafe;

namespace TaskerHome.Core.Renderers.ZBuffer;

public class ParallelRasterizer : IDisposable
{
    private readonly object _lock = new();
    private readonly UnsafeRasterizer[] _threadBuffers;
    private readonly SKRect[] _tileRects;
    private readonly int _threadCount;

    public ParallelRasterizer(int width, int height, int threadCount = 4)
    {
        _threadCount = threadCount;
        _threadBuffers = new UnsafeRasterizer[threadCount];
        _tileRects = new SKRect[threadCount];

        int tileHeight = height / threadCount;
        for (int i = 0; i < threadCount; i++)
        {
            _threadBuffers[i] = new UnsafeRasterizer(width, tileHeight);
            _tileRects[i] = new SKRect(0, i * tileHeight, width, i * tileHeight + tileHeight);
        }
    }

    public void RenderTriangles(List<Triangle2D> triangles, SKCanvas canvas)
    {
        // Параллельная растеризация
        Parallel.For(0, _threadCount, i =>
        {
            var threadBuffer = _threadBuffers[i];
            var clipRect = _tileRects[i];

            foreach (var triangle in triangles)
            {
                if (TriangleIntersectsRect(triangle, clipRect))
                {
                    var localPoints = triangle.Points.Select(p =>
                        new Vector2(p.X, p.Y - clipRect.Top)).ToArray();

                    //threadBuffer.RasterizeTriangle(localPoints, triangle.ZValues, triangle.Color);
                }
            }
        });

        // Объединяем тайлы в единое изображение
        var info = new SKImageInfo(
            width: _threadBuffers[0].Width,
            height: _threadBuffers.Sum(b => b.Height),
            colorType: SKColorType.Rgba8888,
            alphaType: SKAlphaType.Premul
        );

        using var surface = SKSurface.Create(info) 
                            ?? throw new InvalidOperationException("Не удалось создать surface");

        using var canvasCombined = surface.Canvas;
        canvasCombined.Clear(SKColors.Transparent);

        for (int i = 0; i < _threadCount; i++)
        {
            var buffer = _threadBuffers[i];
            var tileInfo = new SKImageInfo(
                buffer.Width, 
                buffer.Height, 
                SKColorType.Rgba8888, 
                SKAlphaType.Premul
            );

            using var subBitmap = new SKBitmap();
            if (!subBitmap.TryAllocPixels(tileInfo))
            {
                throw new InvalidOperationException("Не удалось выделить память для тайла");
            }

            buffer.CopyToBitmap(subBitmap);

            var destRect = new SKRect(
                0, i * buffer.Height,
                buffer.Width, (i + 1) * buffer.Height
            );

            canvasCombined.DrawBitmap(subBitmap, destRect, new SKPaint());
        }

        using var image = surface.Snapshot();
        canvas.DrawImage(image, new SKPoint(0, 0), new SKPaint());
    }    
    
    private bool TriangleIntersectsRect(Triangle2D triangle2D, SKRect rect)
    {
        // Проверка, находится ли треугольник в пределах тайла
        return triangle2D.Points.Any(p => rect.Contains(p.X, p.Y));
    }
    
    public void Dispose()
    {
        foreach (var buffer in _threadBuffers)
        {
            buffer.Dispose();
        }
    }
}