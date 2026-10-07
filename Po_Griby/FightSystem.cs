class FightSystem
{
    public static bool StartFight(Player player, Enemy enemy)
    {
        Console.Clear();
        bool isPlayerTurn = player.ATK_Speed >= enemy.ATK_Speed; // Хто бистріший, той ходить першим

        while (player.IsAlive && enemy.IsAlive)
        {
            RenderBattleUI(player, enemy);

            if (isPlayerTurn)
            {
                bool fled = PlayerTurn(player, enemy);
                if (fled) return true; // Успішна евакуація)
            }
            else
            {
                EnemyTurn(player, enemy);
            }

            if (!enemy.IsAlive) break;
            if (!player.IsAlive) break;

            isPlayerTurn = !isPlayerTurn; // Перехід ходу
            Console.WriteLine("\n[Тицьніть будь-яку клавішу для наступного ходу...]");
            Console.ReadKey(true);
            Console.Clear();
        }

        Console.Clear();
        if (player.IsAlive)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== Победа! Ви здолали {enemy.Name}! ===");
            Console.ResetColor();
            Console.WriteLine("Натисніть клавішу для повернення на карту...");
            Console.ReadKey(true);
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n=== Програш! Вас здолав {enemy.Name}... ===");
            Console.ResetColor();
            return false;
        }
    }

    private static void RenderBattleUI(Player player, Enemy enemy)
    {
        Console.WriteLine("-=========================================================-");
        Console.WriteLine($" Противник: {enemy.Name,-15} HP: [{GetHealthBar(enemy.Health, enemy.MaxHealth)}] {enemy.Health}/{enemy.MaxHealth} ");
        Console.WriteLine("-=========================================================-");
        Console.WriteLine($"                    ({enemy.GetSymbol()}) - {enemy.Name,-15}    ");
        Console.WriteLine("-=========================================================-");
        Console.WriteLine($" Ви: ({player.GetSymbol()})             HP: [{GetHealthBar(player.Health, player.MaxHealth)}] {player.Health}/{player.MaxHealth} ");
        Console.WriteLine("-=========================================================-");
    }

    private static string GetHealthBar(int current, int max)
    {
        int totalBlocks = 10;
        int filled = (int)Math.Ceiling((double)current / max * totalBlocks);
        return new string('█', filled) + new string('-', totalBlocks - filled);
    }

    private static bool PlayerTurn(Player player, Enemy enemy)
    {
        Console.WriteLine("\n-== ВАШ ХІД ==-");
        Console.WriteLine("1. Атакувати (ATK)");
        Console.WriteLine("2. Захист / Щит (DEF)");
        Console.WriteLine("3. Спробувати втекти");
        Console.Write("Ваши дії (1-3): ");

        while (true)
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
            {
                int dmg = player.ATK_Damage;
                enemy.TakeDamage(dmg);
                Console.WriteLine($"\nВи нанесли {enemy.Name} {dmg} урона! (Врахувати щит ворога: {enemy.DEF_Shield})");
                return false;
            }
            else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
            {
                int healShield = player.DEF_Speed;
                player.Health = Math.Min(player.MaxHealth, player.Health + healShield);
                Console.WriteLine($"\nВи стали в супер захисне стійку та відновили {healShield} HP!");
                return false;
            }
            else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3)
            {
                Random rng = new Random();
                if (rng.Next(0, 100) < 50)
                {
                    Console.WriteLine("\nВи успішно провели евакуаційну операцію з битви!");
                    return true;
                }
                else
                {
                    Console.WriteLine("\nСпроба відхіду не вдалася!");
                    return false;
                }
            }
        }
    }

    private static void EnemyTurn(Player player, Enemy enemy)
    {
        Console.WriteLine($"\n-== Хід Противника ({enemy.Name}) ---");
        int dmg = enemy.ATK_Damage;
        player.TakeDamage(dmg);
        Console.WriteLine($"{enemy.Name} атакує вас і завдає болю!");
    }
}
