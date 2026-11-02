using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Po_Griby.MapSystem;

namespace Po_Griby.Gameplay
{
    public interface IFightable
    {
        int Health { get; }
        int AttackSpeed { get; }
        int AttackDamage { get; }
        int DefenseSpeed { get; }
        int DefenseShield { get; }

        void TakeDamage(int damage);
    }

    public abstract class Entity
    {
        public Vector2D Position { get; protected set; }
        public char Symbol { get; protected set; }

        protected Entity(char symbol, Vector2D position)
        {
            Symbol = symbol;
            Position = position;
        }

        public virtual string? Interact(Player player)
        {
            return null;
        }
    }

    public abstract class Character : Entity
    {
        protected Character(char symbol, Vector2D position) : base(symbol, position) { }

        public bool TryMove(FinalMap map, Vector2D position)
        {
            bool isNPC = this is not Player;

            if (!map.IsTileWalkable(position, isNPC))
                return false;

            Entity? entity = map.GetEntityAt(position);

            if (entity != null)
            {
                if (this is Player player)
                    map.SetMessage(entity.Interact(player));

                return false;
            }

            Position = position;
            return true;
        }

        public bool MoveUp(FinalMap map)
        {
            return TryMove(map, Position + new Vector2D(0, -1));
        }

        public bool MoveDown(FinalMap map)
        {
            return TryMove(map, Position + new Vector2D(0, 1));
        }

        public bool MoveLeft(FinalMap map)
        {
            return TryMove(map, Position + new Vector2D(-1, 0));
        }

        public bool MoveRight(FinalMap map)
        {
            return TryMove(map, Position + new Vector2D(1, 0));
        }

        public override string? Interact(Player target)
        {
            return null;
        }
    }

    public class Player : Character, IFightable
    {
        public int Health { get; private set; }
        public int AttackSpeed { get; private set; }
        public int AttackDamage { get; private set; }
        public int DefenseSpeed { get; private set; }
        public int DefenseShield { get; private set; }

        public MapChanger? PendingMapChange { get; set; }
        
        public Enemy? PendingBattle { get; set; }
        public List<MushroomItem> Inventory { get; } = new()
        {
            new MushroomItem("Білий гриб", "Відновлює 10 HP", 10),
            new MushroomItem("Мухомор", "Дивний гриб...", 5)
        };
        public Player(char symbol, Vector2D position) : base(symbol, position)
        {
            string file = "player.txt";

            if (!File.Exists(file))
            {
                File.WriteAllText(file,
                    "Health=20\n" +
                    "AttackSpeed=3\n" +
                    "AttackDamage=5\n" +
                    "DefenseSpeed=2\n" +
                    "DefenseShield=0");
            }

            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                string[] parts = line.Split('=', 2);

                if (parts.Length != 2)
                    continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "Health":
                        Health = int.Parse(value);
                        break;

                    case "AttackSpeed":
                        AttackSpeed = int.Parse(value);
                        break;

                    case "AttackDamage":
                        AttackDamage = int.Parse(value);
                        break;

                    case "DefenseSpeed":
                        DefenseSpeed = int.Parse(value);
                        break;

                    case "DefenseShield":
                        DefenseShield = int.Parse(value);
                        break;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void Heal(int amount)
        {
            Health += amount;
        }

        public void CopyStatsFrom(Player other)
        {
            Health = other.Health;
            AttackSpeed = other.AttackSpeed;
            AttackDamage = other.AttackDamage;
            DefenseSpeed = other.DefenseSpeed;
            DefenseShield = other.DefenseShield;
        }
    }

    class BasicNPC : Character
    {
        private readonly string Dialogue;

        public BasicNPC(char symbol, Vector2D position, string dialogue) : base(symbol, position)
        {
            Dialogue = dialogue;
        }

        public override string? Interact(Player player)
        {
            return Dialogue;
        }
    }

    public class MapChanger : Entity
    {
        public string TileMapFile { get; }
        public string EntityMapFile { get; }

