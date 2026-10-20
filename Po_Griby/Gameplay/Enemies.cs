using System;
using Po_Griby.MapSystem;

namespace Po_Griby.Gameplay
{
    public abstract class Enemy : Character, IFightable
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int AttackSpeed { get; protected set; }
        public int AttackDamage { get; protected set; }
        public int DefenseSpeed { get; protected set; }
        public int DefenseShield { get; protected set; }

        protected Enemy(string name, char symbol, Vector2D position, int health, int damage) 
            : base(symbol, position)
        {
            Name = name;
            Health = health;
            AttackDamage = damage;
            AttackSpeed = 2;
            DefenseSpeed = 1;
            DefenseShield = 0;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public abstract void PerformTurn(Player player);
    }

    public class FalseBoletus : Enemy
    {
        public FalseBoletus(Vector2D position) 
            : base("Помилковий Білий", 'm', position, health: 18, damage: 3) { }

        public override void PerformTurn(Player player)
        {
            Console.WriteLine($"\n{Name} виділяє гіркий сік і обпікає вам шкіру!");
            player.TakeDamage(AttackDamage);
            Console.WriteLine($"Ви отримали {AttackDamage} шкоди!");
        }
    }

    public class PuffballMushroom : Enemy
    {
        private int fuseTimer;
        private readonly int explosionDamage;

        public PuffballMushroom(Vector2D position, int timerTurns = 3, int explosionDamage = 18) 
            : base("Гриб-Пилковик ", 'P', position, health: 15, damage: 0)
        {
            this.fuseTimer = timerTurns;
            this.explosionDamage = explosionDamage;
        }

        public override void PerformTurn(Player player)
        {
            fuseTimer--;

            if (fuseTimer > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n{Name} роздувається і шипить! До вибуху залишилось {fuseTimer} ходів!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n БУМ! {Name} вибухає хмарою отруйного пилу!");
                player.TakeDamage(explosionDamage);
                Console.WriteLine($"Ви отримали {explosionDamage} шкоди від вибуху!");
                Console.ResetColor();
                Health = 0;
            }
        }
    }

    public class WildBoar : Enemy
    {
        private bool isCharging = false;

        public WildBoar(Vector2D position) 
            : base("Дикий Кабан", 'B', position, health: 35, damage: 5) { }

        public override void PerformTurn(Player player)
        {
            if (!isCharging)
            {
                isCharging = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n{Name} риє землю копитом і розганяється на вас!");
                Console.ResetColor();
            }
            else
            {
                isCharging = false;
                int heavyDamage = AttackDamage * 2;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{Name} ВРІЗАЄТЬСЯ у вас на повній швидкості!");
                Console.ResetColor();
                player.TakeDamage(heavyDamage);
                Console.WriteLine($"Ви отримали {heavyDamage} шкоди від тарана!");
            }
        }
    }

    public class WolfPack : Enemy
    {
        public int WolfCount { get; private set; }

        public WolfPack(Vector2D position, int wolfCount = 2) 
            : base($"Зграя вовків ({wolfCount})", 'W', position, health: 15 * wolfCount, damage: 4)
        {
            WolfCount = Math.Clamp(wolfCount, 2, 3);
            Name = $"Зграя вовків - ({WolfCount} ) вовки";
        }

        public override void PerformTurn(Player player)
        {
            int currentWolves = (int)Math.Ceiling((double)Health / 15);
            if (currentWolves < WolfCount) WolfCount = currentWolves;

            Console.WriteLine($"\n{Name} оточує вас!");
            for (int i = 0; i < WolfCount; i++)
            {
                Console.WriteLine($"-> Вовк #{i + 1} кусає вас за бочок, наносячи {AttackDamage} урона);
                player.TakeDamage(AttackDamage);
            }
        }
    }

    public class BearMiniBoss : Enemy
    {
        public BearMiniBoss(Vector2D position) 
            : base("Розлючений Мішутка (Міні-бос)", 'M', position, health: 60, damage: 8) { }

        public override void PerformTurn(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n{Name} оглушливо ричить і завдає потужного удару лапою!");
            Console.ResetColor();
            player.TakeDamage(AttackDamage);
            Console.WriteLine($"Ви отримали {AttackDamage} важкої шкоди!");
        }
    }
}
