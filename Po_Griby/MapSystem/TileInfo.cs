using System.Text.Json;
using System.Text.Json.Serialization;

namespace Po_Griby.MapSystem
{
    public class TileDefinitionsFile
    {
        public List<TileDefinition> Tiles { get; set; } = new();
    }

    public class TileDefinition
    {
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsNPCWalkable { get; set; }
    }

    public class TileInfo
    {

        public ConsoleColor Color { get; private set; }
        public bool IsWalkable { get; private set; }
        public bool IsNPCWalkable { get; private set; }

        public TileInfo(ConsoleColor color, bool isWalkable, bool isNPCWalkable)
        {
            Color = color;
            IsWalkable = isWalkable;
            IsNPCWalkable = isNPCWalkable;
        }

        public static Dictionary<char, TileInfo> LoadTiles(string file = "tiles.json")
        {
            if (!File.Exists(file))
            {
                var defaults = new TileDefinitionsFile
                {
                    Tiles =
                    {
                        new() { Symbol = '.', Color = ConsoleColor.Gray, IsWalkable = true, IsNPCWalkable = true },
                        new() { Symbol = '#', Color = ConsoleColor.DarkGray, IsWalkable = false, IsNPCWalkable = false },
                        new() { Symbol = '~', Color = ConsoleColor.Blue, IsWalkable = false, IsNPCWalkable = false },
                        new() { Symbol = 'G', Color = ConsoleColor.Green, IsWalkable = true, IsNPCWalkable = true },
                        new() { Symbol = '^', Color = ConsoleColor.DarkYellow, IsWalkable = true, IsNPCWalkable = false }
                    }
                };
                SaveTiles(file, defaults);
            }

            string json = File.ReadAllText(file);
            var data = JsonSerializer.Deserialize<TileDefinitionsFile>(json)
                ?? throw new InvalidDataException("Файл визначень тайлів порожній.");

            var tiles = new Dictionary<char, TileInfo>();
            foreach (TileDefinition definition in data.Tiles)
            {
                if (!tiles.TryAdd(definition.Symbol, new TileInfo(definition.Color, definition.IsWalkable, definition.IsNPCWalkable)))
                    throw new InvalidDataException($"Символ тайлу '{definition.Symbol}' указано кілька разів.");
            }
            return tiles;
        }

        private static void SaveTiles(string file, TileDefinitionsFile data)
        {
            File.WriteAllText(file, JsonSerializer.Serialize(data));
        }
    }
}
