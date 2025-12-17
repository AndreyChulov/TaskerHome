using System.Numerics;
using TaskerHome.Core.Renderers.Converters;
using TaskerHome.Core.Renderers.DataModel;
using TaskerHome.Core.Renderers.ZBuffer;

namespace TaskerHome.Core.Helpers;

public static class FaceEnumerableHelper
{
    private static IEnumerable<Triangle2D> ToTriangles(
        this IEnumerable<Face> faces, 
        Camera camera, 
        Matrix4x4 matrix,  
        int width, 
        int height)
    {
        var converter = new FaceToTriangleConverter(camera, width, height);
        
        return converter.ConvertFacesToTriangles(faces, matrix);
    }

    public static List<Triangle2D> ToTrianglesList(
        this IEnumerable<Face> faces, 
        Camera camera, 
        Matrix4x4 matrix,  
        int width, 
        int height) => faces.ToTriangles(camera, matrix, width, height).ToList();

    public static IEnumerable<Triangle3D> ToTriangles3D(this IEnumerable<Face> faces) 
        => Task.Run(async () => await FaceToTriangle3DConverter.ConvertAsync(faces)).Result;

    public static IEnumerable<Triangle3D> ToTriangles3DList(this IEnumerable<Face> faces) 
        => faces.ToTriangles3D().ToList();
}