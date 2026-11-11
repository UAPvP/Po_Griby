
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Po_Griby
{
    public class SaveData
    {
        public string PlayerName { get; set; } = "Player";
        public int Level { get; set; } = 1;
        public int Experience { get; set; }
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Gold { get; set; }

        public string CurrentMap { get; set; } = "forest_1";
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }

        public List<ItemData> Inventory { get; set; } = new List<ItemData>();
        public List<QuestData> Quests { get; set; } = new List<QuestData>();
        public Dictionary<string, bool> Flags { get; set; } = new Dictionary<string, bool>();
    }

    public class ItemData
    {
        public string Id { get; set; } = "";
        public int Count { get; set; }
    }

    public class QuestData
    {
        public string Id { get; set; } = "";
        public int Progress { get; set; }
        public bool Completed { get; set; }
    }

    public static class SaveSystem
    {
        static readonly string folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Po_Griby");

        static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        static string GetPath(int slot)
        {
            return Path.Combine(folder, "save" + slot + ".json");
        }

        public static bool Exists(int slot = 1)
        {
            return File.Exists(GetPath(slot));
        }

        public static bool Save(SaveData data, int slot = 1)
        {
            try
            {
                Directory.CreateDirectory(folder);

                File.WriteAllText(
                    GetPath(slot),
                    JsonSerializer.Serialize(data, options)
                );

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save: " + e.Message);
                return false;
            }
        }

        public static SaveData Load(int slot = 1)
        {
            try
            {
                if (!Exists(slot))
                    return null;

                return JsonSerializer.Deserialize<SaveData>(
                    File.ReadAllText(GetPath(slot))
                );
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load: " + e.Message);
                return null;
            }
        }

        public static void Delete(int slot = 1)
        {
            if (Exists(slot))
                File.Delete(GetPath(slot));
        }

        public static void QuickSave(SaveData data)
        {
            if (Save(data, 1))
            {
                Console.WriteLine("Game saved!");
            }
            else
            {
                Console.WriteLine("Save failed!");
            }
        }

        public static void CheckQuickSave(SaveData data)
        {
            if (!Console.KeyAvailable)
                return;

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.F5)
            {
                QuickSave(data);
            }
        }
    }
}
