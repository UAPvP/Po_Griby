using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po_Griby.UserInterface
{
    public class MenuSection
    {
        public string title { get; }
        public List<Option> options { get; } = new();

        public MenuSection(string title)
        {
            this.title = title;
        }
        public void AddOption(string name, string description, Action execute)
        {
            options.Add(new Option(name,description, execute));
        }
    }
}
