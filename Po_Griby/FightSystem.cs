using System;
using System.Collections.Generic;
using Po_Griby.Gameplay;

namespace Po_Griby
{
    public class FightSystem
    {
        public static bool StartBattle(Player player, Enemy enemy, List<MushroomItem> inventory)
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
                Console.WriteLine("Твій хід! Оберіть дію:");
                Console.WriteLine("1. Атакувати");
                Console.WriteLine("2. Використати предмет з інвентаря");
                Console.WriteLine("3. Втекти з бою");
                Console.Write("> ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                if (choice == "1")
                {
                    Console.WriteLine($"\nВи завдаєте шкоди {enemy.Name}!");
                    enemy.TakeDamage(player.AttackDamage);
                    Console.WriteLine($" {player.AttackDamage} Наносить удар!");
                }
                else if (choice == "2")
                {
                    if (inventory.Count == 0)
                    {
                        Console.WriteLine("\nУ вас немає предметів в інвентарі! Хід пропущено.");
                    }
                    else
                    {
                        Console.WriteLine("\nОберіть предмет");
                        for (int i = 0; i < inventory.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {inventory[i].Name} ({inventory[i].Description})");
                        }
                        Console.Write("> ");
                        if (int.TryParse(Console.ReadLine(), out int itemIdx) && itemIdx > 0 && itemIdx <= inventory.Count)
                        {
                            var item = inventory[itemIdx - 1];
                            item.Use(player);
                            inventory.RemoveAt(itemIdx - 1);
                        }
                        else
                        {
                            Console.WriteLine("Невірний вибір, хід змарновано!");
                        }
                    }
                }
                else if (choice == "3")
                {
                    Console.WriteLine("\n Ви успішно покинули місце битви!");
                    return false;
                }
                else
                {
                    Console.WriteLine("\nВи замріялися і пропустили хід!");
                }

                if (enemy.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nПеремога! {enemy.Name} був повержений!");
                    Console.ResetColor();
                    Console.WriteLine("Натисніть будь-яку клавішу...");
                    Console.ReadKey(true);
                    return true;
                }

                enemy.PerformTurn(player);

                if (player.Health <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nВи загинули в бою... GAME OVER(⁠*⁠_⁠*⁠)");
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

