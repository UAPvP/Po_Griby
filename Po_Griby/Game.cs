using Po_Griby.Gameplay;
using Po_Griby.Interface;
using Po_Griby.MapSystem;
using Po_Griby.UserInterface;
using System;
using System.Reflection.Metadata.Ecma335;
using static Po_Griby.Interface.Menu;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        try
        {
            //menu
            
            Menu menu = new Menu("======🍄PO GRIBY🍄======");

            MenuSection mainSection = new MenuSection("🌲-MAIN-MENU-🌲");

            mainSection.AddOption("    🎮GAME🎮", "Почати нову гру,п", () =>
            {
                game_plot plot = new game_plot();
                plot.plot();
                menu.Close();
            });
            mainSection.AddOption("    📖RULES📖", "правила", () => { });
            mainSection.AddOption("    🚪EXIT🚪", "Вийти з гри", () => {
                Console.Clear();
                Console.WriteLine("До зустрічі!");
                Environment.Exit(0);
            });

            menu.AddSection(mainSection);

            menu.Run();

            //---------------------

            FinalMap map = new FinalMap(
                "map.txt",
                "entities-map.txt",
                "tiles.txt",
                "entity-definitions.txt");

            Player? player = FindPlayer(map);

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
            map.SetMessage("karti " + changer.TileMapFile + " nimae");
            return false;
        }

        try
        {
            FinalMap newMap = new FinalMap(
                changer.TileMapFile,
                changer.EntityMapFile,
                "tiles.txt",
                "entity-definitions.txt");

            Player? newPlayer = FindPlayer(newMap);

            if (newPlayer == null)
            {
                map.SetMessage("na karti ne ma player");
                return false;
            }

            newPlayer.CopyStatsFrom(player);
            map = newMap;
            player = newPlayer;
            return true;
        }
        catch (Exception error)
        {
            map.SetMessage("ne zanydeno karty" + error.Message);
            return false;
        }
    }
}















