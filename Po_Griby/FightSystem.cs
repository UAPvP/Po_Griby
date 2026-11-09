using System;
using System.Collections.Generic;
using Po_Griby.Gameplay;

namespace Po_Griby
{
    public class FightSystem
    {
        public static bool StartBattle(Player player, Enemy enemy, Inventory inventory)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("==================================================");
            Console.WriteLine($"           Розпочався бій з {enemy.Name.ToUpper()}");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            while (player.Health > 0 && enemy.Health > 0)
            {
                Console.WriteLine("\n--------------------------------------------------");
                Console.WriteLine($" Ви: HP [{player.Health}] | DMG [{player.AttackDamage}]");
                Console.WriteLine($" Ворог ({enemy.Name}): HP [{enemy.Health}]");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Ваш хід! Оберіть дію:");
                Console.WriteLine("1. Атакувати");
                Console.WriteLine("2. Використати предмет з інвентаря");
                Console.WriteLine("3. Втекти з бою");
                Console.Write("> ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                if (choice == "1")
                {
                    Console.WriteLine($"\nВи атакуєте {enemy.Name}!");
                    enemy.TakeDamage(player.AttackDamage);
                    Console.WriteLine($"Ви завдали {player.AttackDamage} одиниць шкоди.");
                }
                else if (choice == "2")
                {
                    if (inventory.Items.Count == 0)
                    {
                        Console.WriteLine("\nУ вашому інвентарі немає предметів! Ви пропускаєте хід.");
                    }
                    else
                    {
                        Console.WriteLine("\nОберіть предмет:");
                        for (int i = 0; i < inventory.Items.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {inventory.Items[i].Name} ({inventory.Items[i].Description})");
                        }
                        Console.Write("> ");
                        if (int.TryParse(Console.ReadLine(), out int itemIdx) && itemIdx > 0 && itemIdx <= inventory.Items.Count)
                        {
                            var item = inventory.Items[itemIdx - 1];
                            item.Use(player);
                            inventory.RemoveItem(itemIdx - 1);
                        }
                        else
                        {
                            Console.WriteLine("Неправильний вибір. Ви пропускаєте хід!");
                        }
                    }
                }
                else if (choice == "3")
                {
                    Console.WriteLine("\nВи успішно втекли з бою!");
                    return false;
                }
                else
                {
                    Console.WriteLine("\nВи загаялися і пропустили хід!");
                }

                if (enemy.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nПеремога! Ви здолали ворога: {enemy.Name}!");
                    Console.ResetColor();
                    Console.WriteLine("Натисніть будь-яку клавішу...");
                    Console.ReadKey(true);
                    return true;
                }

                enemy.PerformTurn(player);

                if (player.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nВи загинули в бою... ГРУ ЗАВЕРШЕНО.");
                    Console.ResetColor();
                    Console.WriteLine("Натисніть будь-яку клавішу...");
                    Console.ReadKey(true);
                    return false;
                }
            }

            return player.Health > 0;
        }
    }
}

