
class MapCreationException : Exception
{
    public MapCreationException(string message) : base(message)
    {
    }
}
struct Vector2D
{
    public int x, y;
}
class Entity
{
    protected Vector2D cords;
    protected char Symbol;

    public char GetSymbol()
    {
        return Symbol;
    }
    public void SetCords(Vector2D cords)
    {
        this.cords = cords;
    }
    public Vector2D GetCords()
    {
        return cords;
    }
    public void MoveUp()
    {
        cords.x -= 1;
    }
    public void MoveDown()
    {
        cords.x += 1;
    }
    public void MoveLeft()
    {
        cords.y -= 1;
    }
    public void MoveRight()
    {
        cords.y += 1;
    }
}

class Player : Entity
{
    int Health;      //
    int ATK_Speed;   // для системи файтів Андрія!!!!
    int ATK_Damage;  //
    int DEF_Speed;   //
    int DEF_Shield;  //

    public Player()
    {

        string file = "player.txt";

        if (!File.Exists(file))
        {
            File.WriteAllText(file,
                "Symbol=A\n" +
                "Health=20\n" +
                "ATK_Speed=3\n" +
                "ATK_Damage=5\n" +
                "DEF_Speed=2\n" +
                "DEF_Shield=0");
        }

        string[] lines = File.ReadAllLines(file);

        foreach (string line in lines)
        {
            string[] parts = line.Split('=', 2);

            switch (parts[0])
            {
                case "Symbol":
                    Symbol = char.Parse(parts[1]);
                    break;

                case "Health":
                    Health = int.Parse(parts[1]);
                    break;

                case "ATK_Speed":
                    ATK_Speed = int.Parse(parts[1]);
                    break;

                case "ATK_Damage":
                    ATK_Damage = int.Parse(parts[1]);
                    break;

                case "DEF_Speed":
                    DEF_Speed = int.Parse(parts[1]);
                    break;

                case "DEF_Shield":
                    DEF_Shield = int.Parse(parts[1]);
                    break;
            }
        }
    }
}

class Map
{
    private string[,] map;
    private int size;
    private Vector2D spawn;
    private List<Entity> entities;


    public Map(int size)
    {
        this.size = size;

        spawn.x = size / 2;
        spawn.y = size / 2;

        map = new string[size, size];

        entities = new List<Entity>();
    }
    public Map(int size, int spawnX, int spawnY)
    {
        this.size = size;
        this.spawn.x = spawnX;
        this.spawn.y = spawnY;

        map = new string[size, size];
        entities = new List<Entity>();
    }

    public Vector2D GetSpawn()
    {
        return spawn;
    }
    public void CreateEmptyMap()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i == 0 || i == size - 1 || j == 0 || j == size - 1)
                    map[i, j] = "#";
                else
                    map[i, j] = " ";
            }
        }

        if (map[spawn.x, spawn.y] != " ")
        {
            throw new MapCreationException(
                "Spawnpoint obstructed"
            );
        }

    }

    public void LoadFromLines(string[] lines)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = lines[i][j].ToString();
            }
        }

        if (map[spawn.x, spawn.y] != " ")
        {
            throw new MapCreationException(
                "Spawnpoint obstructed"
            );
        }

    }

    public string[,] GetMap()
    {
        return map;
    }
    public void AddEntity(Entity entity)
    {
        entities.Add(entity);
    }
    public void Show()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                bool entityFound = false;

                foreach (Entity entity in entities)
                {
                    Vector2D entityCords = entity.GetCords();

                    if (i == entityCords.x && j == entityCords.y)
                    {
                        Console.Write(entity.GetSymbol() + " ");
                        entityFound = true;
                        break;
                    }
                }

                if (!entityFound)
                    Console.Write(map[i, j] + " ");
            }

            Console.WriteLine();
        }
    }
}

class MapFile
{
    private string filePath;
    private int size;

    public MapFile(string filePath, int size)
    {
        this.filePath = filePath;
        this.size = size;
    }

    public void LoadMap(Map map)
    {
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
                map.LoadFromLines(lines);
            }
            else
            {
                CreateNewMap(map);
            }
        }
        else
        {
            CreateNewMap(map);
        }
    }

    private void CreateNewMap(Map map)
    {
        map.CreateEmptyMap();

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            string[,] mapArray = map.GetMap();

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    writer.Write(mapArray[i, j]);
                }

                writer.WriteLine();
            }
        }
    }
}
    