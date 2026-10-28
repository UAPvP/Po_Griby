using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Po_Griby.Gameplay;

namespace Po_Griby.MapSystem
{
    public class EntityMap
    {
        private readonly List<Entity> entities = new();

        public IReadOnlyList<Entity> Entities => entities;

        public EntityMap(
            TileMap tileMap,
            Dictionary<char, TileInfo> tileDefinitions,
            Dictionary<char, EntityPlacement> entityDefinitions,
            string filePath)
        {
            char[,] tiles = tileMap.GetTileMap();
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);
            char[,] passabilityLayer = CreatePassabilityLayer(tiles, tileDefinitions);

            if (!File.Exists(filePath))
            {
                SaveLayer(filePath, passabilityLayer);
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);

                // Если сущность находится за границей новой карты, не затираем её молча.
                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        if ((y >= height || x >= width) && IsEntitySymbol(lines[y][x]))
                        {
                            throw new FormatException(
                                $"Сутність '{lines[y][x]}' розташована за межами карти: ({x}, {y}).");
                        }
                    }
                }

                bool layerChanged = lines.Length != height;

                for (int y = 0; y < Math.Min(lines.Length, height); y++)
                {
                    if (lines[y].Length != width)
                        layerChanged = true;

                    int overlapWidth = Math.Min(lines[y].Length, width);

                    for (int x = 0; x < overlapWidth; x++)
                    {
                        char symbol = lines[y][x];
                        char expected = passabilityLayer[y, x];

                        if (IsEntitySymbol(symbol))
                        {
                            if (expected == '#')
                            {
                                throw new FormatException(
                                    $"Сутність '{symbol}' розташована на непрохідному для гравця тайлі ({x}, {y}).");
                            }

                            if (!entityDefinitions.ContainsKey(symbol))
                            {
                                throw new FormatException(
                                    $"Для символу сутності '{symbol}' немає опису.");
                            }
                        }
                        else if (symbol != expected)
                        {
                            // Изменились размеры или проходимость клетки.
                            layerChanged = true;
                        }
                    }
                }

                if (layerChanged)
                    SaveLayer(filePath, passabilityLayer);
            }

            string[] finalLines = File.ReadAllLines(filePath);

            EnsurePlayerExists(finalLines, passabilityLayer, entityDefinitions, filePath);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = finalLines[y][x];

                    if (!IsEntitySymbol(symbol))
                        continue;

                    EntityPlacement definition = entityDefinitions[symbol];
                    entities.Add(definition.CreateEntity(new Vector2D(x, y)));
                }
            }
        }

        private static char[,] CreatePassabilityLayer(
            char[,] tiles,
            Dictionary<char, TileInfo> tileDefinitions)
        {
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);
            char[,] layer = new char[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char tileSymbol = tiles[y, x];

                    if (!tileDefinitions.TryGetValue(tileSymbol, out TileInfo? tileInfo))
                    {
                        throw new FormatException(
                            $"Для тайлу '{tileSymbol}' не задано опис прохідності.");
                    }

                    if (!tileInfo.IsWalkable)
                        layer[y, x] = '#';
                    else if (!tileInfo.IsNPCWalkable)
                        layer[y, x] = 'X';
                    else
                        layer[y, x] = '.';
                }
            }

            return layer;
        }

        private static void EnsurePlayerExists(
            string[] lines,
            char[,] passabilityLayer,
            Dictionary<char, EntityPlacement> entityDefinitions,
            string filePath)
        {
            EntityPlacement? playerDefinition = entityDefinitions.Values
                .FirstOrDefault(definition => definition.ClassName == "Player");

            if (playerDefinition == null)
            {
                throw new FormatException(
                    "У файлі entity-definitions.txt немає опису сутності класу Player.");
            }

            foreach (string line in lines)
            {
                foreach (char symbol in line)
                {
                    if (entityDefinitions.TryGetValue(symbol, out EntityPlacement? definition) &&
                        definition.ClassName == "Player")
                    {
                        return;
                    }
                }
            }

            int height = passabilityLayer.GetLength(0);
            int width = passabilityLayer.GetLength(1);
            int centerX = width / 2;
            int centerY = height / 2;

            if (IsFreePlayerCell(centerX, centerY, lines, passabilityLayer))
            {
                PlacePlayer(centerX, centerY, lines, playerDefinition.Symbol);
                SaveLines(filePath, lines);
                return;
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (IsFreePlayerCell(x, y, lines, passabilityLayer))
                    {
                        PlacePlayer(x, y, lines, playerDefinition.Symbol);
                        SaveLines(filePath, lines);
                        return;
                    }
                }
            }

            throw new FormatException("На карті немає вільної прохідної клітинки для гравця.");
        }

        private static bool IsFreePlayerCell(
            int x,
            int y,
            string[] lines,
            char[,] passabilityLayer)
        {
            if (passabilityLayer[y, x] == '#')
                return false;

            return !IsEntitySymbol(lines[y][x]);
        }

        private static void PlacePlayer(int x, int y, string[] lines, char playerSymbol)
        {
            char[] row = lines[y].ToCharArray();
            row[x] = playerSymbol;
            lines[y] = new string(row);
        }

        private static bool IsEntitySymbol(char symbol)
        {
            return symbol != '#' && symbol != '.' && symbol != 'X';
        }

        private static void SaveLayer(string filePath, char[,] layer)
        {
            int height = layer.GetLength(0);
            int width = layer.GetLength(1);
            string[] lines = new string[height];

            for (int y = 0; y < height; y++)
            {
                char[] row = new char[width];

                for (int x = 0; x < width; x++)
                    row[x] = layer[y, x];

                lines[y] = new string(row);
            }

            SaveLines(filePath, lines);
        }

        private static void SaveLines(string filePath, string[] lines)
        {
            File.WriteAllLines(filePath, lines);
        }
    }
}
