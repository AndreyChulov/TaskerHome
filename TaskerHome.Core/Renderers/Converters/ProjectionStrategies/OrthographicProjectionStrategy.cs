using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters.ProjectionStrategies;

public class OrthographicProjectionStrategy : IProjectionStrategy
{
    public Vector2 ProjectPoint(Vector3 point3D, Vector3 normal, Camera? camera = null)
    {
        // Простая ортография: отбрасываем Z-координату
        return new Vector2(point3D.X, point3D.Y);
    }
}