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
        public virtual void Interact(Player player)
        {
        }
    }
    public abstract class Character : Entity
    {
        protected Character(char symbol, Vector2D position) : base(symbol, position) { }
        public void MoveUp()
        {
            Position += new Vector2D(-1, 0);
        }
        public void MoveDown()
        {
            Position += new Vector2D(1, 0);
        }
        public void MoveLeft()
        {
            Position += new Vector2D(0, -1);
        }
        public void MoveRight()
        {
            Position += new Vector2D(0, 1);
        }
        public override void Interact(Player target) { }
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
                    "Symbol=A\n" +
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
                    case "Symbol":
                        Symbol = char.Parse(value);
                        break;

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
        public override void Interact(Player player)
        {
            Console.WriteLine(Dialogue);
        }
    }

}

