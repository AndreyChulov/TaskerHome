using TaskerHome.Core.Renderers.DataModel;

namespace TaskerHome.Core.Renderers.Converters
{
    public class Triangle2DToTile2DInfoConverter
    {
        private readonly int _canvasWidth;
        private readonly int _canvasHeight;
        private readonly int _tilesHorizontal;
        private readonly int _tilesVertical;

        // Размер одного тайла
        private readonly int _tileWidth;
        private readonly int _tileHeight;

        public Triangle2DToTile2DInfoConverter(
            int canvasWidth,
            int canvasHeight,
            int tilesHorizontal,
            int tilesVertical)
        {
            // Проверки на корректность входных данных
            if (canvasWidth <= 0 || canvasHeight <= 0)
                throw new ArgumentException("Canvas dimensions must be positive.");

            if (tilesHorizontal <= 0 || tilesVertical <= 0)
                throw new ArgumentException("Tile counts must be positive.");

            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
            _tilesHorizontal = tilesHorizontal;
            _tilesVertical = tilesVertical;

            _tileWidth = canvasWidth / tilesHorizontal;
            _tileHeight = canvasHeight / tilesVertical;
        }

        /// <summary>
        /// Синхронная конвертация треугольников в тайлы
        /// </summary>
        public List<Tile2DInfo> Convert(List<Triangle2D> triangles)
        {
            var tiles = InitializeTiles();

            foreach (var triangle in triangles)
            {
                foreach (var tile in tiles.Where(tile => DoesTriangleIntersectTile(triangle, tile)))
                {
                    tile.Triangles.Add(triangle);
                }
            }

            return tiles;
        }

        /// <summary>
        /// Асинхронная конвертация с параллельной обработкой треугольников
        /// </summary>
        public async Task<List<Tile2DInfo>> ConvertAsync(List<Triangle2D> triangles)
        {
            var tiles = InitializeTiles();

            // Используем ConcurrentBag для потокобезопасного заполнения
            var concurrentTiles = tiles
                .Select(tile => new ConcurrentTile(tile))
                .ToList();

            // Распараллеливаем обработку треугольников
            await Parallel.ForEachAsync(triangles, async (triangle, ct) =>
            {
                foreach (var concurrentTile in 
                            concurrentTiles.Where(
                                concurrentTile => DoesTriangleIntersectTile(triangle, concurrentTile.Tile)
                            )
                        )
                {
                    concurrentTile.AddTriangle(triangle);
                }
            });

            // Копируем результаты обратно в исходные тайлы
            for (var i = 0; i < tiles.Count; i++)
            {
                tiles[i].Triangles.AddRange(concurrentTiles[i].Triangles);
            }

            return tiles;
        }

        /// <summary>
        /// Инициализация всех тайлов
        /// </summary>
        private List<Tile2DInfo> InitializeTiles()
        {
            var tiles = new List<Tile2DInfo>();

            for (var y = 0; y < _tilesVertical; y++)
            {
                for (var x = 0; x < _tilesHorizontal; x++)
                {
                    var left = x * _tileWidth;
                    var top = y * _tileHeight;

                    tiles.Add(Tile2DInfo.Create(left, top, _tileWidth, _tileHeight));
                }
            }

            return tiles;
        }

        /// <summary>
        /// Проверка пересечения треугольника с тайлом
        /// </summary>
        private bool DoesTriangleIntersectTile(Triangle2D triangle, Tile2DInfo tile)
        {
            // Просто AABB проверка: если хотя бы одна точка треугольника внутри тайла — считаем пересечение
            return triangle.Points.Any(
                point => tile.Left <= point.X && point.X < tile.Right && tile.Top <= point.Y && point.Y < tile.Bottom);

            // Также можно реализовать более точную проверку, например, SAT или Minkowski sum
        }
    }
}