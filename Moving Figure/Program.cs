using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Media;
using System.Drawing;
using Original;
using Survival;
using Timed;

namespace MovingFigure
{
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        private const int SC_MAXIMIZE = 0xF030;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040; 
        private const uint SC_SIZE = 0xF000; 
        private const uint SWP_NOSIZE = 0x0001; 
        private static readonly IntPtr HWND_TOP = IntPtr.Zero; 
        private const int STD_INPUT_HANDLE = -10; 
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_SIZEBOX = 0x40000;
       

        
        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static void DisableQuickEditMode()
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            if (GetConsoleMode(consoleHandle, out uint mode))
            {
                mode &= ~ENABLE_QUICK_EDIT_MODE;
                SetConsoleMode(consoleHandle, mode);
            }
        }
        private static void DisableResize()
        { 
            IntPtr consoleWindow = GetConsoleWindow();
            IntPtr systemMenu = GetSystemMenu(consoleWindow, false);          
            DeleteMenu(systemMenu, SC_SIZE, MF_BYCOMMAND);      
            DeleteMenu(systemMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            int style = GetWindowLong(consoleWindow, GWL_STYLE);
            style &= ~WS_MAXIMIZEBOX;
            style &= ~WS_SIZEBOX;
            SetWindowLong(consoleWindow, GWL_STYLE, style);
           

        }
        static void MoveWindow(int x, int y)
        {
            IntPtr consoleWindow = GetConsoleWindow(); 
            if (consoleWindow == IntPtr.Zero)
            {
                Console.WriteLine("Unable to get console window handle.");
                return;
            }

            SetWindowPos(consoleWindow, HWND_TOP, x, y, 0, 0, SWP_NOSIZE);
        }
        private static int option = 1;
        public static Random random = new Random();


        static void Main(string[] args)
        {
            DisableQuickEditMode();
            DisableResize();
            
            Console.SetWindowSize(126, 35);
            Console.SetBufferSize(126, 35);

            MoveWindow(150, 50);
            Console.Title = "Moving Figure 2.0";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while(true)
            {
                Console.CursorVisible = false;
                MainMenu();

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                    {
                        option--;
                        if (option < 1)
                        {
                            option = 1;
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                    {
                        option++;
                        if (option > 3)
                        {
                            option = 3;
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        Thread.Sleep(250);
                        Console.SetWindowSize(86, 31);
                        Console.SetBufferSize(86, 31);
                        if (option == 1)
                        {
                            MoveWindow(300, 50);
                            OriginalMode normalMode = new OriginalMode();
                            normalMode.Run();
                        }
                        else if (option == 2)
                        {
                            MoveWindow(300, 50);
                            SurvivalMode survivalMdoe = new SurvivalMode();
                            survivalMdoe.Run();
                        }
                        else if (option == 3)
                        {                            
                            MoveWindow(300, 50);
                            TimedMode timedMode = new TimedMode();
                            timedMode.Run();
                        }
                    }
                }

            }

        }

        

        public static void GameOver(int flag)
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(intercept: true);
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(5, 10);
            Console.WriteLine(" ██████╗  █████╗ ███╗   ███╗███████╗   █████╗ ██╗   ██╗███████╗██████╗ ██╗");
            Console.SetCursorPosition(5, 11);
            Console.WriteLine("██╔════╝ ██╔══██╗████╗ ████║██╔════╝  ██╔══██╗██║   ██║██╔════╝██╔══██╗██║");
            Console.SetCursorPosition(5, 12);
            Console.WriteLine("██║  ██╗ ███████║██╔████╔██║█████╗    ██║  ██║╚██╗ ██╔╝█████╗  ██████╔╝██║");
            Console.SetCursorPosition(5, 13);
            Console.WriteLine("██║  ╚██╗██╔══██║██║╚██╔╝██║██╔══╝    ██║  ██║ ╚████╔╝ ██╔══╝  ██╔══██╗╚═╝");
            Console.SetCursorPosition(5, 14);
            Console.WriteLine("╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗  ╚█████╔╝  ╚██╔╝  ███████╗██║  ██║██╗");
            Console.SetCursorPosition(5, 15);
            Console.WriteLine(" ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝   ╚════╝    ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝");
            if (flag == 1)
            {
                Console.SetCursorPosition(30, 18);
                Console.WriteLine("PRESS ANY KEY TO SEE RESULT");
            }
            Console.ReadKey();
            Console.Clear();
            
        }
        public static void LoadingScreen()
        {
            static void drawDescription()
            {
                if (option == 1)
                {
                    Console.SetCursorPosition(16, 10);
                    Console.WriteLine("You'r mission is to get every point that is on the screen");
                    Console.SetCursorPosition(18, 11);
                    Console.WriteLine(" as fast as you can and get a hight amount of score.");
                    Console.SetCursorPosition(35, 12);
                    Console.WriteLine(" ♦ SPEED IS KEY ♦ ");
                }
                else if (option == 2)
                {
                    Console.SetCursorPosition(16, 10);
                    Console.WriteLine("You'r mission is to get every point that is on the screen,");
                    Console.SetCursorPosition(16, 11);
                    Console.WriteLine("     before the timer runs out. Watch out for mines!!");
                    Console.SetCursorPosition(33, 12);
                    Console.WriteLine(" ♦ PRECISION IS KEY ♦ ");
                }
                else if (option == 3)
                {
                    Console.SetCursorPosition(11, 10);
                    Console.WriteLine("   Use the WASD and the Arrows to move around the screen, and to");
                    Console.SetCursorPosition(11, 11);
                    Console.WriteLine("get every point that is on the screen. Watch out for moving mines!!");
                    Console.SetCursorPosition(34, 12);
                    Console.WriteLine(" ♦ TIMING IS KEY ♦ ");
                }
            }
            static void DotDotDotAnimation(int delay, string word)
            {
                
                string dotdotdot = string.Empty;

                for (int i = 0; i < 3; i++)
                {
                    dotdotdot += '.';
                    int space = word == "Loading" ? (40 + word.Length) : (40 - (word.Length / 3) + word.Length);
                    Console.SetCursorPosition(space, 15);
                    Console.WriteLine(dotdotdot);
                    Thread.Sleep(delay / 3);
                }


                Console.SetCursorPosition(38 + word.Length, 15);
                Console.WriteLine("   ");
            }
         
            Console.Clear();
            drawDescription();
            Console.SetCursorPosition(18, 16);
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.SetCursorPosition(18, 17); 
            Console.WriteLine("║                                                  ║");
            Console.SetCursorPosition(18, 18);
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            int totalProgress = 100;
            int currentProgress = 0;

            List<string> funnyMassages = new List<string>()
            {
                "Definitely not a virus",
                "Alt-F4 speeds things up",
                "Laughing at you, i mean Loading",
                "  Stay Determined",
                "Spinning up the hamster",
                "Loading new Loading screen",
                "We're testing your patience"
            };

            bool funnyMassageTicker = true;

            while (currentProgress < totalProgress)
            {
                int randomMassage = random.Next(0,101);
                if (randomMassage >= 0 && randomMassage <= 35 && currentProgress >= 15 && funnyMassageTicker)
                {
                    string word = funnyMassages[random.Next(funnyMassages.Count)];
                    if (funnyMassages.Count > 0)
                    {
                        funnyMassages.Remove(word);
                    }
                    funnyMassageTicker = false;
                    Console.SetCursorPosition(40 - (word.Length/3) , 15);
                    Console.Write(word.PadRight(30));
                    int randomDelay = random.Next(550, 1000);
                    DotDotDotAnimation(randomDelay, word);
                }               
                else
                {
                    funnyMassageTicker = true;
                    Console.SetCursorPosition(40, 15);
                    Console.WriteLine("Loading");
                    int randomDelay = random.Next(450, 750);
                    DotDotDotAnimation(randomDelay, "Loading");
                }

                int maxIncrement = (currentProgress < 90) ? 15 : 5;
                int increment = random.Next(5, maxIncrement + 1);
                currentProgress += increment;

                if (currentProgress > totalProgress)
                    currentProgress = totalProgress;

                Console.SetCursorPosition(19, 17);

                string progressBar = new string('█', currentProgress /2).PadRight(40, ' ');
                Console.Write(progressBar);

                Console.SetCursorPosition(72, 17);
                Console.Write($"{currentProgress}%");

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(intercept: true);
                }

                Console.SetCursorPosition(18, 15);
                Console.WriteLine("                                                          ");
            }


            // Final screen
            Console.Clear();
            drawDescription();
            Console.SetCursorPosition(33, 15);
            Console.WriteLine("PRESS ANY KEY TO START");
            Console.ReadKey();
        }
        private static void MainMenu()
        {
            Console.SetCursorPosition(13, 4);
            Console.WriteLine("███╗   ███╗ █████╗ ██╗   ██╗██╗███╗  ██╗ ██████╗    ███████╗██╗ ██████╗ ██╗   ██╗██████╗ ███████╗");
            Console.SetCursorPosition(13, 5);
            Console.WriteLine("████╗ ████║██╔══██╗██║   ██║██║████╗ ██║██╔════╝    ██╔════╝██║██╔════╝ ██║   ██║██╔══██╗██╔════╝");
            Console.SetCursorPosition(13, 6);
            Console.WriteLine("██╔████╔██║██║  ██║╚██╗ ██╔╝██║██╔██╗██║██║  ██╗    █████╗  ██║██║  ██╗ ██║   ██║██████╔╝█████╗");
            Console.SetCursorPosition(13, 7);
            Console.WriteLine("██║╚██╔╝██║██║  ██║ ╚████╔╝ ██║██║╚████║██║  ╚██╗   ██╔══╝  ██║██║  ╚██╗██║   ██║██╔══██╗██╔══╝");
            Console.SetCursorPosition(13, 8);
            Console.WriteLine("██║ ╚═╝ ██║╚█████╔╝  ╚██╔╝  ██║██║ ╚███║╚██████╔╝   ██║     ██║╚██████╔╝╚██████╔╝██║  ██║███████╗");
            Console.SetCursorPosition(13, 9);
            Console.WriteLine("╚═╝     ╚═╝ ╚════╝    ╚═╝   ╚═╝╚═╝  ╚══╝ ╚═════╝    ╚═╝     ╚═╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝╚══════╝ v2.0");

            if (option == 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }
            
            Console.SetCursorPosition(47, 13);
            Console.WriteLine("█▀▀█ █▀▀█  ▀  █▀▀▀  ▀  █▀▀▄ █▀▀█ █");
            Console.SetCursorPosition(47, 14);
            Console.WriteLine("█  █ █▄▄▀ ▀█▀ █ ▀█ ▀█▀ █  █ █▄▄█ █");
            Console.SetCursorPosition(47, 15);
            Console.WriteLine("▀▀▀▀ ▀ ▀▀ ▀▀▀ ▀▀▀▀ ▀▀▀ ▀  ▀ ▀  ▀ ▀▀▀");
            Console.ResetColor();

            if (option == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;             
            }

            Console.SetCursorPosition(46, 19);
            Console.WriteLine("█▀▀ █  █ █▀▀█ ▀█ █▀  ▀  ▀█ █▀ █▀▀█ █ ");
            Console.SetCursorPosition(46, 20);
            Console.WriteLine("▀▀█ █  █ █▄▄▀  █▄█  ▀█▀  █▄█  █▄▄█ █   ");
            Console.SetCursorPosition(46, 21);
            Console.WriteLine("▀▀▀  ▀▀▀ ▀ ▀▀   ▀   ▀▀▀   ▀   ▀  ▀ ▀▀▀");
            Console.ResetColor();

            if (option == 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;             
            }

            Console.SetCursorPosition(52, 25);
            Console.WriteLine("▀▀█▀▀  ▀  █▀▄▀█ █▀▀ █▀▀▄");
            Console.SetCursorPosition(52, 26);
            Console.WriteLine("  █   ▀█▀ █ ▀ █ █▀▀ █  █");
            Console.SetCursorPosition(52, 27);
            Console.WriteLine("  ▀   ▀▀▀ ▀   ▀ ▀▀▀ ▀▀▀ ");
            Console.ResetColor();
        }

       
    }
}