using System.Numerics;
using SkiaSharp;

namespace TaskerHome.Core.Renderers.DataModel;

public class Triangle3D
{
    private readonly Lazy<Vector3> _normal;
    private readonly Lazy<bool> _isDegenerate;
    
    /// <summary>
    /// Точки треугольника
    /// </summary>
    public required Vector3[] Points { get; init; }
    
    /// <summary>
    /// Цвет заполнения треугольника
    /// </summary>
    public required SKColor Color { get; init; }
    
    /// <summary>
    /// Нормаль треугольника (вычисляется при первом обращении)
    /// </summary>
    public Vector3 Normal => _normal.Value;

    /// <summary>
    /// Проверка на вырожденный треугольник
    /// </summary>
    public bool IsDegenerate => _isDegenerate.Value;

    // Создание треугольника с 3D точками
    private Triangle3D()
    {
        // Инициализация вычисления нормали с проверкой
        _normal = new Lazy<Vector3>(ConstructNormal, LazyThreadSafetyMode.PublicationOnly);

        // Инициализация проверки на вырожденность с проверкой
        _isDegenerate = new Lazy<bool>(ConstructIsDegenerate, LazyThreadSafetyMode.PublicationOnly);
    }
    
    public static Triangle3D Create(Vector3 v1, Vector3 v2, Vector3 v3, SKColor color) => 
        new()
        {
            Points = [v1, v2, v3],
            Color = color
        };

    public static Triangle3D Create(Vector3[] points, SKColor color)
    {
        Triangle3D triangle = new()
           {
               Points = points,
               Color = color
           };
        
        triangle.ValidatePointsCount();
        
        return triangle;
    }
        
    
    private bool ConstructIsDegenerate()
    {
        ValidatePointsCount();

        // Получаем нормаль, которая в свою очередь проверит корректность точек
        var normal = _normal.Value;
        return normal.LengthSquared() < 1e-6f;
    }

    private void ValidatePointsCount()
    {
        // Сначала проверяем инициализацию точек
        if (Points is not { Length: 3 })
        {
            throw new InvalidOperationException(
                "Точки треугольника должны быть инициализированы (ровно 3 точки) перед проверкой на вырожденность");
        }
    }

    private Vector3 ConstructNormal()
    {
        ValidatePointsCount();

        var v1 = Points[1] - Points[0];
        var v2 = Points[2] - Points[0];
        
        return Vector3.Cross(v1, v2);
    }
}