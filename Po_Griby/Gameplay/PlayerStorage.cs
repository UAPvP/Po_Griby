using System.Text.Json;

namespace Po_Griby.Gameplay
{
    public sealed class PlayerData
    {
        public int Health { get; set; } = 20;
        public int AttackSpeed { get; set; } = 3;
        public int AttackDamage { get; set; } = 5;
        public int DefenseSpeed { get; set; } = 2;
        public int DefenseShield { get; set; }
        public List<MushroomItemData> Inventory { get; set; } = new();
    }

    public sealed class MushroomItemData
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int HealingValue { get; set; }
    }

    public static class PlayerStorage
    {
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static PlayerData Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaults = new PlayerData
                {
                    Inventory =
                    {
                        new() { Name = "Білий гриб", Description = "Відновлює 10 HP", HealingValue = 10 },
                        new() { Name = "Мухомор", Description = "Дивний гриб...", HealingValue = 5 }
                    }
                };
                File.WriteAllText(filePath, JsonSerializer.Serialize(defaults, Options));
                return defaults;
            }

            return JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(filePath), Options)
                ?? throw new InvalidDataException("Файл даних гравця порожній.");
        }

        public static void Save(string filePath, Player player)
        {
            var data = new PlayerData
            {
                Health = player.Health,
                AttackSpeed = player.AttackSpeed,
                AttackDamage = player.AttackDamage,
                DefenseSpeed = player.DefenseSpeed,
                DefenseShield = player.DefenseShield,
                Inventory = player.Inventory.Select(item => new MushroomItemData
                {
                    Name = item.Name,
                    Description = item.Description,
                    HealingValue = item.HealingValue
                }).ToList()
            };

            File.WriteAllText(filePath, JsonSerializer.Serialize(data, Options));
        }
    }
}
