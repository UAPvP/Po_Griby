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
        public int HealingValue => Type == ItemType.Heal ? Value : 0;
        public bool IsQuestMushroom { get; }


        public MushroomItem(string name, ItemType type, int value, string description, bool isQuestMushroom = false)
        {
            Name = name;
            Type = type;
            Value = value;
            Description = description;
            IsQuestMushroom = isQuestMushroom;
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
        public int MaxItems { get; } = 12;
        public bool IsFull()
        {
            return items.Count >= MaxItems;
        }

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

        public void Clear()
        {
            items.Clear();
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < items.Count)
            {
                items.RemoveAt(index);
            }
        }

        public int CountItems()
        {
            return items.Count;
        }

        public bool RemoveItems(int amount)
        {
            if (amount < 1 || items.Count < amount)
                return false;

            for (int i = 0; i < amount; i++)
            {
                items.RemoveAt(0);
            }

            return true;
        }

        public bool RemoveQuestItems(int amount)
        {
            int count = items.Count(item => item.IsQuestMushroom);

            if (count < amount)
                return false;

            for (int i = items.Count - 1; i >= 0 && amount > 0; i--)
            {
                if (items[i].IsQuestMushroom)
                {
                    items.RemoveAt(i);
                    amount--;
                }
            }

            return true;
        }

        public bool IsEmpty()
        {
            return items.Count == 0;
        }
    }
}
