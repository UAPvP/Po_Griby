using System;
using System.Collections.Generic;
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
        public MapChanger(char symbol, Vector2D position) : base(symbol, position)
        {
            throw new NotImplementedException();
        }
    }
}

