using System.Timers;
using MovingFigure;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using Survival;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using System.Media;

namespace Original
{
    class OriginalMode 
    {
        private static int seconds = 0;
        private static bool TimerRunning = true;
        private static double AmountOfPoints = 0;
        private static int AmountOfStepsMade = 0;
        private static string userInput = string.Empty;
        private static int width = 49;
        private static int height = 12;
        private static int Xdefault = width / 2;
        private static int Ydefault = (height / 2) - 1;
        private static char[,] grid = new char[width, height];
        private static int boardXOffset = 21;
        private static int boardYOffset = 6;
        private static int consecutivePointsTaken = 0;
        private static int consecutivePointsScore = 0;
        private static bool isAlive = true;
        private static List<int[]> possiblePointPositions = new List<int[]>();
        private static int PointsFiguresAmount = Program.random.Next(15, 20);
        private static List<int[]> points = new List<int[]>();


        public void Run()
        {
            Program.LoadingScreen();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start BackgroundApp.exe 1",
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);
            Console.Clear();
            Task timerTask = DisplayTimerAsync();
            
            //Grid Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = '*';
                }
            }
            grid[Xdefault, Ydefault] = 'S';

            // Init the point position
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == '*')
                    {
                        possiblePointPositions.Add(new int[2] { x, y });
                    }
                }
            }
            for (int i = 0; i < PointsFiguresAmount; i++)
            {
                int[] currentPosition = possiblePointPositions[Program.random.Next(possiblePointPositions.Count)];
                grid[currentPosition[0], currentPosition[1]] = 'P';
                points.Add(new int[2] { currentPosition[0], currentPosition[1] });
            }
            

            // Game Loop
            while (isAlive)
            {
                // Board
                {
                    Console.ForegroundColor = ConsoleColor.White;               
                    Console.SetCursorPosition(boardXOffset -1, 5);
                    Console.Write(" _________________________________________________");
                    Console.SetCursorPosition(boardXOffset - 1, 6);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 7);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 8);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 9);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 10);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 11);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 12);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 13);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 14);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 15);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 16);
                    Console.Write("│                                                 │");
                    Console.SetCursorPosition(boardXOffset - 1, 17);
                    Console.Write("│_________________________________________________│");

                    Console.SetCursorPosition(boardXOffset - 1, 18);
                    Console.WriteLine("│░░░░░░░░░░░░╔════════════════╗░░░░░░░░░░░░░░░░░░░│");

                    Console.SetCursorPosition(boardXOffset - 1, 19);
                    Console.WriteLine($"│ W/w - Up   ║");

                    Console.SetCursorPosition(boardXOffset + 29, 19);
                    Console.WriteLine("║w..∞ multiple steps│");

                    Console.SetCursorPosition(boardXOffset - 1, 20);
                    Console.WriteLine("│ S/s - Down ║                ║s..∞ multiple steps│");

                    Console.SetCursorPosition(boardXOffset - 1, 21);
                    Console.WriteLine($"│ A/a - Left ║   Steps: {AmountOfStepsMade}");
                    Console.SetCursorPosition(boardXOffset + 29, 21);
                    Console.WriteLine("║a..∞ multiple steps│");

                    Console.SetCursorPosition(boardXOffset - 1, 22);
                    Console.WriteLine("│ D/d - Right╚════════════════╝d..∞ multiple steps│");

                    Console.SetCursorPosition(boardXOffset - 1, 23);
                    Console.WriteLine("│░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░│");


                    Console.SetCursorPosition(boardXOffset + 17, 25);
                    Console.Write("Command: ");
                }

                // Figures drawing
                 for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (grid[x, y] == 'P')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.SetCursorPosition(x + boardXOffset, y + boardYOffset);
                            Console.Write(grid[x, y]);
                            Console.ResetColor();
                        }
                        else if (grid[x, y] == 'S')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(Xdefault + boardXOffset, Ydefault + boardYOffset);
                            Console.Write(grid[x, y]);
                            Console.ResetColor();
                        }
                        
                        
                    }
                }

                //Movement input
                while (isAlive)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);
                        long targetDelayNanoseconds = 0_750;

                        if (key.Key == ConsoleKey.W || key.Key == ConsoleKey.S)
                        {
                            targetDelayNanoseconds = 100_000;
                        }
                        
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        double ticksPerNanosecond = Stopwatch.Frequency / 1_000_000_000.0;
                        long targetDelayTicks = (long)(targetDelayNanoseconds * ticksPerNanosecond);
                        while (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == key.Key && stopwatch.ElapsedTicks < targetDelayTicks){ /* Nothing (Delay) */ }

                        stopwatch.Stop();

                        if (key.Key == ConsoleKey.Enter)
                        {
                            for (int i = 0; i < userInput.Length; i++)
                            {
                                if (userInput[i] == 'w' || userInput[i] == 'W')
                                {
                                    Ydefault--;
                                    AmountOfStepsMade++;
                                }
                                else if (userInput[i] == 'd' || userInput[i] == 'D')
                                {
                                    Xdefault++;
                                    AmountOfStepsMade++;
                                }
                                else if (userInput[i] == 'a' || userInput[i] == 'A')
                                {
                                    Xdefault--;
                                    AmountOfStepsMade++;
                                }
                                else if (userInput[i] == 's' || userInput[i] == 'S')
                                {
                                    Ydefault++;
                                    AmountOfStepsMade++;
                                }
                            }
                            

                            try
                            {
                                if (grid[Xdefault, Ydefault] == 'P')
                                {
                                    consecutivePointsTaken++;
                                    if (consecutivePointsTaken > 7)
                                    {
                                        consecutivePointsTaken = 7;
                                    }
                                }
                                else
                                {
                                    consecutivePointsTaken = 0;
                                }
                            }
                            catch (Exception) { }

                            consecutivePointsScore = (consecutivePointsTaken == 2 || consecutivePointsTaken == 3) ? consecutivePointsScore += 1 :
                                                     (consecutivePointsTaken == 4 || consecutivePointsTaken == 5) ? consecutivePointsScore += 2 :
                                                     (consecutivePointsTaken == 6) ? consecutivePointsScore += 3 :
                                                     (consecutivePointsTaken == 7) ? consecutivePointsScore += 4 : consecutivePointsScore += 0;

                            userInput = string.Empty;
                            Console.SetCursorPosition(46, 25);
                            Console.WriteLine(userInput.PadRight(30));
                            break;
                        }

                        
                        if (key.Key == ConsoleKey.Backspace && userInput.Length > 0)
                        {
                            userInput = userInput.Substring(0, userInput.Length - 1);
                        }
                        if (!char.IsControl(key.KeyChar) && userInput.Length < 48)
                        {
                            userInput += key.KeyChar;
                        }
                       

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(46, 25);
                        Console.WriteLine(userInput.PadRight(60));
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }

                // Check If a wall is hit
                if (!(Xdefault >= 0 && Xdefault < width && Ydefault >= 0 && Ydefault < height))
                {
                    TimerRunning = false;
                    isAlive = false;
                    Program.GameOver(0);
                    Environment.Exit(0);

                }

                // Check if Point is hit
                for (int i = 0; i < points.Count; i++)
                {
                    if (Xdefault == points[i][0] && Ydefault == points[i][1])
                    {
                        SoundPlayer pointTaken = new SoundPlayer(@$"soundEffects\point{consecutivePointsTaken}.wav");
                        pointTaken.Play();
                        AmountOfPoints += 1;
                        grid[points[i][0], points[i][1]] = '*';                       
                        points.Remove(points[i]);
                    }
                }
      
                //Check if you got all points
                if (AmountOfPoints == PointsFiguresAmount || !doesGridContainsPoints())
                {
                    isAlive = false;
                    TimerRunning = false;                
                    Console.Clear();
                    NormalModeFinalScore(AmountOfPoints, AmountOfStepsMade, consecutivePointsScore);
                    break;
                }
                          
            }

        }
        private static bool doesGridContainsPoints()
        {
            bool contains = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x,y] == 'P')
                    {
                        contains = true;
                    }
                }
            }

            return contains;
        }
        private static void NormalModeFinalScore(double AmountOfPoints, int AmountOfStepsMade, int consecutivePointsScore)
        {
            int minutes = seconds / 60;
            int SumSecounds = seconds % 60;

            double score = 200;
            score -= (AmountOfStepsMade / 2.5);
            score += consecutivePointsScore;

            int timeScore =   (seconds == 1) ? 600 : (seconds == 2) ? 450 : (seconds == 3) ? 375 : (seconds == 4) ? 300 : (seconds == 5) ? 250 
                            :  (seconds == 6) ? 215 : (seconds == 7) ? 195 : (seconds == 8) ? 185 : (seconds == 9) ? 170 : (seconds == 10) ? 150

                            : (seconds == 11) ? 135 : (seconds == 12) ? 120 : (seconds == 13) ? 105 : (seconds == 14) ? 90 : (seconds == 15) ? 80
                            : (seconds == 16) ? 75 : (seconds == 17) ? 69 : (seconds == 18) ? 66 : (seconds == 19) ? 60 : (seconds == 20) ? 55

                            : (seconds == 21) ? 48 : (seconds == 22) ? 42 : (seconds == 23) ? 36 : (seconds == 24) ? 30 : (seconds == 25) ? 26
                            : (seconds == 26) ? 18 : (seconds == 27) ? 14 : (seconds == 28) ? 10 : (seconds == 29) ? 7 : (seconds == 30) ? 5

                            : (seconds == 31) ? 0 : (seconds == 32) ? 0 : (seconds == 33) ? 0 : (seconds == 34) ? 0 : (seconds == 35) ? 0
                            : (seconds == 36) ? -5 : (seconds == 37) ? -10 : (seconds == 38) ? -15 : (seconds == 39) ? -20 : (seconds == 40) ? -30

                            : (seconds == 41) ? -40 : (seconds == 42) ? -50 : (seconds == 43) ? -60 : (seconds == 44) ? -70 : (seconds == 45) ? -80
                            : (seconds == 46) ? -100 : (seconds == 47) ? -120 : (seconds == 48) ? -140 : (seconds == 49) ? 160 : (seconds == 50) ? 180 : -200;

            score += timeScore;





            if (true)
            { 
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(35, 8);
                Console.WriteLine(" █▀█ █▀▀ █▀ █ █ █   ▀█▀");
                Console.SetCursorPosition(35, 9);
                Console.WriteLine(" █▀▄ ██▄ ▄█ █▄█ █▄▄  █ ");
                Console.SetCursorPosition(35, 11);
                Console.WriteLine("╔═════════════════════╗");
                Console.SetCursorPosition(35, 12);
                Console.WriteLine("║                     ║");

                Console.SetCursorPosition(33, 13);
                Console.WriteLine($"  ║     POINTS: {AmountOfPoints}");
                Console.SetCursorPosition(57, 13);
                Console.WriteLine("║");

                Console.SetCursorPosition(35, 14);
                Console.WriteLine("║                     ║");
                Console.SetCursorPosition(35, 15);
                Console.WriteLine("╠═════════════════════╣");
                Console.SetCursorPosition(35, 16);
                Console.WriteLine("║                     ║");

                Console.SetCursorPosition(33, 17);
                Console.WriteLine($"  ║     STEPS: {AmountOfStepsMade}");
                Console.SetCursorPosition(57, 17);
                Console.WriteLine("║");

                Console.SetCursorPosition(35, 18);
                Console.WriteLine("║                     ║");

                if (seconds < 10)
                {
                    Console.SetCursorPosition(33, 19);
                    Console.WriteLine($"  ║     TIME: {minutes}:0{SumSecounds}");
                    Console.SetCursorPosition(57, 19);
                    Console.WriteLine("║");
                }
                else
                {
                    Console.SetCursorPosition(33, 19);
                    Console.WriteLine($"  ║     TIME: {minutes}:{SumSecounds}");
                    Console.SetCursorPosition(57, 19);
                    Console.WriteLine("║");
                }


                Console.SetCursorPosition(35, 20);
                Console.WriteLine("║                     ║");

                Console.SetCursorPosition(35, 21);
                Console.WriteLine("╠═════════════════════╣");

                Console.SetCursorPosition(33, 22);
                Console.WriteLine($"  ║     SCORE: {score:f0}");
                Console.SetCursorPosition(57, 22);
                Console.WriteLine("║");

                Console.SetCursorPosition(35, 23);
                Console.WriteLine("╚═════════════════════╝");
                Console.SetCursorPosition(45, 24);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        private static async Task DisplayTimerAsync()
        {

            while (TimerRunning)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
                string timerText = $"Time: {timeSpan:mm\\:ss}";

                Console.SetCursorPosition(37, 19);
                if (Console.CursorLeft == 37 && Console.CursorTop == 19)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(timerText);
                }

                seconds++;
                await Task.Delay(1000);
            }
        }

    }
} 