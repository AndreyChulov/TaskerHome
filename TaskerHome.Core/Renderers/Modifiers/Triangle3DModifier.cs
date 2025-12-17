using System.Collections.Concurrent;
using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Modifiers;

public static class Triangle3DModifier
{
    /// <summary>
    /// Асинхронное модифицирование коллекции треугольников с учетом камеры и/или матрицы преобразования
    /// </summary>
    public static async Task<IEnumerable<Triangle3D>> Modify(
        IEnumerable<Triangle3D> triangles,
        Camera? camera = null,
        Matrix4x4? modelMatrix = null,
        int maxDegreeOfParallelism = 4)
    {
        ArgumentNullException.ThrowIfNull(triangles);

        // Если нет параметров - возвращаем исходные треугольники
        if (camera == null && modelMatrix == null)
            return triangles.ToList();

        // Вычисляем общую матрицу преобразования
        var transform = GetTransformationMatrix(camera, modelMatrix);

        var result = new ConcurrentBag<Triangle3D>();

        await Parallel.ForEachAsync(triangles, 
            new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, 
            (triangle, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Преобразуем все точки
            var transformedPoints = new Vector3[3];
            for (var i = 0; i < 3; i++)
            {
                transformedPoints[i] = TransformPoint(triangle.Points[i], transform);
            }

            // Создаем новый треугольник
            result.Add(Triangle3D.Create(transformedPoints, triangle.Color));

            return ValueTask.CompletedTask;
        });

        return result;
    }

    /// <summary>
    /// Создает итоговую матрицу преобразования на основе входных параметров
    /// </summary>
    private static Matrix4x4 GetTransformationMatrix(Camera? camera, Matrix4x4? modelMatrix)
    {
        Matrix4x4 transform = Matrix4x4.Identity;

        if (modelMatrix.HasValue)
        {
            transform = modelMatrix.Value;
        }

        if (camera == null) return transform;
        
        // Сначала применяем модельную матрицу, потом view, потом projection
        transform = Matrix4x4.Multiply(transform, camera.ViewMatrix);
        transform = Matrix4x4.Multiply(transform, camera.ProjectionMatrix);

        return transform;
    }

    /// <summary>
    /// Преобразует точку с использованием матрицы преобразования
    /// </summary>
    private static Vector3 TransformPoint(Vector3 point, Matrix4x4 transform)
    {
        // Преобразуем точку с использованием матрицы 4x4
        var result = Vector4.Transform(point, transform);
        
        // Применяем перспективное деление
        if (result.W is 0 or 1) return new Vector3(result.X, result.Y, result.Z);
        
        result.X /= result.W;
        result.Y /= result.W;
        result.Z /= result.W;

        return new Vector3(result.X, result.Y, result.Z);
    }
}