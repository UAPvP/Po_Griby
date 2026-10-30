using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Po_Griby.UserInterface
{
    public class Option
    {
        public string name { get; }
        public string description { get; }
        public Action Execute { get; set; }

        public Option(string name, string description, Action Execute)
        {
            this.name = name;
            this.Execute = Execute;
        }
    }
}
