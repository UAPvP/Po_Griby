namespace Po_Griby.MapSystem
{
    class TileInfo
    {
        public ConsoleColor Color { get; private set; }
        public bool IsWalkable { get; private set; }
        public TileInfo(ConsoleColor color, bool isWalkable)
        {
            Color = color;
            IsWalkable = isWalkable;
        }

        public static Dictionary<char, TileInfo> LoadTiles(string file = "tiles.txt")
        {
            if (!File.Exists(file))
            {
                File.WriteAllText(file,
                    ".=Gray=true\n" +
                    "#=DarkGray=false\n" +
                    "~=Blue=false\n" +
                    "G=Green=true\n");
            }

            Dictionary<char, TileInfo> tiles = new();

            foreach (string line in File.ReadAllLines(file))
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                string[] parts = trimmedLine.Split('=');
                if (parts.Length != 3 || parts[0].Length != 1)
                    continue;
                char symbol = parts[0][0];

                if (!Enum.TryParse(parts[1], true, out ConsoleColor color))
                    continue;

                if (!bool.TryParse(parts[2], out bool isWalkable))
                    continue;

                tiles[symbol] = new TileInfo(color, isWalkable);
            }
            return tiles;
        }
    }
}