using System.Collections.Concurrent;
using System.Numerics;
using SkiaSharp;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters;

public static class FaceToTriangle3DConverter
{
    // Основной метод конвертации с триангуляцией
    public static IEnumerable<Triangle3D> Convert(Face face)
    {
        if (face.Vertices == null)
            throw new ArgumentNullException(nameof(face.Vertices));
            
        if (face.Vertices.Length < 3)
            throw new ArgumentException("Face должен содержать минимум 3 вершины");

        var triangles = new List<Triangle3D>();

        // Простейшая триангуляция (fan) для выпуклых полигонов
        for (var i = 1; i < face.Vertices.Length - 1; i++)
        {
            var triangle = Triangle3D.Create(
                face.Vertices[0], 
                face.Vertices[i], 
                face.Vertices[i + 1], 
                face.Color);
            
            // Проверка вырожденности (вычисления происходят только при необходимости)
            if (!triangle.IsDegenerate)
                triangles.Add(triangle);
        }
        
        return triangles;
    }

    // Асинхронная параллельная конвертация
    public static async Task<IEnumerable<Triangle3D>> ConvertAsync(
        IEnumerable<Face> faces, 
        int maxDegreeParallelism = 4)
    {
        var result = new ConcurrentBag<Triangle3D>();
        
        await Parallel.ForEachAsync(faces, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeParallelism }, 
            (face, token) =>
        {
            foreach (var triangle in Convert(face))
            {
                result.Add(triangle);
            }
            return ValueTask.CompletedTask;
        });
        
        return result;
    }
}