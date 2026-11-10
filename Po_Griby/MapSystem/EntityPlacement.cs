using System.Text.Json;
using System.Text.Json.Serialization;
using Po_Griby.Gameplay;

namespace Po_Griby.MapSystem
{
    public sealed class EntityDefinitionsFile
    {
        public List<EntityDefinition> Entities { get; set; } = new();
    }

    public sealed class EntityDefinition
    {
        public char Symbol { get; set; }
        public ConsoleColor Color { get; set; }
        public string Type { get; set; } = "";
        public string? Dialogue { get; set; }
        public string? Target { get; set; }
        public int HealingValue { get; set; } = 10;
        public MovePattern MovePattern { get; set; } = Po_Griby.Gameplay.MovePattern.Stationary;
    }

    public class EntityPlacement
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public char Symbol { get; }
        public ConsoleColor Color { get; }
        public string ClassName { get; }
        public string? Dialogue { get; }
        public string? Target { get; }
        public int HealingValue { get; }
        public MovePattern MovePattern { get; }

        public EntityPlacement(
            char symbol,
            ConsoleColor color,
            string className,
            string? dialogue,
            string? target = null,
            int healingValue = 10,
            MovePattern movePattern = MovePattern.Stationary)
        {
            Symbol = symbol;
            Color = color;
            ClassName = className;
            Dialogue = string.IsNullOrWhiteSpace(dialogue) ? null : dialogue;
            Target = string.IsNullOrWhiteSpace(target) ? null : target;
            HealingValue = healingValue;
            MovePattern = movePattern;
        }

        public Entity CreateEntity(Vector2D position)
        {
            Entity entity = ClassName switch
            {
                "Player" => new Player(Symbol, position),
                "BasicNPC" => new BasicNPC(Symbol, position, Dialogue ?? string.Empty),
                "Mushroom" => new Mushroom(Symbol, position, HealingValue),
                "MapChanger" => new MapChanger(Symbol, position, Target),
                "FalseBoletus" => new FalseBoletus(Symbol, position),
                "PuffballMushroom" => new PuffballMushroom(Symbol, position),
                "WildBoar" => new WildBoar(Symbol, position),
                "WolfPack" => new WolfPack(Symbol, position),
                "BearMiniBoss" => new BearMiniBoss(Symbol, position),
                _ => throw new InvalidOperationException($"Невідомий тип сутності: {ClassName}")
            };

            if (entity is MovingCharacter movingCharacter)
                movingCharacter.MovePattern = MovePattern;

            return entity;
        }

        public static Dictionary<char, EntityPlacement> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                var defaults = new EntityDefinitionsFile
                {
                    Entities =
                    {
                        new() { Symbol = 'P', Color = ConsoleColor.Gray, Type = "Player" },
                        new() { Symbol = 'E', Color = ConsoleColor.Red, Type = "BasicNPC", Dialogue = "Вітаю! Ласкаво просимо." },
                        new() { Symbol = 'N', Color = ConsoleColor.Green, Type = "BasicNPC", Dialogue = "На півночі є грибний ліс, і там живуть NPC." },
                        new() { Symbol = 'g', Color = ConsoleColor.Green, Type = "Mushroom", HealingValue = 10 },
                        new() { Symbol = 'm', Color = ConsoleColor.DarkRed, Type = "FalseBoletus" },
                        new() { Symbol = 'p', Color = ConsoleColor.Yellow, Type = "PuffballMushroom" },
                        new() { Symbol = 'B', Color = ConsoleColor.DarkYellow, Type = "WildBoar" },
                        new() { Symbol = 'W', Color = ConsoleColor.DarkGray, Type = "WolfPack" },
                        new() { Symbol = 'M', Color = ConsoleColor.Magenta, Type = "BearMiniBoss" }
                    }
                };
                Save(filePath, defaults);
            }

            string json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<EntityDefinitionsFile>(json, JsonOptions)
                ?? throw new InvalidDataException("Файл визначень сутностей порожній.");

            var definitions = new Dictionary<char, EntityPlacement>();
            foreach (EntityDefinition definition in data.Entities)
            {
                if (definition.Symbol is '#' or '.' or 'X')
                    throw new InvalidDataException($"Символ '{definition.Symbol}' зарезервований для шару прохідності.");
                if (string.IsNullOrWhiteSpace(definition.Type))
                    throw new InvalidDataException($"Для сутності '{definition.Symbol}' не вказано тип.");

                var placement = new EntityPlacement(
                    definition.Symbol,
                    definition.Color,
                    definition.Type,
                    definition.Dialogue,
                    definition.Target,
                    definition.HealingValue,
                    definition.MovePattern);
                if (!definitions.TryAdd(definition.Symbol, placement))
                    throw new InvalidDataException($"Символ сутності '{definition.Symbol}' указано кілька разів.");
            }
            return definitions;
        }

        private static void Save(string filePath, EntityDefinitionsFile data)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(data, JsonOptions));
        }
    }
}
