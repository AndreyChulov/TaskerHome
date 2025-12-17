using System.Numerics;

namespace TaskerHome.Core.Helpers;

public static class Vector3Helper
{
    private static Vector4 ToVector4(this Vector3 v, float value4 = 0) => new(v, value4);
    /// <summary>
    /// - Matrix4x4 представляет аффинные преобразования (вращение, масштабирование, перемещение),
    /// которые требуют 4-мерного вектора для корректного применения. <br/>
    /// - Vector3 — это точка или вектор в 3D-пространстве, но для умножения на матрицу нужно
    ///  добавить 4-ю компоненту w: <br/>
    /// - w = 1 — для точек (чтобы учитывалось смещение). <br/>
    /// - w = 0 — для векторов (смещение игнорируется). <br/>
    /// </summary>
    /// <param name="v">Вектор для преобразования</param>
    /// <returns>Результирующий вектор</returns>
    public static Vector4 ToPointVector4(this Vector3 v) => v.ToVector4(1);
    /// <summary>
    /// - Matrix4x4 представляет аффинные преобразования (вращение, масштабирование, перемещение),
    /// которые требуют 4-мерного вектора для корректного применения. <br/>
    /// - Vector3 — это точка или вектор в 3D-пространстве, но для умножения на матрицу нужно
    ///  добавить 4-ю компоненту w: <br/>
    /// - w = 1 — для точек (чтобы учитывалось смещение). <br/>
    /// - w = 0 — для векторов (смещение игнорируется). <br/>
    /// </summary>
    /// <param name="v">Вектор для преобразования</param>
    /// <returns>Результирующий вектор</returns>
    public static Vector4 ToVectorVector4(this Vector3 v) => v.ToVector4();
}