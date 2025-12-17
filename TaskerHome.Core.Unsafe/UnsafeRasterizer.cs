using System.Numerics;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace TaskerHome.Core.Unsafe;

public unsafe class UnsafeRasterizer
{
    public int Width { get; }
    public int Height { get; }

    private byte* _colorBuffer;
    private float* _zBuffer;

    public UnsafeRasterizer(int width, int height)
    {
        Width = width;
        Height = height;

        // Выделяем неуправляемую память
        _colorBuffer = (byte*)Marshal.AllocHGlobal(width * height * 4);
        _zBuffer = (float*)Marshal.AllocHGlobal(width * height * sizeof(float));

        // Инициализируем буферы
        InitializeBuffers();
    }

    private void InitializeBuffers()
    {
        Parallel.For(0, Height, y =>
        {
            for (int x = 0; x < Width; x++)
            {
                int index = y * Width + x;
                _zBuffer[index] = float.PositiveInfinity;

                // RGBA (черный цвет, непрозрачный)
                _colorBuffer[index * 4] = 0;      // R
                _colorBuffer[index * 4 + 1] = 0;  // G
                _colorBuffer[index * 4 + 2] = 0;  // B
                _colorBuffer[index * 4 + 3] = 255; // A
            }
        });
    }

    public void RasterizeTriangle(Vector2[] points, float[] zValues, SKColor color)
    {
        if (points.Length != 3 || zValues.Length != 3)
            return;

        // Сортируем точки по Y для оптимизации
        var indices = new int[] { 0, 1, 2 };
        Array.Sort(indices, (i, j) => points[i].Y.CompareTo(points[j].Y));

        Vector2 v0 = points[indices[0]];
        Vector2 v1 = points[indices[1]];
        Vector2 v2 = points[indices[2]];

        float z0 = zValues[indices[0]];
        float z1 = zValues[indices[1]];
        float z2 = zValues[indices[2]];

        // Вычисляем коэффициенты
        float area = EdgeFunction(v1 - v0, v2 - v0);

        if (area == 0)
            return;

        // Вычисляем ограничивающий прямоугольник
        int minY = (int)MathF.Floor(MathF.Min(MathF.Min(v0.Y, v1.Y), v2.Y));
        int maxY = (int)MathF.Ceiling(MathF.Max(MathF.Max(v0.Y, v1.Y), v2.Y));
        int minX = (int)MathF.Floor(MathF.Min(MathF.Min(v0.X, v1.X), v2.X));
        int maxX = (int)MathF.Ceiling(MathF.Max(MathF.Max(v0.X, v1.X), v2.X));

        for (int y = minY; y <= maxY; y++)
        {
            if (y < 0 || y >= Height)
                continue;

            for (int x = minX; x <= maxX; x++)
            {
                if (x < 0 || x >= Width)
                    continue;

                Vector2 p = new Vector2(x + 0.5f, y + 0.5f); // Центр пикселя

                // Вычисляем барицентрические координаты
                float w0 = EdgeFunction(v1 - v0, p - v0) / area;
                float w1 = EdgeFunction(v2 - v1, p - v1) / area;
                float w2 = EdgeFunction(v0 - v2, p - v2) / area;

                if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                {
                    float z = z0 * w0 + z1 * w1 + z2 * w2;
                    int pixelIndex = y * Width + x;

                    if (z < _zBuffer[pixelIndex])
                    {
                        _zBuffer[pixelIndex] = z;

                        // BGRA (Skia использует Bgra8888)
                        _colorBuffer[pixelIndex * 4] = color.Blue;
                        _colorBuffer[pixelIndex * 4 + 1] = color.Green;
                        _colorBuffer[pixelIndex * 4 + 2] = color.Red;
                        _colorBuffer[pixelIndex * 4 + 3] = color.Alpha;
                    }
                }
            }
        }
    }

    private static float EdgeFunction(Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }    
    
    public void CopyToBitmap(SKBitmap bitmap)
    {
        var info = new SKImageInfo(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);


        if (bitmap.IsEmpty || bitmap.IsNull || bitmap.Info != info)
        {
            // Если формат не совпадает или pixmap не доступен, пересоздаем битмап
            using var oldBitmap = new SKBitmap(info);
            if (!bitmap.TryAllocPixels(info))
            {
                throw new InvalidOperationException("Не удалось выделить память для нового формата");
            }
        }

        // Получаем pixmap битмапа
        using var pixmap = bitmap.PeekPixels();

        // Безопасное копирование данных
        IntPtr pixelData = pixmap.GetPixels();
        if (pixelData == IntPtr.Zero)
            throw new InvalidOperationException("Не удалось получить пиксели pixmap");

        // Копируем данные из неуправляемого буфера в pixmap
        Buffer.MemoryCopy(_colorBuffer, pixelData.ToPointer(), Width * Height * 4, Width * Height * 4);
    }
    
    public void Dispose()
    {
        Marshal.FreeHGlobal(new IntPtr(_colorBuffer));
        Marshal.FreeHGlobal(new IntPtr(_zBuffer));
    }
}