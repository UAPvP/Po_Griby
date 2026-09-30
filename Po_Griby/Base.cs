int size = 28;
string filePath = "map.txt";

string[,] map = new string[size, size];

if (File.Exists(filePath))
{
    string[] lines = File.ReadAllLines(filePath);

    bool correctSize = lines.Length == size;

    if (correctSize)
    {
        for (int i = 0; i < size; i++)
        {
            if (lines[i].Length != size)
            {
                correctSize = false;
                break;
            }
        }
    }

    if (correctSize)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = lines[i][j].ToString();
            }
        }
    }
    else
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == 0 || i == size - 1 || j == 0 || j == size - 1)
                    map[i, j] = "X";
                else
                    map[i, j] = " ";
            }
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    writer.Write(map[i, j]);
                }

                writer.WriteLine();
            }
        }
    }
}
else
{
    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < size; j++)
        {
            if (i == 0 || i == size - 1 || j == 0 || j == size - 1)
                map[i, j] = "X";
            else
                map[i, j] = " ";
        }
    }

    using (StreamWriter writer = new StreamWriter(filePath))
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                writer.Write(map[i, j]);
            }

            writer.WriteLine();
        }
    }
}
for (int i = 0; i < map.GetLength(0); i++)
{
    for (int j = 0; j < map.GetLength(1); j++)
    {
        Console.Write(map[i, j] + " ");
    }

    Console.WriteLine();
}