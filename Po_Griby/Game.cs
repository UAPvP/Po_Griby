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
            const long tickIntervalMs = 200;
            var gameClock = System.Diagnostics.Stopwatch.StartNew();
            long nextTickAt = tickIntervalMs;
            bool shouldRender = true;

            while (isRunning)
            {
                bool playerInputReceived = false;
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    playerInputReceived = true;

                    switch (key)
                    {
                        case ConsoleKey.W: player.MoveUp(map, logger); break;
                        case ConsoleKey.A: player.MoveLeft(map, logger); break;
                        case ConsoleKey.S: player.MoveDown(map, logger); break;
                        case ConsoleKey.D: player.MoveRight(map, logger); break;
                        case ConsoleKey.F5:
                            PlayerStorage.Save("player.json", player);
                            map.SetMessage("igra sohranena");
                            logger.Log(LogLevel.Info, "igra sohranena");
                            shouldRender = true;
                            break;
                        case ConsoleKey.Q:
                            logger.Log(LogLevel.Info, "Гравець вийшов з гри.");
                            isRunning = false;
                            break;
                    }

                }

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
                    shouldRender = true;
                }

                bool tickElapsed = gameClock.ElapsedMilliseconds >= nextTickAt;
                if (tickElapsed)
                {
                    MovingCharacter[] movingEntities = map.GetEntityMap().Entities
                        .OfType<MovingCharacter>()
                        .ToArray();
                    logger.Log(LogLevel.Debug, $"Тік світу: оновлення сутностей ({movingEntities.Length}).");

                    foreach (MovingCharacter entity in movingEntities)
                    {
                        entity.Move(map, player, logger);
                    }

                    // Один тик за цикл: якщо обработка задержалась (например, во время боя),
                    // не пытаемся проиграть накопившиеся тики подряд.
                    nextTickAt = gameClock.ElapsedMilliseconds + tickIntervalMs;

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

                if (playerInputReceived || tickElapsed)
                    PlayerStorage.Save("player.json", player);

                Thread.Sleep(10);
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
