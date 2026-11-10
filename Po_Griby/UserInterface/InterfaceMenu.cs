using Po_Griby.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po_Griby.UserInterface
{
    internal class InterfaceMenu
    {
        private Inventory inventory;

        public InterfaceMenu(Inventory inventory)
        {
            this.inventory = inventory;
        }
        public void Show_T()
        {
            Console.SetCursorPosition(70, 23);
            Console.WriteLine("====== 📖 КВЕСТИ 📖 ======");
            // вивід, налаштування завдань/квестів
        }

        public void Show_D()
        {
            Console.SetCursorPosition(70, 13);
            Console.WriteLine("====== 👤 ДІАЛОГИ 👤 ======");
            // вивід, налаштування діалогів з сутностями
        }

        public IReadOnlyList<MushroomItem> Items => inventory.Items;
        public void Show_I()
        {
            int x = 70;
            int y = 0;

            Console.SetCursorPosition(x, y);
            Console.WriteLine("====== 🎒 ІНВЕНТАР 🎒 ======");

            y++;

            if (inventory.Items.Count == 0)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine("Інвентар порожній.");
                return;
            }

            for (int i = 0; i < inventory.Items.Count; i++)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine($"{i + 1}. {inventory.Items[i].Name} - {inventory.Items[i].Description}");
                y++;
            }
        }
    }
}
