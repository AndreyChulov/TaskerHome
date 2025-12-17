using System.Numerics;

namespace TaskerHome.Core.Helpers;

public static class Matrix4X4Helper
{
    /// <summary>
    /// - Matrix4x4 представляет аффинные преобразования (вращение, масштабирование, перемещение),
    /// которые требуют 4-мерного вектора для корректного применения. <br/>
    /// - Vector3 — это точка или вектор в 3D-пространстве, но для умножения на матрицу нужно
    ///  добавить 4-ю компоненту w: <br/>
    /// - w = 1 — для точек (чтобы учитывалось смещение). <br/>
    /// - w = 0 — для векторов (смещение игнорируется). <br/>
    /// </summary>
    /// <param name="v">Вектор для умножения</param>
    /// <param name="m">Матрица для умножения</param>
    /// <returns>Результирующий вектор</returns>
    public static Vector4 PointMultiply(this Matrix4x4 m, Vector3 v) => Vector4.Transform(v.ToPointVector4(), m);
    /// <summary>
    /// - Matrix4x4 представляет аффинные преобразования (вращение, масштабирование, перемещение),
    /// которые требуют 4-мерного вектора для корректного применения. <br/>
    /// - Vector3 — это точка или вектор в 3D-пространстве, но для умножения на матрицу нужно
    ///  добавить 4-ю компоненту w: <br/>
    /// - w = 1 — для точек (чтобы учитывалось смещение). <br/>
    /// - w = 0 — для векторов (смещение игнорируется). <br/>
    /// </summary>
    /// <param name="v">Вектор для умножения</param>
    /// <param name="m">Матрица для умножения</param>
    /// <returns>Результирующий вектор</returns>
    public static Vector4 VectorMultiply(this Matrix4x4 m, Vector3 v) => Vector4.Transform(v.ToVectorVector4(), m);
}