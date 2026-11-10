using System;
using System.IO;
using System.Linq;
using Po_Griby;
using Po_Griby.Gameplay;
using Po_Griby.Interface;
using Po_Griby.Logging;
using Po_Griby.MapSystem;
using Po_Griby.UserInterface;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        string logPath = Path.Combine(Environment.CurrentDirectory, "lastsession.log");
        File.WriteAllText(logPath, string.Empty);
        var logger = new Logger(new FileLogProvider(logPath));
        logger.Log(LogLevel.Info, "Ігрову сесію розпочато.");

        try
        {
            Menu menu = new Menu("======🍄PO GRIBY🍄======");
            MenuSection mainSection = new MenuSection("🌲-ГОЛОВНЕ МЕНЮ-🌲");

            mainSection.AddOption("🎮 ГРАТИ", "Почати гру", () =>
            {
                game_plot plot = new game_plot();
                plot.plot();
                menu.Close();
            });
            mainSection.AddOption("📖 ПРАВИЛА", "Керування та правила", () =>
            {
                Console.Clear();
                Console.WriteLine("Керування: W — вгору, A — ліворуч, S — вниз, D — праворуч, Q — вихід.\n" +
                                  "Зіткніться з ворогом, щоб почати бій. У бою можна атакувати, використати гриб або втекти.");
                Console.WriteLine("\nНатисніть будь-яку клавішу, щоб повернутися до меню...");
                Console.ReadKey(true);
            });
            mainSection.AddOption("🚪 ВИЙТИ", "Завершити гру", () =>
            {
                Console.Clear();
                Console.WriteLine("До зустрічі!");
                Environment.Exit(0);
            });

            menu.AddSection(mainSection);
            menu.Run();

            FinalMap map = new FinalMap(
                "map.txt",
                "entities-map.txt",
                "tiles.json",
                "entities.json");
            logger.Log(LogLevel.Info, "Мапу гри завантажено.");

            Player? player = FindPlayer(map);
            if (player == null)
            {
                logger.Log(LogLevel.Warning, "На мапі не знайдено гравця.");
                Console.WriteLine("На мапі немає гравця. Додайте його символ до entities-map.txt.");
                return;
            }

            logger.Log(LogLevel.Info, "Гравця знайдено, гра починається.");
            bool isRunning = true;
            Console.Clear();
            bool shouldRender = true;

            while (isRunning)
            {
                if (shouldRender)
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
                    shouldRender = false;
                }

                ConsoleKey key = Console.ReadKey(true).Key;
                bool playerCommand = key is ConsoleKey.W or ConsoleKey.A or ConsoleKey.S or ConsoleKey.D;
                bool playerMoved = false;
                switch (key)
                {
                    case ConsoleKey.W: playerMoved = player.MoveUp(map, logger); break;
                    case ConsoleKey.A: playerMoved = player.MoveLeft(map, logger); break;
                    case ConsoleKey.S: playerMoved = player.MoveDown(map, logger); break;
                    case ConsoleKey.D: playerMoved = player.MoveRight(map, logger); break;
                    case ConsoleKey.Q:
                        logger.Log(LogLevel.Info, "Гравець вийшов з гри.");
                        isRunning = false;
                        break;
                }

                if (playerCommand)
                    shouldRender = true;

                if (!isRunning)
                {
                    PlayerStorage.Save("player.json", player);
                    break;
                }

                if (player.PendingPickup != null)
                {
                    DroppedItem item = player.PendingPickup;
                    player.PlayerInventory.AddItem(item.ToInventoryItem());
                    map.GetEntityMap().Remove(item);
                    player.PendingPickup = null;
                    logger.Log(LogLevel.Info,
                        $"Гравець підібрав предмет {item.Name} ({item.Symbol}) у ({item.Position.x}, {item.Position.y}).");
                    shouldRender = true;
                    PlayerStorage.Save("player.json", player);
                }

                if (player.PendingMapChange != null)
                {
                    MapChanger changer = player.PendingMapChange;
                    player.PendingMapChange = null;
                    if (ChangeMap(changer, ref map, ref player))
                    {
                        logger.Log(LogLevel.Info, $"Гравець перейшов на мапу {changer.TileMapFile}.");
                        Console.Clear();
                        shouldRender = true;
                    }
                }

                if (playerMoved)
                {
                    MovingCharacter[] movingEntities = map.GetEntityMap().Entities
                        .OfType<MovingCharacter>()
                        .ToArray();
                    logger.Log(LogLevel.Debug, $"Хід гравця: оновлення сутностей ({movingEntities.Length}).");

                    foreach (MovingCharacter entity in movingEntities)
                        entity.Move(map, player, logger);

                    shouldRender = true;
                }

                if (player.PendingBattle != null)
                {
                    Enemy enemyToFight = player.PendingBattle;
                    player.PendingBattle = null;
                    bool playerWon = FightSystem.StartBattle(player, enemyToFight, player.PlayerInventory);
                    logger.Log(playerWon ? LogLevel.Info : LogLevel.Warning,
                        playerWon ? "Гравець переміг у бою." : "Гравець програв бій.");
                    Console.Clear();

                    if (playerWon)
                        map.GetEntityMap().Remove(enemyToFight);
                    else
                    {
                        PlayerStorage.Save("player.json", player);
                        break;
                    }
                    PlayerStorage.Save("player.json", player);
                    shouldRender = true;
                }

                if (shouldRender)
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
                    shouldRender = false;
                }

            }
        }
        catch (Exception error)
        {
            logger.Log(LogLevel.Critical, "Не вдалося продовжити гру.", error);
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
