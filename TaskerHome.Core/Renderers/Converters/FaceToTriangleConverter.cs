using System.Numerics;
using SkiaSharp;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters;

public class FaceToTriangleConverter
{
    private readonly Camera _camera;
    private readonly int _width;
    private readonly int _height;
    private readonly float _zRange;

    public FaceToTriangleConverter(Camera camera, int width, int height)
    {
        _camera = camera;
        _width = width;
        _height = height;
        _zRange = camera.FarPlane - camera.NearPlane;
    }

    public IEnumerable<Triangle2D> ConvertFacesToTriangles(IEnumerable<Face> faces, Matrix4x4 modelMatrix)
    {
        var triangles = new List<Triangle2D>();
        var viewProjection = _camera.ViewMatrix * _camera.ProjectionMatrix;

        foreach (var face in faces)
        {
            // Применяем MVP к каждому многоугольнику
            for (int i = 1; i < face.Vertices.Length - 1; i++)
            {
                var v0 = face.Vertices[0];
                var v1 = face.Vertices[i];
                var v2 = face.Vertices[i + 1];

                triangles.Add(CreateTriangle(v0, v1, v2, face.Color, modelMatrix, viewProjection));
            }
        }

        // Сортировка по среднему Z (для ранней отсечки)
        //triangles = triangles.OrderByDescending(t => t.AverageZ).ToList();

        return triangles;
    }

    private Triangle2D CreateTriangle(
        Vector3 v1, Vector3 v2, Vector3 v3,
        SKColor color, Matrix4x4 modelMatrix, Matrix4x4 viewProjection)
    {
        // 1. Применяем модельную матрицу (M)
        var worldV1 = Vector4.Transform(v1, modelMatrix);
        var worldV2 = Vector4.Transform(v2, modelMatrix);
        var worldV3 = Vector4.Transform(v3, modelMatrix);

        // 2. Убираем W (т.к. это Vector4)
        worldV1 /= worldV1.W;
        worldV2 /= worldV2.W;
        worldV3 /= worldV3.W;

        // 3. Применяем ViewProjection (V * P)
        var projV1 = Vector4.Transform(worldV1, viewProjection);
        var projV2 = Vector4.Transform(worldV2, viewProjection);
        var projV3 = Vector4.Transform(worldV3, viewProjection);

        // 4. Perspective divide (W)
        projV1 /= projV1.W;
        projV2 /= projV2.W;
        projV3 /= projV3.W;

        // 5. Экранные координаты
        var s1 = new Vector2(
            (_width / 2f) * (projV1.X + 1),
            (_height / 2f) * (1 - projV1.Y)
        );
        var s2 = new Vector2(
            (_width / 2f) * (projV2.X + 1),
            (_height / 2f) * (1 - projV2.Y)
        );
        var s3 = new Vector2(
            (_width / 2f) * (projV3.X + 1),
            (_height / 2f) * (1 - projV3.Y)
        );

        // 6. Нормализация Z (Z ∈ [0, 1] для z-буфера)
        float z1 = NormalizeZ(worldV1.Z);
        float z2 = NormalizeZ(worldV2.Z);
        float z3 = NormalizeZ(worldV3.Z);

        return new Triangle2D
        {
            Points = new[] { s1, s2, s3 },
            Normal = Vector3.Zero,
            //ZValues = new[] { z1, z2, z3 },
            Color = color
        };
    }

    private float NormalizeZ(float z)
    {
        return (z - _camera.NearPlane) / _zRange;
    }
}