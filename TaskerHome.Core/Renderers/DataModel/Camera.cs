using System.Numerics;

namespace TaskerHome.Core.Renderers.DataModel;

public class Camera
{
    public Vector3 Position { get; set; }
    public Vector3 LookDirection { get; set; }
    public Vector3 UpDirection { get; set; } = Vector3.UnitY;
    public float FieldOfView { get; set; } = MathF.PI / 4; // 45 градусов
    public float AspectRatio { get; set; }
    public float NearPlane { get; set; } = 0.1f;
    public float FarPlane { get; set; } = 1000f;

    public Matrix4x4 ViewMatrix => Matrix4x4.CreateLookAt(Position, Position + LookDirection, UpDirection);
    public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(
        FieldOfView, AspectRatio, NearPlane, FarPlane);
}