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

            char[,] passabilityLayer =
                CreatePassabilityLayer(tiles, tileDefinitions);

            if (!File.Exists(filePath))
            {
                SaveLayer(filePath, passabilityLayer);
            }
            else
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        bool outsideMap = y >= height || x >= width;

                        if (outsideMap && IsEntitySymbol(lines[y][x]))
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
                        char expectedPassability = passabilityLayer[y, x];

                        if (IsEntitySymbol(symbol))
                        {
                            if (expectedPassability == '#')
                            {
                                throw new FormatException(
                                    $"Сутність '{symbol}' розташована на непрохідній клітинці ({x}, {y}).");
                            }

                            if (!entityDefinitions.ContainsKey(symbol))
                            {
                                throw new FormatException(
                                    $"Для символу сутності '{symbol}' немає опису.");
                            }
                        }
                        else if (symbol != expectedPassability)
                        {
                            layerChanged = true;
                        }
                    }
                }

                if (layerChanged)
                {
                    SaveLayer(filePath, passabilityLayer);
                }
            }
            string[] finalLines = File.ReadAllLines(filePath);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = finalLines[y][x];

                    if (!IsEntitySymbol(symbol))
                        continue;

                    EntityPlacement definition = entityDefinitions[symbol];
                    Vector2D position = new Vector2D(x, y);

                    entities.Add(definition.CreateEntity(position));
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

                    layer[y, x] = tileInfo.IsWalkable ? '.' : '#';
                }
            }

            return layer;
        }

        private static bool IsEntitySymbol(char symbol)
        {
            return symbol != '#' && symbol != '.';
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

            File.WriteAllLines(filePath, lines);
        }
    }
}