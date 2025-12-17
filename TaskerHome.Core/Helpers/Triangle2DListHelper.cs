using TaskerHome.Core.Renderers.Converters;
using TaskerHome.Core.Renderers.DataModel;
using TaskerHome.Core.Renderers.Filters;

namespace TaskerHome.Core.Helpers;

public static class Triangle2DListHelper
{
    public static List<Triangle2D> FilterInvisible(this List<Triangle2D> triangles) =>
            Task.Run(async () => await new Triangles2DInvisibleFilter().FilterAsync(triangles)).Result;

    public static List<Tile2DInfo> ToTile2DInfoList(this List<Triangle2D> triangles,
        int width, int height, int tilesHorizontal, int tilesVertical) => Task.Run(async () =>
        {
            var converter = new Triangle2DToTile2DInfoConverter(width, height, tilesHorizontal, tilesVertical);

            return await converter.ConvertAsync(triangles);
        }).Result;
}