        public MapChanger(char symbol, Vector2D position, string? target) : base(symbol, position)
        {
            if (string.IsNullOrWhiteSpace(target))
                throw new FormatException($"Dlya zminini karti '{symbol}' ne vkazano kartu.");

            TileMapFile = target.Trim();
            EntityMapFile = "entities-" + TileMapFile;
        }

        public override string? Interact(Player player)
        {
            player.PendingMapChange = this;
            return "perehid na kartu " + TileMapFile;
        }
    }

    public abstract class Enemy : Character, IFightable
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int AttackSpeed { get; protected set; }
        public int AttackDamage { get; protected set; }
        public int DefenseSpeed { get; protected set; }
        public int DefenseShield { get; protected set; }

        protected Enemy(string name, char symbol, Vector2D position, int health, int damage, int attackSpeed = 2, int defenseSpeed = 1, int defenseShield = 0)
            : base(symbol, position)
        {
            Name = name;
            Health = health;
            AttackDamage = damage;
            AttackSpeed = attackSpeed;
            DefenseSpeed = defenseSpeed;
            DefenseShield = defenseShield;
        }

        public override string? Interact(Player player)
        {
            player.PendingBattle = this;
            return $"Увага! Зустріч із ворогом: {Name}!";
        }
        
        public virtual void TakeDamage(int damage)
        {
            int actualDamage = damage;

            if (DefenseShield > 0)
            {
                if (DefenseShield >= actualDamage)
                {
                    DefenseShield -= actualDamage;
                    Console.WriteLine($"Щит ворога {Name} поглинув увесь урон ({actualDamage})!");
                    return;
                }
                else
                {
                    actualDamage -= DefenseShield;
                    DefenseShield = 0;
                }
            }

            Health = Math.Max(0, Health - actualDamage);
        }

        public abstract void PerformTurn(Player player);
    }

    public class FalseBoletus : Enemy
    {
        public FalseBoletus(Vector2D position) 
            : base("Помилковий Білий", 'm', position, health: 18, damage: 3, attackSpeed: 1, defenseSpeed: 1, defenseShield: 2) { }

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
            : base("Гриб-Пилковик", 'P', position, health: 15, damage: 0, attackSpeed: 1, defenseSpeed: 1, defenseShield: 0)
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
            : base("Дикий Кабан", 'B', position, health: 35, damage: 5, attackSpeed: 3, defenseSpeed: 1, defenseShield: 5) { }

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
            : base($"Зграя вовків ({wolfCount})", 'W', position, health: 15 * wolfCount, damage: 4, attackSpeed: 2, defenseSpeed: 1, defenseShield: 0)
        {
            WolfCount = Math.Clamp(wolfCount, 2, 3);
            Name = $"Зграя вовків - ({WolfCount}) вовки";
        }

        public override void PerformTurn(Player player)
        {
            int currentWolves = (int)Math.Ceiling((double)Health / 15);
            if (currentWolves < WolfCount) WolfCount = currentWolves;

            Console.WriteLine($"\n{Name} оточує вас!");
            for (int i = 0; i < WolfCount; i++)
            {
                Console.WriteLine($"-> Вовк #{i + 1} кусає вас за бочок, наносячи {AttackDamage} урона");
                player.TakeDamage(AttackDamage);
            }
        }
    }

    public class BearMiniBoss : Enemy
    {
        public BearMiniBoss(Vector2D position) 
            : base("Розлючений Мішутка (Міні-бос)", 'M', position, health: 60, damage: 8, attackSpeed: 1, defenseSpeed: 1, defenseShield: 10) { }

        public override void PerformTurn(Player player)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n{Name} оглушливо ричить і завдає потужного удару лапою!");
            Console.ResetColor();
            player.TakeDamage(AttackDamage);
            Console.WriteLine($"Ви отримали {AttackDamage} важкої шкоди!");
        }
    }

    public class MushroomItem
    {
        public string Name { get; }
        public string Description { get; }
        private int healAmount;

        public MushroomItem(string name, string description, int healAmount)
        {
            Name = name;
            Description = description;
            this.healAmount = healAmount;
        }

        public void Use(Player player)
        {
            player.Heal(healAmount);
            Console.WriteLine($"\nvi zyili {Name} i vidnovili {healAmount} hp");
        }
    }
}
