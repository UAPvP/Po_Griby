using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Po_Griby.UserInterface;
using static Po_Griby.UserInterface.Decorations;

namespace Po_Griby.Interface
{
    public class Menu
    {
        public string Header { get; }
        public List<MenuSection> sections { get; } = new();

        private readonly List<int> optionYPositions = new();
        private readonly Cursore cursor = new();
        private readonly CursorePrintText textCursor = new();
        private readonly Decorations.ForestAnimation forest = new();
        private readonly SnowAnimation snow = new();
        private bool exitMenu;
        private int selectedIndex;

        public Menu(string header)
        {
            Header = header;
        }

        public void AddSection(MenuSection section) => sections.Add(section);

        public void Close() => exitMenu = true;

        private List<Option> GetAllOptions() => sections.SelectMany(section => section.options).ToList();

        public void MoveNext()
        {
            int count = GetAllOptions().Count;
            if (count == 0)
                return;
            selectedIndex = (selectedIndex + 1) % count;
        }

        public void MovePrevious()
        {
            int count = GetAllOptions().Count;
            if (count == 0)
                return;
            selectedIndex = (selectedIndex - 1 + count) % count;
        }

        public void Render()
        {
            optionYPositions.Clear();
            Console.Clear();
            snow.UpdateSnow();
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);

            cursor.tp_Cursor(48, 7);
            Console.WriteLine(Header);
            cursor.tp_Cursor(53, 9);

            int row = 0;
            foreach (MenuSection section in sections)
            {
                Console.WriteLine(section.title);
                foreach (Option option in section.options)
                {
                    int y = 11 + row * 2;
                    optionYPositions.Add(y);
                    cursor.tp_Cursor(52, y);
                    Console.WriteLine(option.name);
                    row++;
                }
            }

            if (optionYPositions.Count > 0)
            {
                selectedIndex = Math.Clamp(selectedIndex, 0, optionYPositions.Count - 1);
                textCursor.PrintText(49, optionYPositions[selectedIndex], "-->");
            }
        }

        public void SelectCurrent()
        {
            List<Option> options = GetAllOptions();
            if (options.Count == 0)
                return;
            options[selectedIndex].Execute();
        }

        public void Run()
        {
            Console.CursorVisible = false;
            try
            {
                Render();
                if (optionYPositions.Count == 0)
                    return;

                while (!exitMenu)
                {
                    if (Console.KeyAvailable)
                    {
                        switch (Console.ReadKey(true).Key)
                        {
                            case ConsoleKey.DownArrow: MoveNext(); break;
                            case ConsoleKey.UpArrow: MovePrevious(); break;
                            case ConsoleKey.Enter: SelectCurrent(); break;
                            case ConsoleKey.Escape: exitMenu = true; break;
                        }
                    }

                    if (!exitMenu)
                    {
                        Render();
                        Thread.Sleep(150);
                    }
                }
            }
            finally
            {
                Console.CursorVisible = true;
            }
        }
    }
}