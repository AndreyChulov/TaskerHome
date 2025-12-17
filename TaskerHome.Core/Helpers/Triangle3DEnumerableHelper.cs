using System.Numerics;
using TaskerHome.Core.Renderers.Converters;
using TaskerHome.Core.Renderers.Converters.ProjectionStrategies;
using TaskerHome.Core.Renderers.DataModel;
using TaskerHome.Core.Renderers.Modifiers;

namespace TaskerHome.Core.Helpers;

public static class Triangle3DEnumerableHelper
{
    public static IEnumerable<Triangle3D> ModifyToTriangle3D(this IEnumerable<Triangle3D> triangles,
        Camera? camera = null, Matrix4x4? modelMatrix = null) =>
            Task.Run(async () => await Triangle3DModifier.Modify(triangles, camera, modelMatrix)).Result;

    public static List<Triangle3D> ModifyToTriangle3DList(this IEnumerable<Triangle3D> triangles,
        Camera? camera = null, Matrix4x4? modelMatrix = null) =>
            triangles.ModifyToTriangle3D(camera, modelMatrix).ToList();

    private static List<Triangle2D> ToTriangles2D(this IEnumerable<Triangle3D> triangles,
        IProjectionStrategy projectionStrategy, Camera camera) =>
            Task.Run(
                async () => 
                    await new Triangle3DToTriangle2DConverter(projectionStrategy, camera)
                        .ConvertRangeAsync(triangles)
                    )
                    .Result;

    public static List<Triangle2D> ToBarycentricTriangles2DList(this IEnumerable<Triangle3D> triangles, Camera camera) =>
            triangles.ToTriangles2D(new BarycentricProjectionStrategy(), camera);

    public static List<Triangle2D> ToOrthographicTriangles2DList(this IEnumerable<Triangle3D> triangles, Camera camera) =>
            triangles.ToTriangles2D(new OrthographicProjectionStrategy(), camera);

    public static List<Triangle2D> ToPerspectiveTriangles2DList(this IEnumerable<Triangle3D> triangles,
        Camera camera, int width, int height) =>
            triangles.ToTriangles2D(new PerspectiveProjectionStrategy(width, height), camera);

}