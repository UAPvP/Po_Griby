namespace Po_Griby.MapSystem
{
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

        public static Dictionary<char, TileInfo> LoadTiles(string file = "tiles.txt")
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file,
                    ".=Gray,true,true\n" +
                    "#=DarkGray,false,false\n" +
                    "~=Blue,false,false\n" +
                    "G=Green,true,true\n" +
                    "^=DarkYellow,true,false\n");
            }

            Dictionary<char, TileInfo> tiles = new();

            foreach (string line in File.ReadAllLines(file))
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                string[] entry = trimmedLine.Split('=', 2);
                if (entry.Length != 2 || entry[0].Trim().Length != 1)
                    throw new FormatException($"Неправильний формат тайлу: {line}");

                char symbol = entry[0].Trim()[0];
                string[] parts = entry[1].Split(',', 3);

                if (parts.Length != 3)
                    throw new FormatException(
                        $"Для тайлу '{symbol}' очікується формат символ=колір,IsWalkable,IsNPCWalkable.");

                if (!Enum.TryParse(parts[0].Trim(), true, out ConsoleColor color))
                    throw new FormatException($"Невідомий колір тайлу: {parts[0].Trim()}");

                if (!bool.TryParse(parts[1].Trim(), out bool isWalkable))
                    throw new FormatException($"Неправильне значення IsWalkable у тайлу '{symbol}'.");

                if (!bool.TryParse(parts[2].Trim(), out bool isNPCWalkable))
                    throw new FormatException($"Неправильне значення IsNPCWalkable у тайлу '{symbol}'.");

                if (isNPCWalkable && !isWalkable)
                    throw new FormatException(
                        $"Тайл '{symbol}' не може бути прохідним для NPC і непрохідним для гравця.");

                tiles[symbol] = new TileInfo(color, isWalkable, isNPCWalkable);
            }
            return tiles;
        }
    }
}
