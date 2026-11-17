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

        public string? QuestId { get; set; }
        public string? QuestDescription { get; set; }
        public int QuestAmount { get; set; } = 1;
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

        public string? QuestId { get; }
        public string? QuestDescription { get; }
        public int QuestAmount { get; }

        public EntityPlacement(
            char symbol,
            ConsoleColor color,
            string className,
            string? dialogue,
            string? target = null,
            int healingValue = 10,
            MovePattern movePattern = MovePattern.Stationary,
            string? questId = null,
            string? questDescription = null,
            int questAmount = 1)
        {
            Symbol = symbol;
            Color = color;
            ClassName = className;
            Dialogue = string.IsNullOrWhiteSpace(dialogue) ? null : dialogue;
            Target = string.IsNullOrWhiteSpace(target) ? null : target;
            HealingValue = healingValue;
            MovePattern = movePattern;
            QuestId = questId;
            QuestDescription = questDescription;
            QuestAmount = questAmount;

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
                "QuestNPC" => new QuestNPC(Symbol, position, Dialogue ?? string.Empty, QuestId ?? "", QuestDescription ?? "", QuestAmount),
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

                        // Основні NPC
                        new() { Symbol = 'E', Color = ConsoleColor.Red, Type = "BasicNPC", Dialogue = "Вітаю! Ласкаво просимо." },
                        new() { Symbol = 'N', Color = ConsoleColor.Green, Type = "BasicNPC", Dialogue = "На півночі є грибний ліс, і там живуть NPC." },

                        // Гриби
                        new() { Symbol = 'g', Color = ConsoleColor.Green, Type = "Mushroom", HealingValue = 10 },
                        new() { Symbol = 'm', Color = ConsoleColor.DarkRed, Type = "FalseBoletus" },
                        new() { Symbol = 'p', Color = ConsoleColor.Yellow, Type = "PuffballMushroom" },

                        // Вороги
                        new() { Symbol = 'B', Color = ConsoleColor.DarkYellow, Type = "WildBoar" },
                        new() { Symbol = 'W', Color = ConsoleColor.DarkGray, Type = "WolfPack" },
                        new() { Symbol = 'M', Color = ConsoleColor.Magenta, Type = "BearMiniBoss" },

                        // Перехід між картами
                        new() { Symbol = '1', Color = ConsoleColor.White, Type = "MapChanger", Dialogue = "Зміна мапи", Target = "map2.txt" },
                        new() { Symbol = '2', Color = ConsoleColor.White, Type = "MapChanger", Dialogue = "Зміна мапи", Target = "map3.txt" },
                        new() { Symbol = '3', Color = ConsoleColor.White, Type = "MapChanger", Dialogue = "Зміна мапи", Target = "map4.txt" },

                        // NPC першої карти
                        new() { Symbol = 'd', Color = ConsoleColor.White, Type = "BasicNPC", Dialogue = "О, грибничкуємо? Тільки мухомори не збирай, я минулого разу після них 3 дня з деревом розмовляв" },
                        new() { Symbol = 'Y', Color = ConsoleColor.White, Type = "BasicNPC", Dialogue = "Я вже четверту годину не можу найти вихід, а він тут точно є?..." },
                        new() { Symbol = 'D', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Ти теж по гриби прийшов? Хм... а я думав що всі вже пішли" },
                        new() { Symbol = 'b', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Мені треба всього 3 грибочки. Тільки не ті червоні, від них сни потім цікаві" },
                        new() { Symbol = 'h', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Акуратніше, куди на мій город прешся, дурень безсовісний" },
                        new() { Symbol = 'с', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Мяу мяу" },

                        // Грибний король — видає квест
                        new()
                        {
                        Symbol = 'K',
                        Color = ConsoleColor.DarkYellow,
                        Type = "QuestNPC",
                        Dialogue = "Схились перед Великим грибним королем, ти тут ніхто, хочеш вижити!",
                        QuestId = "collect_5_mushrooms",
                        QuestDescription = "Знайди та принеси мені 5 грибів.",
                        QuestAmount = 5
                        },

                        new() { Symbol = 'l', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Як думаєш чи існує місце де гусиниці керують світом?..." },
                        new() { Symbol = 'A', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Нічого собі, ще один дурень зайшов у цей ліс, хочеш чогось випити? Води, зілля чи чогось міцнішого?" },
                        new() { Symbol = 'T', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Я просто хотів знайти білий гриб. ЧОМУ ВОНИ ВСІ ДИВЛЯТЬСЯ НА МЕНЕ??" },
                        new() { Symbol = 'S', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Привіт, ти не бачив мого носа? Я його десь загубив... хоча він здається на місці..." },

                        // NPC інших карт
                        new() { Symbol = 'F', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Дивно, сніг йде вже 3тій день.. а от сліди твої тут здається були ще до того як ти прийшов." },
                        new() { Symbol = 'O', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Не довіряй тому хто пам'ятає тебе краще ніж ти сам" },
                        new() { Symbol = 'f', Color = ConsoleColor.DarkYellow, Type = "BasicNPC", Dialogue = "Хочеш знайти вихід? тоді перестань шукати дорогу " }
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

                if (definition.Type == "QuestNPC" &&
                    (string.IsNullOrWhiteSpace(definition.QuestId) ||
                    string.IsNullOrWhiteSpace(definition.QuestDescription) ||
                    definition.QuestAmount < 1))
                {
                    throw new InvalidDataException($"Для QuestNPC '{definition.Symbol}' неправильно задано квест.");
                }

                var placement = new EntityPlacement(
                    definition.Symbol,
                    definition.Color,
                    definition.Type,
                    definition.Dialogue,
                    definition.Target,
                    definition.HealingValue,
                    definition.MovePattern,
                    definition.QuestId,
                    definition.QuestDescription,
                    definition.QuestAmount);
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
