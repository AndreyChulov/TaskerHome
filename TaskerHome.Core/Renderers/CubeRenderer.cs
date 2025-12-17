using System.Numerics;
using Avalonia.Skia;
using SkiaSharp;
using TaskerHome.Core.Helpers;
using TaskerHome.Core.Renderers.DataModel;
using TaskerHome.Core.Renderers.ZBuffer;

namespace TaskerHome.Core.Renderers;

public class CubeRenderer
{
    private static readonly SKColor[] Colors =
    {
        new SKColor(255, 0, 0),   // Red
        new SKColor(0, 255, 0),   // Green
        new SKColor(0, 0, 255),   // Blue
        new SKColor(255, 255, 0), // Yellow
        new SKColor(255, 0, 255), // Magenta
        new SKColor(0, 255, 255)  // Cyan
    };
    
    private static readonly List<Face> Model =
    [
        new()
        {
            Vertices = [new Vector3(-1, -1, 1), new Vector3(1, -1, 1), new Vector3(1, 1, 1), new Vector3(-1, 1, 1)],
            Color = Colors[0]
        },
        // Задняя грань

        new()
        {
            Vertices = [new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(1, -1, -1)],
            Color = Colors[1]
        },
        // Соединяющие грани

        new()
        {
            Vertices = [new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), new Vector3(-1, 1, 1), new Vector3(-1, 1, -1)],
            Color = Colors[2]
        },

        new()
        {
            Vertices = [new Vector3(1, -1, -1), new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1)],
            Color = Colors[3]
        },

        new()
        {
            Vertices = [new Vector3(-1, -1, -1), new Vector3(1, -1, -1), new Vector3(1, -1, 1), new Vector3(-1, -1, 1)],
            Color = Colors[4]
        },

        new()
        {
            Vertices = [new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, -1)],
            Color = Colors[5]
        }
    ];

    private float _angle;

    public void Render(SKCanvas canvas, Matrix4x4 view, Matrix4x4 proj)
    {
        _angle += 0.01f;
        var world = Matrix4x4.CreateRotationY(_angle) * Matrix4x4.CreateRotationX(_angle * 0.7f);
        var rect = canvas.LocalClipBounds;
        var width = (int)rect.Width;
        var height = (int)rect.Height;
        var camera = new Camera
        {
            Position = new Vector3(0, 0, 5),
            LookDirection = new Vector3(0, 0, -1),
            AspectRatio = (float)width / height,
            NearPlane = 0.1f,
            FarPlane = 1000f
        };

        Model
            .ToTriangles3D()
            .ModifyToTriangle3D(camera, world)
            .ToBarycentricTriangles2DList(camera)
            .FilterInvisible()
            .ToTile2DInfoList(width, height, 4, 4)
            .
        
        //var faces = Model;
        //var baseModelTriangles3D - faces.ToTriangles3D()
        //var triangles = faces.ToTrianglesList(camera, world, width, height);

        using var rasterizer = new ParallelRasterizer(width, height);
        rasterizer.RenderTriangles(triangles, canvas);
        
        /*var facesWithDepth = faces.Select(f => new
        {
            Face = f,
            Depth = f.Vertices.Select(v => world.PointMultiply(v).Z).Average()
        }).OrderBy(f => f.Depth);

        foreach (var f in facesWithDepth)
        {
            DrawFace(canvas, f.Face, world, view, proj);
        }*/
    }

    private void DrawFace(SKCanvas canvas, Face face, Matrix4x4 world, Matrix4x4 view, Matrix4x4 proj)
    {
        using var paint = new SKPaint();
        paint.Color = face.Color;
        paint.Style = SKPaintStyle.Fill;
        paint.IsAntialias = true;

        var points = face.Vertices.Select(v =>
        {
            var pos = Vector4.Transform(new Vector4(v, 1), world * view * proj);
            return new SKPoint(
                (pos.X / pos.W + 1) * canvas.LocalClipBounds.Width / 2,
                (-pos.Y / pos.W + 1) * canvas.LocalClipBounds.Height / 2);
        }).ToArray();

        using var path = new SKPath();
        path.MoveTo(points[0]);
        path.LineTo(points[1]);
        path.LineTo(points[2]);
        path.LineTo(points[3]);
        path.Close();
        
        canvas.DrawPath(path, paint);
    }
}