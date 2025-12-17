namespace TaskerHome.Core.Helpers;

public static class ZBufferHelper
{
    public static IEnumerable<T> SortByDepth<T>(
        this IEnumerable<T> elements, 
        Func<T, float> depthSelector)
    {
        return elements.OrderByDescending(depthSelector);
    }
}