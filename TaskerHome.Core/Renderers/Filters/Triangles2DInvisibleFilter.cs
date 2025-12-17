using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Filters;

public class Triangles2DInvisibleFilter
{
    // Малое значение для сравнения с учетом погрешности вычислений
    private const float Epsilon = 1e-6f;

    #region Асинхронные методы фильтрации

    /// <summary>
    /// Асинхронно фильтрует невидимые треугольники на основе Z-компоненты нормали
    /// </summary>
    public async Task<List<Triangle2D>> FilterAsync(
        List<Triangle2D> triangles, 
        CancellationToken cancellationToken = default)
    {
        if (triangles == null)
            throw new ArgumentNullException(nameof(triangles));

        // Выносим фильтрацию в отдельный поток
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return triangles
                .Where(t => 
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return t.Normal.Z > -Epsilon;
                })
                .ToList();
        }, cancellationToken);
    }

    /// <summary>
    /// Асинхронно фильтрует треугольники с учетом пользовательского направления камеры
    /// </summary>
    public async Task<List<Triangle2D>> FilterWithCustomViewDirectionAsync(
        List<Triangle2D> triangles, 
        Vector3 viewDirection, 
        CancellationToken cancellationToken = default)
    {
        if (triangles == null)
            throw new ArgumentNullException(nameof(triangles));
        
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Нормализуем вектор в отдельном потоке
            var normalizedView = Vector3.Normalize(viewDirection);
            var dotThreshold = -Epsilon;

            return triangles
                .Where(t => 
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return Vector3.Dot(t.Normal, normalizedView) > dotThreshold;
                })
                .ToList();
        }, cancellationToken);
    }

    #endregion

    #region Параллельная фильтрация (PLINQ)

    /// <summary>
    /// Асинхронно фильтрует треугольники с параллельной обработкой (для больших списков)
    /// </summary>
    public async Task<List<Triangle2D>> ParallelFilterAsync(
        List<Triangle2D> triangles, 
        int maxDegreeOfParallelism = 4, 
        CancellationToken cancellationToken = default)
    {
        if (triangles == null)
            throw new ArgumentNullException(nameof(triangles));

        return await Task.Run(() =>
        {
            return triangles
                .AsParallel()
                .WithDegreeOfParallelism(maxDegreeOfParallelism)
                .WithCancellation(cancellationToken)
                .Where(t => 
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return t.Normal.Z > -Epsilon;
                })
                .ToList();
        }, cancellationToken);
    }

    #endregion

    #region Обработка ошибок и отмена

    /// <summary>
    /// Фильтрует треугольники с возвратом результата в виде ValueTask
    /// </summary>
    public ValueTask<List<Triangle2D>> TryFilterAsync(
        List<Triangle2D> triangles, 
        CancellationToken cancellationToken = default)
    {
        if (triangles == null)
            throw new ArgumentNullException(nameof(triangles));

        return new ValueTask<List<Triangle2D>>(Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return triangles
                .Where(t => 
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return t.Normal.Z > -Epsilon;
                })
                .ToList();
        }, cancellationToken));
    }

    #endregion
}