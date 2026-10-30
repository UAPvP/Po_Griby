using System;
using System.Collections.Generic;
using Po_Griby.Gameplay;

namespace Po_Griby.MapSystem
{
    public class FinalMap
    {
        private readonly TileMap tileMap;
        private readonly EntityMap entityMap;
        private readonly Dictionary<char, TileInfo> tileDefinitions;
        private readonly Dictionary<char, EntityPlacement> entityDefinitions;

        public string LastMessage { get; private set; } = string.Empty;

        public FinalMap(
            string tileMapFilePath,
            string entityMapFilePath,
            string tileDefinitionsFilePath = "tiles.txt",
            string entityDefinitionsFilePath = "entity-definitions.txt")
        {
            tileMap = new TileMap(tileMapFilePath);
            tileDefinitions = TileInfo.LoadTiles(tileDefinitionsFilePath);
            entityDefinitions = EntityPlacement.Load(entityDefinitionsFilePath);

            entityMap = new EntityMap(
                tileMap,
                tileDefinitions,
                entityDefinitions,
                entityMapFilePath);
        }
        public EntityMap GetEntityMap()
        {
            return entityMap;
        }
        public TileMap GetTileMap()
        {
            return tileMap;
        }

        public bool IsTileWalkable(Vector2D position, bool isNPC = false)
        {
            char[,] tiles = tileMap.GetTileMap();

            if (position.x < 0 || position.y < 0 ||
                position.x >= tiles.GetLength(1) ||
                position.y >= tiles.GetLength(0))
            {
                return false;
            }

            char tileSymbol = tiles[position.y, position.x];

            if (!tileDefinitions.TryGetValue(tileSymbol, out TileInfo? tileInfo))
                return false;

            return isNPC ? tileInfo.IsNPCWalkable : tileInfo.IsWalkable;
        }

        public Entity? GetEntityAt(Vector2D position)
        {
            foreach (Entity entity in entityMap.Entities)
            {
                if (entity.Position.x == position.x &&
                    entity.Position.y == position.y)
                {
                    return entity;
                }
            }

            return null;
        }

        public void SetMessage(string? message)
        {
            LastMessage = message ?? string.Empty;
        }

        public bool isPassable(Vector2D position, bool isNPC = false)
        {
            return IsTileWalkable(position, isNPC) && GetEntityAt(position) == null;
        }
        public void Show()
        {
            char[,] tiles = tileMap.GetTileMap();
            int height = tiles.GetLength(0);
            int width = tiles.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Entity? entity = GetEntityAt(new Vector2D(x, y));

                    if (entity != null)
                    {
                        Console.ForegroundColor = entityDefinitions[entity.Symbol].Color;
                        Console.Write(entity.Symbol);
                    }
                    else
                    {
                        char tileSymbol = tiles[y, x];
                        Console.ForegroundColor = tileDefinitions[tileSymbol].Color;
                        Console.Write(tileSymbol);
                    }
                }

                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ResetColor();
        }

    }
}
