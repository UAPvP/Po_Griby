using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po_Griby.UserInterface
{
    internal class Decorations
    {
        public class ForestAnimation
        {
            PrintMirror mirrorTree = new PrintMirror();
            public void DrawForest(ConsoleColor TreeColor1, ConsoleColor TreeColor2)
            {
                // Ліва ялинка
                Console.ForegroundColor = TreeColor1;

                PrintAt(1, 14, "#");
                PrintAt(1, 15, "##");
                PrintAt(1, 16, "###         #");
                PrintAt(1, 17, "####       ###");
                PrintAt(1, 18, "#####     #####");
                PrintAt(1, 19, "######   #######");
                PrintAt(1, 20, "####### #########");
                PrintAt(1, 21, "##################");
                PrintAt(1, 22, "###################");
                PrintAt(1, 23, "####################");
                PrintAt(1, 24, "#####################");
                PrintAt(1, 25, "######################");
                PrintAt(1, 26, "#######################");
                PrintAt(1, 27, "## ##  ##  ### ###### ##");
                PrintAt(1, 27, "## #   #   # #  ####   ##");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                PrintAt(12, 27, "###");
                PrintAt(12, 28, "###");
                PrintAt(12, 29, "###");

                PrintAt(1, 27, "#");
                PrintAt(1, 28, "#");
                PrintAt(1, 29, "#");

                Console.ForegroundColor = TreeColor2;
                PrintAt(28, 19, "#");
                PrintAt(27, 20, "###");
                PrintAt(26, 21, "#####          #");
                PrintAt(25, 22, "#######        ###");
                PrintAt(24, 23, "#########      #####    #");
                PrintAt(23, 24, "###########    #######  ###");
                PrintAt(22, 25, "#############  ##############");
                PrintAt(21, 26, "###############################");
                PrintAt(20, 27, "# ###### ### ###################");
                PrintAt(19, 27, "## ######### ###### ##### ###### ##");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                PrintAt(28, 27, "##");
                PrintAt(27, 28, "###");
                PrintAt(27, 29, "###");

                PrintAt(40, 28, "##");
                PrintAt(40, 29, "##");

                PrintAt(48, 28, "#");
                PrintAt(48, 29, "#");

                //Right
                Console.ForegroundColor = TreeColor1;
                mirrorTree.PrintMirrorAt(1, 14, "#");
                mirrorTree.PrintMirrorAt(1, 15, "##");
                mirrorTree.PrintMirrorAt(1, 16, "###         #");
                mirrorTree.PrintMirrorAt(1, 17, "####       ###");
                mirrorTree.PrintMirrorAt(1, 18, "#####     #####");
                mirrorTree.PrintMirrorAt(1, 19, "######   #######");
                mirrorTree.PrintMirrorAt(1, 20, "####### #########");
                mirrorTree.PrintMirrorAt(1, 21, "##################");
                mirrorTree.PrintMirrorAt(1, 22, "###################");
                mirrorTree.PrintMirrorAt(1, 23, "####################");
                mirrorTree.PrintMirrorAt(1, 24, "#####################");
                mirrorTree.PrintMirrorAt(1, 25, "######################");
                mirrorTree.PrintMirrorAt(1, 26, "#######################");
                mirrorTree.PrintMirrorAt(1, 27, "## #   #   # #  ####   ##");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                mirrorTree.PrintMirrorAt(12, 27, "###");
                mirrorTree.PrintMirrorAt(12, 28, "###");
                mirrorTree.PrintMirrorAt(12, 29, "###");

                Console.ForegroundColor = TreeColor2;
                mirrorTree.PrintMirrorAt(28, 19, "#");
                mirrorTree.PrintMirrorAt(27, 20, "###");
                mirrorTree.PrintMirrorAt(26, 21, "#####          #");
                mirrorTree.PrintMirrorAt(25, 22, "#######        ###");
                mirrorTree.PrintMirrorAt(24, 23, "#########      #####    #");
                mirrorTree.PrintMirrorAt(23, 24, "###########    #######  ###");
                mirrorTree.PrintMirrorAt(22, 25, "#############  ##############");
                mirrorTree.PrintMirrorAt(21, 26, "###############################");
                mirrorTree.PrintMirrorAt(20, 27, "# ##  ## ### ###################");
                mirrorTree.PrintMirrorAt(19, 27, "## #   ##  # #  ### ##### ###### ##");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                

                mirrorTree.PrintMirrorAt(1, 27, "#");
                mirrorTree.PrintMirrorAt(1, 28, "#");
                mirrorTree.PrintMirrorAt(1, 29, "#");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                mirrorTree.PrintMirrorAt(28, 27, "##");
                mirrorTree.PrintMirrorAt(27, 28, "###");
                mirrorTree.PrintMirrorAt(27, 29, "###");

                mirrorTree.PrintMirrorAt(40, 28, "##");
                mirrorTree.PrintMirrorAt(40, 29, "##");

                mirrorTree.PrintMirrorAt(48, 28, "#");
                mirrorTree.PrintMirrorAt(48, 29, "#");

                Console.ResetColor();
            }

            

                private void PrintAt(int x, int y, string text)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(text);
            }
        }

        public class ForestAnimation2
        {
            PrintMirror mirrorTree = new PrintMirror();
            draw_steps steps = new draw_steps();
            public void DrawForest(ConsoleColor TreeColor1, ConsoleColor TreeColor2)
            {
                Console.Clear();

                int width = Console.WindowWidth;
                int height = Console.WindowHeight;
                int center = width / 2;



                PrintTree(3, 10, 16, TreeColor1);
                PrintTree(13, 8, 16, TreeColor2);
                PrintTree(25, 7, 12, TreeColor1);
                PrintTree(36, 5, 12, TreeColor2);
                PrintTree(44, 5, 10, TreeColor1);
                
                

                PrintTree(102, 14, 17, TreeColor2);
                PrintTree(90, 11, 14, TreeColor1);
                PrintTree(78, 10, 12, TreeColor2);
                PrintTree(67, 8, 10, TreeColor1);
                PrintTree(63, 7, 10, TreeColor2);
                PrintTree(56, 5, 10, TreeColor1);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                for (int y = height / 3; y < height; y++)
                {
                    int roadWidth = (y - height / 3) * width / height;

                    int left = center - roadWidth;
                    int right = center + roadWidth;

                    for (int x = Math.Max(0, left + 1); x < Math.Min(width, right); x++)
                    {
                        PrintAtSafe(x, y, ".");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Gray;

                for (int y = height / 3; y < height; y++)
                {
                    int roadWidth = (y - height / 3) * width / height;

                    int left = center - roadWidth;
                    int right = center + roadWidth;

                    PrintAtSafe(left, y, "/");
                    PrintAtSafe(right, y, "\\");
                }

                PrintTree(-15, 13, 17, TreeColor1);


                AnimateSteps();

                Console.ResetColor();
            }

            public void PrintTree(int x, int y, int size, ConsoleColor color)
            {
                Console.ForegroundColor = color;

                for (int i = 0; i < size; i++)
                {
                    int spaces = size - i - 1;
                    int branches = i * 2 + 1;

                    for (int j = 0; j < branches; j++)
                    {
                        int posX = x + spaces + j;
                        int posY = y + i;

                        if (posX >= 0 && posX < Console.WindowWidth &&
                            posY >= 0 && posY < Console.WindowHeight)
                        {
                            Console.SetCursorPosition(posX, posY);
                            Console.Write("#");
                        }
                    }
                }

                Console.ResetColor();
            }

            private void PrintAtSafe(int x, int y, string text)
            {
                if (x < 0 || y < 0 ||
                    y >= Console.WindowHeight ||
                    x + text.Length > Console.WindowWidth)
                    return;

                Console.SetCursorPosition(x, y);
                Console.Write(text);
            }

            public void AnimateSteps()
            {
                int center = Console.WindowWidth / 2;
                int height = Console.WindowHeight;

                int y = height - 5;

                for (int i = 0; i < 5; i++)
                {
                    if (i % 2 == 0)
                    {
                        steps.Step_l(center - 4, y);
                    }
                    else
                    {
                        steps.Step_r(center + 1, y);
                    }

                    y -= 3;

                    System.Threading.Thread.Sleep(400);
                }
            }
        }

        public class draw_steps
        {
            public void Step_r(int x, int y)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine("##");
                Console.SetCursorPosition(x, y+1);
                Console.WriteLine("###");
                Console.SetCursorPosition(x, y + 2);
                Console.WriteLine("###");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine("##");
                
            }

            public void Step_l(int x, int y)
            {
                Console.SetCursorPosition(x, y);
                Console.WriteLine("##");
                Console.SetCursorPosition(x-1, y + 1);
                Console.WriteLine("###");
                Console.SetCursorPosition(x-1, y + 2);
                Console.WriteLine("###");
                Console.SetCursorPosition(x, y + 3);
                Console.WriteLine("##");
                
            }
        }

        public class SnowAnimation
        {
            private Random random = new Random();

            private List<int> snowX = new List<int>();
            private List<int> snowY = new List<int>();

            public SnowAnimation()
            {
                for (int i = 0; i < 50; i++)
                {
                    snowX.Add(random.Next(1, Console.WindowWidth - 1));
                    snowY.Add(random.Next(0, Console.WindowHeight));
                }
            }
            public void UpdateSnow()
            {
                Console.ForegroundColor = ConsoleColor.White;

                for (int i = 0; i < snowX.Count; i++)
                {
                    snowY[i]++;

                    if (snowY[i] >= Console.WindowHeight)
                    {
                        snowY[i] = 0;
                        snowX[i] = random.Next(1, Console.WindowWidth - 1);
                    }

                    Console.SetCursorPosition(snowX[i], snowY[i]);
                    Console.Write("*");
                }

                Console.ResetColor();
            }
        }
    }

    public class  game_plot
    {
        Decorations.ForestAnimation forest = new();
        Decorations.ForestAnimation2 forest2 = new();
        public void plot ()
        {
            Cursore cursor = new();
            Console.Clear();
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);

            cursor.tp_Cursor(48, 7);
            delay_text.PrintText("WELCOME TO FOREST");
            
            cursor.tp_Cursor(34, 9);
            delay_text.PrintText("Чи боялися ви коли-небудь ходити в ліс?     ");
            forest.DrawForest(ConsoleColor.DarkRed, ConsoleColor.Red);
            cursor.tp_Cursor(35, 10);
            Console.ForegroundColor = ConsoleColor.Red;
            
            delay_text.PrintText_s("Ні?");
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.White;
            cursor.tp_Cursor(34, 11);
            delay_text.PrintText("А варто було б,");
            cursor.tp_Cursor(34, 12);
            delay_text.PrintText("хто знає чи після одної такої прогулянки ти повернешся до дому...");
            cursor.tp_Cursor(34, 13);
            delay_text.PrintText("і чи взагалі виберешся звідти ...");
            forest.DrawForest(ConsoleColor.DarkRed, ConsoleColor.Red);
            cursor.tp_Cursor(64, 13);
            Console.ForegroundColor = ConsoleColor.Red;
            
            delay_text.PrintText_s("живим");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Clear();
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);
            cursor.tp_Cursor(48, 7);
            Console.WriteLine("WELCOME TO FOREST");

            cursor.tp_Cursor(34, 9);
            delay_text.PrintText("Звичайний пасмурний день, дощ паріщив як з відра,");
            cursor.tp_Cursor(34, 10);
            delay_text.PrintText("а це значить що в лісі грибів буде вдосталь.");
            cursor.tp_Cursor(34, 11);
            delay_text.PrintText("Ти зібрався, вдягнувся, взяв телефон, ");
            cursor.tp_Cursor(34, 12);
            delay_text.PrintText("в рюкзак запихнув перше що попалось під руку:");
            cursor.tp_Cursor(34, 13);
            delay_text.PrintText("пачку печеня, домашній ніж який вже давно бажано було заточити    ");
            cursor.tp_Cursor(34, 14);
            delay_text.PrintText("і аптечку");
            cursor.tp_Cursor(34, 15);
            delay_text.PrintText("ти вже й не пам'ятаєш коли останній раз виходив кудись без неї,  ");
            cursor.tp_Cursor(34, 16);
            delay_text.PrintText("анемія все     ");
            forest.DrawForest(ConsoleColor.DarkRed, ConsoleColor.Red);
            cursor.tp_Cursor(35, 17);
            Console.ForegroundColor = ConsoleColor.Red;

            delay_text.PrintText_s("прогресує й прогресує");
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.White;
            
            Console.Clear();

            forest2.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);

            Console.Clear();

            Console.Clear();
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);
            cursor.tp_Cursor(48, 7);
            Console.WriteLine("WELCOME TO FOREST");

            cursor.tp_Cursor(34, 9);
            delay_text.PrintText("І ось ти в лісі, на диво тут доволі...");
            forest.DrawForest(ConsoleColor.DarkRed, ConsoleColor.Red);
            cursor.tp_Cursor(35, 10);
            Console.ForegroundColor = ConsoleColor.Red;

            delay_text.PrintText_s("Тихо");
            forest.DrawForest(ConsoleColor.DarkGreen, ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.White;
            cursor.tp_Cursor(34, 11);
            delay_text.PrintText("Ти спокійно ходиш по лісу, навіть знайшов ");
            cursor.tp_Cursor(34, 12);
            delay_text.PrintText("Декілька красивих білих грибів");
            cursor.tp_Cursor(34, 13);
            delay_text.PrintText("В моменті ти підскакуєш від блискавки яка вдарила за метрів 30 від тебе");
            cursor.tp_Cursor(34, 14);
            delay_text.PrintText("Ти не втримав рівновагу й впав покотившись в невелику прірву позаду тебе");
            cursor.tp_Cursor(34, 15);
            delay_text.PrintText("Впавши ти сильно вдарився в голову, через хворобу ти ітак був ослаблений,");
            cursor.tp_Cursor(34, 16);
            delay_text.PrintText("І навіть такий невеличкий удар зміг позбавити тебе свідомості...");
            delay_text.PrintText_s("     ");
            Console.Clear();
            forest.DrawForest(ConsoleColor.DarkRed, ConsoleColor.Red);
            cursor.tp_Cursor(48, 7);
            Console.ForegroundColor = ConsoleColor.Red;

            delay_text.PrintText_s("Через декілька годин     ");

            Console.Clear();

        }


    }

}
