using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Po_Griby.UserInterface;

namespace Po_Griby.Interface
{
    public class Menu
    {
        public string Header { get; }
        public List<MenuSection> sections { get; } = new();

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
            throw new NotImplementedException();
        }

        public void MovePrevious() //вказівник попередній
        {
            throw new NotImplementedException();
        }

        public void Render() //відображення
        {
            throw new NotImplementedException();
        }

        public void SelectCurrent() //натиснути обраний варіант
        {
            throw new NotImplementedException();
        }
    }
}
