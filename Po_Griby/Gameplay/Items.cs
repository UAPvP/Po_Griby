using System;

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
                    Console.WriteLine($"[Ефект] Відновлено {Value} HP!");
                    break;

                case ItemType.DamageBuff:
                    Console.WriteLine($"[Ефект] Урон збільшено на {Value} для наступного удару!");
                    break;

                case ItemType.Shield:
                    Console.WriteLine($"Ви встали в захисну стійку![Ефект] Отримано {Value} щита!");
                    break;

                case ItemType.Poison:
                    Console.WriteLine($"[Ефект] Ой! Ви отруїлися і втратили {Value} HP!");
                    player.TakeDamage(Value);
                    break;

                case ItemType.Placebo:
                    Console.WriteLine("[Ефект] Ви почуваєтеся як вам стало краще... Але нічого не змінилось.");
                    break;
            }
        }
    }
}
