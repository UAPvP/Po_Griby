using Po_Griby.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Po_Griby.UserInterface.Decorations;

namespace Po_Griby.Interface
{
    public class Menu
    {
        public string Header { get; }
        public List<MenuSection> sections { get; } = new();
        private List<int> optionYPositions = new List<int>();
        Cursore cursor = new();
        CursorePrintText textCursor = new();
        Decorations.ForestAnimation forest = new();
        SnowAnimation snow = new SnowAnimation();
        private bool exitMenu = false;


        public void Close()
        {
            exitMenu = true;
        }
        private int selectedIndex = 0;

        public Menu(string header)
        {
            Header = header;
        }

        public void AddSection(MenuSection section)
        {
            sections.Add(section);
        }

        private List<Option> GetAllOptions()
        {
            return sections
                .SelectMany(s => s.options)
                .ToList();
        }

        public void MoveNext() //вказівник наступний
        {
            List<Option> options = GetAllOptions();


            if (options.Count == 0)
                return;

            selectedIndex++;

            if (selectedIndex >= options.Count)
                selectedIndex = 0;


        }

        public void MovePrevious() //вказівник попередній
        {
            List<Option> options = GetAllOptions();

            if (options.Count == 0)
                return;

            selectedIndex--;

            if (selectedIndex < 0)
                selectedIndex = options.Count - 1;
        }

        public void Render() //відображення
        {
            int i = 2;
            Console.Clear();

            snow.UpdateSnow();

            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);

            cursor.tp_Cursor(48, 7);
            Console.WriteLine(Header);

            cursor.tp_Cursor(53, 9);

            foreach (MenuSection section in sections)
            {
                Console.WriteLine(section.title);

                foreach (Option option in section.options)
                {
                    int y = 9 + i;

                    optionYPositions.Add(y);

                    cursor.tp_Cursor(52, y);
                    Console.WriteLine(option.name);

                    i = i + 2;
                }
            }

            textCursor.PrintText(52, optionYPositions[selectedIndex], "-->");
        }

        public void SelectCurrent() //натиснути обраний варіант
        {
            List<Option> options = GetAllOptions();

            if (options.Count == 0)
                return;

            options[selectedIndex].Execute();
        }

        public void Run()
        {
            Console.CursorVisible = false;

            Render();

            if (optionYPositions.Count == 0)
                return;

            while (!exitMenu)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.DownArrow)
                    {
                        MoveNext();
                    }
                    else if (key == ConsoleKey.UpArrow)
                    {
                        MovePrevious();
                    }
                    else if (key == ConsoleKey.Enter)
                    {
                        SelectCurrent();
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }

                Render();

                Thread.Sleep(150);
            }

            Console.CursorVisible = true;
        }

    }
}
