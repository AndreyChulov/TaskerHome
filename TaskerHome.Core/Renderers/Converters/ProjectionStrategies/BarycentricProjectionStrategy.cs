using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters.ProjectionStrategies;

public class BarycentricProjectionStrategy : IProjectionStrategy
{
    public Vector2 ProjectPoint(Vector3 point3D, Vector3 normal, Camera? camera = null)
    {
        // Проверяем валидность нормали
        if (normal.LengthSquared() < 1e-6f)
            return new Vector2(point3D.X, point3D.Y); // Резервная проекция

        // Создаем ортонормированный базис в плоскости треугольника
        var uAxis = Vector3.Normalize(new Vector3(normal.Z, 0, -normal.X));
        if (uAxis.LengthSquared() < 1e-6f)
            uAxis = Vector3.UnitX;

        var vAxis = Vector3.Cross(normal, uAxis);
        vAxis = Vector3.Normalize(vAxis);

        // Преобразуем точку в локальные координаты плоскости
        var u = Vector3.Dot(point3D, uAxis);
        var v = Vector3.Dot(point3D, vAxis);

        return new Vector2(u, v);
    }
}