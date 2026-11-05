using System;
using Po_Griby.Gameplay;
using Po_Griby.MapSystem;
using Po_Griby;

class Program
{
    static void Main()
    {
        try
        {
            FinalMap map = new FinalMap(
                "map.txt",
                "entities-map.txt",
                "tiles.json",
                "entities.json");

            Player? player = FindPlayer(map);

            if (player == null)
            {
                Console.WriteLine("На мапі немає гравця. Додайте його символ до entities-map.txt.");
                return;
            }

            bool isRunning = true;
            Console.Clear();

            while (isRunning)
            {
                Console.SetCursorPosition(0, 0);
                map.Show();
                Console.WriteLine();
                Console.WriteLine("Керування: W — вгору, A — ліворуч, S — вниз, D — праворуч, Q — вихід.");

                int messageWidth = Math.Max(1, Console.WindowWidth - 1);
                string message = map.LastMessage;

                if (message.Length > messageWidth)
                    message = message.Substring(0, messageWidth);

                Console.Write(message.PadRight(messageWidth));

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
                        PlayerStorage.Save("player.json", player);
                        isRunning = false;
                        break;
                }

                if (player.PendingMapChange != null)
                {
                    MapChanger changer = player.PendingMapChange;
                    player.PendingMapChange = null;

                    if (ChangeMap(changer, ref map, ref player))
                        Console.Clear();
                }
                if (player.PendingBattle != null)
                 {
                     Enemy enemyToFight = player.PendingBattle;
                     player.PendingBattle = null;
                
                     bool playerWon = FightSystem.StartBattle(player, enemyToFight, player.Inventory);
                     Console.Clear();
                
                     if (playerWon)
                     {
                         map.GetEntityMap().Remove(enemyToFight);
                     }
                     else
                     {
                         PlayerStorage.Save("player.json", player);
                         break;
                     }
                 }
                PlayerStorage.Save("player.json", player);
            }
        }
        catch (Exception error)
        {
            Console.ResetColor();
            Console.WriteLine("Не вдалося завантажити гру:");
            Console.WriteLine(error.Message);
        }
    }

    static Player? FindPlayer(FinalMap map)
    {
        foreach (Entity entity in map.GetEntityMap().Entities)
        {
            if (entity is Player found)
                return found;
        }

        return null;
    }

    static bool ChangeMap(MapChanger changer, ref FinalMap map, ref Player player)
    {
        if (!File.Exists(changer.TileMapFile))
        {
            map.SetMessage("Мапу " + changer.TileMapFile + " не знайдено.");
            return false;
        }

        try
        {
            FinalMap newMap = new FinalMap(
                changer.TileMapFile,
                changer.EntityMapFile,
                "tiles.json",
                "entities.json");

            Player? newPlayer = FindPlayer(newMap);

            if (newPlayer == null)
            {
                map.SetMessage("На мапі немає гравця.");
                return false;
            }

            newPlayer.CopyStatsFrom(player);
            map = newMap;
            player = newPlayer;
            return true;
        }
        catch (Exception error)
        {
            map.SetMessage("Не вдалося завантажити мапу: " + error.Message);
            return false;
        }
    }
}















