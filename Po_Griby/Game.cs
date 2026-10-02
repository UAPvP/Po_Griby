class Program
{
    static void Main()
    {
        int size = 28;
        string filePath = "map.txt";
        Player player = new Player();


        Map map = new Map(size, 1, 1);

        MapFile mapFile = new MapFile(filePath, size);

        mapFile.LoadMap(map);
        player.SetCords(map.GetSpawn());
        map.AddEntity(player);
        map.Show();

        while (true)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case (ConsoleKey.UpArrow):
                    player.MoveUp();
                    break;
                case (ConsoleKey.DownArrow):
                    player.MoveDown();
                    break;
                case (ConsoleKey.LeftArrow):
                    player.MoveLeft();
                    break;
                case (ConsoleKey.RightArrow):
                    player.MoveRight();
                    break;
            }
            Console.Clear();
            map.Show();
        }
    }
}