using System;
using Po_Griby.Gameplay;
using Po_Griby.MapSystem;

class Program
{
    static void Main()
    {
        try
        {
            FinalMap map = new FinalMap(
                "map.txt",
                "entities-map.txt",
                "tiles.txt",
                "entity-definitions.txt");

            Player? player = null;

            foreach (Entity entity in map.GetEntityMap().Entities)
            {
                if (entity is Player foundPlayer)
                {
                    player = foundPlayer;
                    break;
                }
            }

            if (player == null)
            {
                Console.WriteLine("На карті немає гравця. Додайте символ гравця до entities-map.txt.");
                return;
            }

            bool isRunning = true;
            Console.Clear();

            while (isRunning)
            {
                Console.SetCursorPosition(0, 0);
                map.Show();
                Console.WriteLine();
                Console.WriteLine("Керування: W — вгору, A — вліво, S — вниз, D — вправо, Q — вихід.");

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W:
                        player.MoveUp(map);
                        break;
                    case ConsoleKey.A:
                        player.MoveLeft(map);
                        break;
                    case ConsoleKey.S:
                        player.MoveDown(map);
                        break;
                    case ConsoleKey.D:
                        player.MoveRight(map);
                        break;
                    case ConsoleKey.Q:
                        isRunning = false;
                        break;
                }
            }
        }
        catch (Exception error)
        {
            Console.ResetColor();
            Console.WriteLine("Не вдалося завантажити гру:");
            Console.WriteLine(error.Message);
        }
    }
}
