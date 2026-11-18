using Po_Griby.Gameplay;
using System;
using System.Collections.Generic;

namespace Po_Griby.UserInterface
{
    internal class InterfaceMenu
    {
        private int lastInventoryLineCount;

        public void Show_T(Player player)
        {
            Console.SetCursorPosition(70, 20);
            Console.WriteLine("====== 📜 ЗАВДАННЯ 📜 ======");

            for (int i = 21; i <= 29; i++)
            {
                Console.SetCursorPosition(70, i);
                Console.Write(new string(' ', Console.WindowWidth - 72));
            }
            

            Console.SetCursorPosition(70, 21);

            if (player.CurrentQuest == null)
            {
                Console.WriteLine("Немає активних завдань.");
            }
            else
            {
                Console.WriteLine(player.CurrentQuest.Description);

                Console.SetCursorPosition(70, 22);

                int progress = player.CurrentQuest.GetProgress(player.PlayerInventory);

                Console.WriteLine(
                    "Прогрес: " + progress + "/" +
                    player.CurrentQuest.RequiredAmount);
            }
        }

        private List<string> messages = new List<string>();


        public void AddMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            messages.Add(message);

            if (messages.Count > 6)
                messages.RemoveAt(0);
        }

        public void Show_D()
        {
            int x = 70;
            int startY = 14;
            int maxLines = 8;
            

            Console.SetCursorPosition(70, 13);
            Console.WriteLine("====== 👤 ДІАЛОГИ 👤 ======");
            int width = Console.WindowWidth - 71;

            for (int i = 14; i < 22; i++)
            {
                Console.SetCursorPosition(70, i);
                Console.Write(new string(' ', width));
            }
            List<string> lines = new List<string>();
            

            foreach (string message in messages)
            {
                string[] paragraphs = message.Split('\n');

                foreach (string paragraph in paragraphs)
                {
                    string[] words = paragraph.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);

                    string line = "";

                    foreach (string word in words)
                    {
                        if (line.Length + word.Length + 1 > width)
                        {
                            if (line.Length > 0)
                            {
                                lines.Add(line);
                                line = "";
                            }
                        }

                        if (line.Length > 0)
                            line += " ";

                        line += word;
                    }

                    if (line.Length > 0)
                        lines.Add(line);
                }
            }
            int firstLine = Math.Max(0, lines.Count - maxLines);
            int y = startY;

            for (int i = firstLine; i < lines.Count; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(lines[i]);
                y++;
            }
        }

        public void Show_I(Player player)
        {
            const int x = 70;
            int width = Console.WindowWidth - x;
            if (width <= 0 || Console.WindowHeight < 2)
                return;

            Inventory inventory = player.PlayerInventory;
            int visibleItemCount = Math.Min(inventory.Items.Count, Console.WindowHeight - 1);
            int rowsToClear = Math.Min(
                Console.WindowHeight,
                Math.Max(lastInventoryLineCount, visibleItemCount) + 1);

            for (int row = 0; row < rowsToClear; row++)
            {
                Console.SetCursorPosition(x, row);
                Console.Write(new string(' ', width));
            }

            Console.SetCursorPosition(x, 0);
            Console.Write(FitPanelLine("====== 🎒 ІНВЕНТАР 🎒 ======", width));

            if (inventory.Items.Count == 0)
            {
                Console.SetCursorPosition(x, 1);
                Console.Write(FitPanelLine("Інвентар порожній.", width));
                lastInventoryLineCount = 1;
                return;
            }

            for (int i = 0; i < visibleItemCount; i++)
            {
                Console.SetCursorPosition(x, i + 1);
                string line = $"{i + 1}. {inventory.Items[i].Name} - {inventory.Items[i].Description}";
                Console.Write(FitPanelLine(line, width));
            }

            lastInventoryLineCount = visibleItemCount;
        }

        public void OpenInventory(Player player)
        {
            Inventory inventory = player.PlayerInventory;
            int selectedIndex = 0;
            bool isOpen = true;

            while (isOpen)
            {
                Console.Clear();

                Console.WriteLine("====== 🎒 ІНВЕНТАР 🎒 ======\n");

                if (inventory.IsEmpty())
                {
                    Console.WriteLine("Інвентар порожній!");
                    Console.WriteLine("\nНатисніть Esc, щоб повернутися.");
                }
                else
                {
                    for (int i = 0; i < inventory.Items.Count; i++)
                    {
                        if (i == selectedIndex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("--> ");
                        }
                        else
                        {
                            Console.Write("    ");
                        }

                        Console.WriteLine(
                            $"{inventory.Items[i].Name} - {inventory.Items[i].Description}");

                        Console.ResetColor();
                    }

                    Console.WriteLine("\n↑ ↓ — переміщення");
                    Console.WriteLine("Enter — використати гриб");
                    Console.WriteLine("Esc — назад");
                }

                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (!inventory.IsEmpty())
                        {
                            selectedIndex--;

                            if (selectedIndex < 0)
                                selectedIndex = inventory.Items.Count - 1;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (!inventory.IsEmpty())
                        {
                            selectedIndex++;

                            if (selectedIndex >= inventory.Items.Count)
                                selectedIndex = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (!inventory.IsEmpty())
                        {
                            MushroomItem item = inventory.Items[selectedIndex];
                            Console.Clear();
                            item.Use(player);

                            if (player.CurrentQuest != null)
                            {
                                Console.WriteLine("Прогрес квесту: " + player.CurrentQuest.GetProgress(player.PlayerInventory));
                            }
                            else
                            {
                                Console.WriteLine("Немає активного квесту");
                            }

                            inventory.RemoveItem(selectedIndex);

                            if (selectedIndex >= inventory.Items.Count)
                                selectedIndex = inventory.Items.Count - 1;

                            Console.WriteLine("\nНатисніть будь-яку клавішу...");
                            Console.ReadKey(true);
                        }
                        break;

                    case ConsoleKey.Escape:
                        isOpen = false;
                        break;
                }
            }
        }

        private static string FitPanelLine(string text, int width)
        {
            if (text.Length > width)
                return width == 1 ? "…" : text[..(width - 1)] + "…";

            return text.PadRight(width);
        }
    }
}
