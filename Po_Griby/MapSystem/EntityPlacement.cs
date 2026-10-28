using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Po_Griby.Gameplay;

namespace Po_Griby.MapSystem
{
    public class EntityPlacement
    {
        public char Symbol { get; }
        public ConsoleColor Color { get; }
        public string ClassName { get; }
        public string? Dialogue { get; }

        public EntityPlacement(
            char symbol,
            ConsoleColor color,
            string className,
            string? dialogue)
        {
            Symbol = symbol;
            Color = color;
            ClassName = className;
            Dialogue = string.IsNullOrWhiteSpace(dialogue) ? null : dialogue;
        }

        public Entity CreateEntity(Vector2D position)
        {
            switch (ClassName)
            {
                case "Player":
                    return new Player(Symbol, position);

                case "BasicNPC":
                    return new BasicNPC(Symbol, position, Dialogue ?? string.Empty);

                case "MapChanger":
                    return new MapChanger(Symbol, position);

                default:
                    throw new InvalidOperationException(
                        $"Невідомий клас сутності: {ClassName}");
            }
        }

        public static Dictionary<char, EntityPlacement> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string[] exampleLines =
                {
                "P=Gray,Player,",
                "E=Red,BasicNPC,Вітаю! Ласкаво просимо.",
                "N=Green,BasicNPC,На півночі є грибний ліс, і там живуть NPC."
                };

                File.WriteAllLines(filePath, exampleLines);
            }
            var definitions = new Dictionary<char, EntityPlacement>();

            foreach (string originalLine in File.ReadAllLines(filePath))
            {
                string line = originalLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] entry = line.Split('=', 2);

                if (entry.Length != 2 || entry[0].Trim().Length != 1)
                    throw new FormatException($"Неправильний символ або формат рядка: {originalLine}");

                char symbol = entry[0].Trim()[0];
                if (symbol == '#' || symbol == '.' || symbol == 'X')
                {
                    throw new FormatException(
                        $"Символ '{symbol}' зарезервований для шару проходу карти.");
                }

                string[] parts = entry[1].Split(',', 3);

                if (parts.Length < 2)
                    throw new FormatException($"Не вказано колір або клас: {originalLine}");

                if (!Enum.TryParse(parts[0].Trim(), true, out ConsoleColor color))
                    throw new FormatException($"Невідомий колір: {parts[0].Trim()}");

                string className = parts[1].Trim();

                if (string.IsNullOrWhiteSpace(className))
                    throw new FormatException($"Не вказано назву класу: {originalLine}");

                string? dialogue = parts.Length == 3 ? parts[2].Trim() : null;

                if (definitions.ContainsKey(symbol))
                    throw new FormatException($"Символ '{symbol}' описано більше одного разу.");

                definitions.Add(
                    symbol,
                    new EntityPlacement(symbol, color, className, dialogue));
            }

            return definitions;
        }
    }
}
