using Po_Griby.Gameplay;
using System;
using System.Collections.Generic;

namespace Po_Griby.UserInterface
{
    internal class InterfaceMenu
    {
        private Inventory inventory;

        public InterfaceMenu(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public IReadOnlyList<MushroomItem> Items => inventory.Items;

        public void Show_T()
        {
            Console.SetCursorPosition(70, 23);
            Console.WriteLine("====== 📖 КВЕСТИ 📖 ======");
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
            Console.SetCursorPosition(70, 13);
            Console.WriteLine("====== 👤 ДІАЛОГИ 👤 ======");
            int width = Console.WindowWidth - 71;

            for (int i = 14; i < 22; i++)
            {
                Console.SetCursorPosition(70, i);
                Console.Write(new string(' ', width));
            }

            int y = 14;

            foreach (string message in messages)
            {
                Console.SetCursorPosition(70, y);
                Console.Write(message);
                y++;

                if (y >= 22)
                    break;
            }
        }

        public void Show_I()
        {
            int x = 70;
            int y = 0;

            Console.SetCursorPosition(x, y);
            Console.WriteLine("====== 🎒 ІНВЕНТАР 🎒 ======");

            y++;

            if (inventory.Items.Count == 0)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine("Інвентар порожній.");
                return;
            }

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine($"{i + 1}. {inventory.Items[i].Name} - {inventory.Items[i].Description}");
                y++;
            }
        }

        public void OpenInventory(Player player)
        {
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
    }
}
