using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po_Griby.UserInterface
{
    public class Cursore
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Cursore(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Cursore()
        {
        }

        public void tp_Cursor(int x, int y)
        {
            this.X = x;
            this.Y = y;

            Console.SetCursorPosition(X, Y);
        }
    }

    public class CursorePrintText
    {
        public void PrintText(int x, int y, string text)
        {
            Cursore cursore = new Cursore(x, y);

            cursore.tp_Cursor(x, y);
            Console.WriteLine(text);
        }
    }

    public class PrintMirror
    {
        public void PrintMirrorAt(int x, int y, string text)
        {
            char[] chars = text.ToCharArray();
            Array.Reverse(chars);

            int newX = Console.WindowWidth - x - text.Length;

            Console.SetCursorPosition(newX, y);
            Console.Write(chars);
        }
    }

    public class delay_text
    {
        public static void PrintText(string text, int delay = 50)
        {
            foreach (char letter in text)
            {
                Console.Write(letter);
                System.Threading.Thread.Sleep(delay);
            }
        }
        public static void PrintText_s(string text, int delay = 200)
        {
            foreach (char letter in text)
            {
                Console.Write(letter);
                System.Threading.Thread.Sleep(delay);
            }
        }
    }


}
