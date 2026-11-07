using System;
using System.Collections.Generic;

namespace Po_Griby.Gameplay
{
    public enum ItemType
    {
        Heal,
        DamageBuff,
        Shield,
        Poison,
        Placebo
    }

    public class MushroomItem
    {
        public string Name { get; }
        public ItemType Type { get; }
        public int Value { get; }
        public string Description { get; }

        public MushroomItem(string name, ItemType type, int value, string description)
        {
            Name = name;
            Type = type;
            Value = value;
            Description = description;
        }

        public void Use(Player player)
        {
            Console.WriteLine($"\nВи використали: {Name}!");
            Console.WriteLine(Description);

            switch (Type)
            {
                case ItemType.Heal:
                    player.Heal(Value);
                    Console.WriteLine($"Відновлено {Value} HP!");
                    break;

                case ItemType.DamageBuff:
                    Console.WriteLine($"Шкоду збільшено на {Value} для наступного удару!");
                    break;

                case ItemType.Shield:
                    Console.WriteLine($"Отримано {Value} щита!");
                    break;

                case ItemType.Poison:
                    Console.WriteLine($"Ой! Ви отруїлися і втратили {Value} HP!");
                    player.TakeDamage(Value);
                    break;

                case ItemType.Placebo:
                    Console.WriteLine("Ви почуваєтеся трохи впевненіше... Але нічого не сталося.");
                    break;
            }
        }
    }

    public class Inventory
    {
        private readonly List<MushroomItem> items = new();

        public IReadOnlyList<MushroomItem> Items => items;

        public Inventory()
        {
            items.Add(new MushroomItem("Білий гриб (Боровик)", ItemType.Heal, 10, "Справжній скарб лісу.."));
            items.Add(new MushroomItem("Мухомор", ItemType.Poison, 5, "Виглядає гарно, але їсти не варто."));
            items.Add(new MushroomItem("ПлацеГриб", ItemType.Placebo, 0, "Звичайний поганка, від якої чомусь стає смішно."));
        }

        public void AddItem(MushroomItem item)
        {
            items.Add(item);
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                items.RemoveAt(index);
            }
        }

        public bool IsEmpty()
        {
            return items.Count == 0;
        }
    }
}
