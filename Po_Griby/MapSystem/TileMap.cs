using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Po_Griby.MapSystem
{
    class TileMap
    {
        private char[,] map;
        private int width;
        private int height;

        public TileMap(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                height = lines.Length;
                width = lines.Length > 0 ? lines.Max(line => line.Length) : 0;

                map = new char[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (j < lines[i].Length)
                        {
                            map[i, j] = lines[i][j];
                        }
                        else
                        {
                            map[i, j] = ' ';
                        }
                    }
                }
            }
            else
            {
                CreateEmptyTileMap(32, 32);
            }
        }
        public TileMap(int height, int width)
        {
            this.height = height;
            this.width = width;

            map = new char[height, width];
        }
        public void CreateEmptyTileMap(int height, int width)
        {
            this.height = height;
            this.width = width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == 0 || i == height - 1 || j == 0 || j == width - 1)
                        map[i, j] = '#';
                    else
                        map[i, j] = ' ';
                }
            }
        }

        public void LoadTilesFromLines(string[] lines)
        {
            height = lines.GetLength(0);
            width = lines.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = lines[i][j];
                }
            }
        }

        public char[,] GetTileMap()
        {
            return map;
        }
    }
}


