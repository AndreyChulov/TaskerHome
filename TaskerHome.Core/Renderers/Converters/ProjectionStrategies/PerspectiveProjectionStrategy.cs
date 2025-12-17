using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters.ProjectionStrategies;

public class PerspectiveProjectionStrategy(int width, int height) : IProjectionStrategy
{
    public Vector2 ProjectPoint(Vector3 point3D, Vector3 normal, Camera? camera = null)
    {
        // 1. Применяем матрицу проекции камеры
        var view = camera?.ViewMatrix ?? Matrix4x4.Identity;
        var projection = camera?.ProjectionMatrix ?? Matrix4x4.Identity;
        var viewProjection = view * projection;
        var projected = Vector4.Transform(point3D, viewProjection);
        
        // 2. Перспективное деление
        if (projected.W != 0)
        {
            projected.X /= projected.W;
            projected.Y /= projected.W;
            projected.Z /= projected.W;
        }
        
        // 3. Преобразование в экранные координаты
        var x = (projected.X + 1) * 0.5f * width;
        var y = (-projected.Y + 1) * 0.5f * height;
        
        return new Vector2(x, y);
    }
}