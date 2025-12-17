using System.Numerics;
using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters.ProjectionStrategies;

public interface IProjectionStrategy
{
    Vector2 ProjectPoint(Vector3 point3D, Vector3 normal, Camera? camera = null);
}