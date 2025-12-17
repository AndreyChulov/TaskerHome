using System.Numerics;

namespace TaskerHome.Core.Transformations;

public static class Transform3D
{
    public static Matrix4x4 CreatePerspectiveFieldOfView(
        float fieldOfView, float aspect, float nearPlane, float farPlane)
    {
        float yScale = 1f / MathF.Tan(fieldOfView * 0.5f);
        float xScale = yScale / aspect;

        return new Matrix4x4(
            xScale, 0, 0, 0,
            0, yScale, 0, 0,
            0, 0, farPlane / (farPlane - nearPlane), 1,
            0, 0, -nearPlane * farPlane / (farPlane - nearPlane), 0);
    }

    public static Matrix4x4 CreateLookAt(Vector3 position, Vector3 target, Vector3 up)
    {
        Vector3 forward = Vector3.Normalize(position - target);
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
        Vector3 upVector = Vector3.Cross(forward, right);

        return new Matrix4x4(
            right.X, upVector.X, forward.X, 0,
            right.Y, upVector.Y, forward.Y, 0,
            right.Z, upVector.Z, forward.Z, 0,
            -Vector3.Dot(right, position), 
            -Vector3.Dot(upVector, position), 
            -Vector3.Dot(forward, position), 
            1);
    }
}