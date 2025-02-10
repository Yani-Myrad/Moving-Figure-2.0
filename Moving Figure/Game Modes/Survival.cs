using System.Media;
using System;
using MovingFigure;
using System.Timers;
using System.Drawing;
using System.Reflection;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;
using System.Reflection.Emit;
using Microsoft.VisualBasic.FileIO;

namespace Survival
{
    class Point
    {
        public int x { get; set; }
        public int y { get; set; }
        private int area { get; set; }
        private int minesAmount { get; set; }
        private int[,] minesPosition { get; set; }

        public List<int[]> possibleMinesPositions { get; set; }
        public List<int[]> possiblePointPositions { get; set; }

        public Point(int minesAmount)
        {

            this.area = Program.random.Next(1,5);
            this.minesAmount = minesAmount;
            this.possibleMinesPositions = new List<int[]>();
            this.possiblePointPositions = new List<int[]>();

            // Init the point position
            for (int x = 0; x < SurvivalMode.width; x++)
            {
                for (int y = 0; y < SurvivalMode.height; y++)
                {
                    if (SurvivalMode.grid[x, y] == '*')
                    {
                        this.possiblePointPositions.Add(new int[2] { x, y });
                    }
                }
            }
            int[] currentPosition = this.possiblePointPositions[Program.random.Next(this.possiblePointPositions.Count)];
            this.x = currentPosition[0];
            this.y = currentPosition[1];
            SurvivalMode.grid[this.x, this.y] = 'P';

            // Area Init   
            if (this.area == 1)
            {
                int startPositionX = this.x;
                int startPositionY = this.y;
                int width = Program.random.Next(1,3);
                int height = Program.random.Next(1,3);
               
                
                for (int y = startPositionY - height; y <= startPositionY + height; y++)
                {
                    for (int x = startPositionX - width; x <= startPositionX + width; x++)
                    {
                        if ((y >= 0 && y < SurvivalMode.height) && (x >= 0 && x < SurvivalMode.width))
                        {
                            minesPositionInit(x, y);

                        }
                    }
                }
            }
            else if (this.area == 2)
            {
                int width = Program.random.Next(1,3);
                int height = Program.random.Next(1,3);
               
                
                for (int y = this.y - height; y < this.y + height; y++)
                {
                    for (int x = this.x - width; x < this.x + width; x++)
                    {
                        if ((y >= 0 && y < SurvivalMode.height) && (x >= 0 && x < SurvivalMode.width))
                        {
                            if (y == this.y)
                            {
                                minesPositionInit(x, y);
                            }
                            else
                            {
                                minesPositionInit(this.x, y);
                                break;
                            }
                        }
                    }
                }
            }
            else if (this.area == 3)
            {
                int gridSize = Program.random.Next(1,3);


                for (int i = 1; i <= gridSize; i++)
                {
                    if (this.x + i >= 0 && this.x + i < SurvivalMode.width && this.y - i >= 0 && this.y - i < SurvivalMode.height)
                    {
                        minesPositionInit(this.x + i, this.y - i);
                    }
                    if (this.x + i >= 0 && this.x + i < SurvivalMode.width && this.y + i >= 0 && this.y + i < SurvivalMode.height)
                    {
                        minesPositionInit(this.x + i, this.y + i);
                    }
                    if (this.x - i >= 0 && this.x - i < SurvivalMode.width && this.y + i >= 0 && this.y + i < SurvivalMode.height)
                    {
                        minesPositionInit(this.x - i, this.y + i);
                    }
                    if (this.x - i >= 0 && this.x - i < SurvivalMode.width && this.y - i >= 0 && this.y - i < SurvivalMode.height)
                    {
                        minesPositionInit(this.x - i, this.y - i);
                    }
                }
            }
            else if (this.area == 4)
            {
                int x = 2;
                int y = 1;

                if (this.x + x >= 0 && this.x + x < SurvivalMode.width && this.y - y >= 0 && this.y - y < SurvivalMode.height)
                {
                    minesPositionInit(this.x + x, this.y - y);
                }
                if (this.x + x >= 0 && this.x + x < SurvivalMode.width && this.y + y >= 0 && this.y + y < SurvivalMode.height)
                {
                    minesPositionInit(this.x + x, this.y + y);
                }
                if (this.x - x >= 0 && this.x - x < SurvivalMode.width && this.y + y >= 0 && this.y + y < SurvivalMode.height)
                {
                    minesPositionInit(this.x - x, this.y + y);
                }
                if (this.x - x >= 0 && this.x - x < SurvivalMode.width && this.y - y >= 0 && this.y - y < SurvivalMode.height)
                {
                    minesPositionInit(this.x - x, this.y - y);
                }
            }


            // Seting mines positions
            this.minesAmount = (this.possibleMinesPositions.Count == 0) ? this.minesAmount = 0 : this.minesAmount;
            this.minesAmount = (this.minesAmount > this.possibleMinesPositions.Count) ? this.minesAmount -= (this.minesAmount - this.possibleMinesPositions.Count) : this.minesAmount;
            this.minesPosition = new int[this.minesAmount, 2];
            for (int i = 0; i < this.minesAmount; i++)
            {
                int[] currentMinePosition = this.possibleMinesPositions[Program.random.Next(this.possibleMinesPositions.Count)];
                this.possibleMinesPositions.Remove(currentMinePosition);
                this.minesPosition[i, 0] = currentMinePosition[0];
                this.minesPosition[i, 1] = currentMinePosition[1];
                SurvivalMode.grid[this.minesPosition[i, 0], this.minesPosition[i, 1]] = 'M';

            }
        }

        private void minesPositionInit(int x, int y)
        {
            bool posible = false;

            for (int i = 0; i < minesAmount; i++) // for every mine
            {
                if (SurvivalMode.grid[x, y] == '*')
                {
                    posible = true;
                }
            }

            if (posible)
            {
                this.possibleMinesPositions.Add(new int[2] { x, y });
                SurvivalMode.possibleMinesPositionsSum++;
            }
        }


    }
    class Level
    {
        public int amountOfPoints { get; set; }
        public int amounyOfTransitions { get; set; }
        public int amountOfSteps { get; set; }
        public double score { get; set; }
        public int consecutivePointsScore { get; set; }


        public Level(int P, int T, int c, int S, int s)
        {
            this.amountOfPoints = P;
            this.amounyOfTransitions = T;
            this.consecutivePointsScore = c;
            this.amountOfSteps = S;
            this.score = s;
        }
    }
    class SurvivalMode
    {
        static public int Xdefault = 13;
        static public int Ydefault = 4;
        static public int seconds = 0;
        static public int possibleMinesPositionsSum = 0;
        static int consecutivePointsTaken = 0;
        static bool debugerMode = false;
        //Grid Properties
        static public int width = 26;
        static public int height = 9;
        static public char[,] grid = new char[width, height];
        static int boardXOffset = 33;
        static int boardYOffset = 8;
        static bool TimerRunning = true;
        static string userInput = string.Empty;
        static bool isAlive = true;
        static int levelId = 1;
        static List<Level> levels = new List<Level>();

        public void Run()
        {
            Program.LoadingScreen();
            Console.Clear();
        newLevel:
            Console.Clear();
            Xdefault = 13;
            Ydefault = 4;

            Level currentLevel = new Level(0,0,0,0,200);
            seconds = levelId switch
            {
                >= 1 and < 30 => 25,
                >= 30 and <= 50 => 30,
                >= 50 => 35
            };

            consecutivePointsTaken = 0;

        CancellationTokenSource cts = new CancellationTokenSource();
            Task timerTask = DisplayTimerAsync(cts.Token);

            //Grid Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = '*';
                }
            }
            grid[Xdefault, Ydefault] = 'S'; 
            
            int MinesFiguresAmount = levelId switch
            {
                >= 1 and <= 2 => 5,
                3 => 6,
                >= 4 and <= 5 => 7,
                6 => 8,
                >= 7 and <= 8 => 9,
                >= 9 and <= 10 => 10,
                11 => 11,
                >= 12 and <= 13 => 12,
                >= 14 and <= 15 => 13,
                >= 16 and <= 17 => 14,
                18 => 15,
                19 => 16,
                >= 20 and <= 21 => 17,
                22 => 18,
                23 => 19,
                >= 24 and <= 25 => 20,
                >= 26 and <= 27 => 21,
                28 => 22,
                >= 29 and <= 30 => 23,
                >= 31 and <= 32 => 24,
                33 => 25,
                34 => 26,
                >= 35 and <= 36 => 27,
                37 => 28,
                >= 38 and <= 39 => 29,
                40 => 30,
                >= 41 and <= 42 => 31,
                43 => 32,
                44 => 33,
                >= 45 and <= 46 => 34,
                47 => 35,
                >= 48 and <= 49 => 36,
                50 => 37
            };

            int PointsFiguresAmount = levelId switch
            {
                >= 1 and <= 2 => 3,
                3 => 5,
                >= 4 and <= 5 => 6,
                6 => 7,
                >= 7 and <= 8 => 9,
                >= 9 and <= 10 => 10,
                11 => 11,
                >= 12 and <= 13 => 13,
                >= 14 and <= 15 => 14,
                >= 16 and <= 17 => 16,
                18 => 17,
                19 => 18,
                >= 20 and <= 21 => 20,
                22 => 21,
                23 => 22,
                >= 24 and <= 25 => 24,
                >= 26 and <= 27 => 26,
                28 => 28,
                >= 29 and <= 30 => 30,
                >= 31 and <= 32 => 32,
                33 => 33,
                34 => 34,
                >= 35 and <= 36 => 36,
                37 => 37,
                >= 38 and <= 39 => 39,
                40 => 40,
                >= 41 and <= 42 => 42,
                43 => 43,
                44 => 44,
                >= 45 and <= 46 => 46,
                47 => 47,
                >= 48 and <= 49 => 49,
                50 => 50
            };

            // Seperating the mines by the points
            int[] minesSeperatedPosition = new int[PointsFiguresAmount];
            for (int i = 0; i < MinesFiguresAmount; i++)
            {
                minesSeperatedPosition[i % minesSeperatedPosition.Length]++;
            }

            //Init the points
            List<Point> points = new List<Point>();
            for (int i = 0; i < PointsFiguresAmount; i++)
            {
                Point point = new Point(minesSeperatedPosition[i]);
                points.Add(point);
            }

            while (isAlive) 
            {          
                // Board
                if (true)
                {
                    //Game Menu
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(boardXOffset - 1, 7);
                    Console.WriteLine(" __________________________");
                    Console.SetCursorPosition(boardXOffset - 1, 8);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 9);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 10);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 11);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 12);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 13);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 14);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 15);
                    Console.WriteLine("│                          │");
                    Console.SetCursorPosition(boardXOffset - 1, 16);
                    Console.WriteLine("│__________________________│");
                    Console.WriteLine();
                    Console.Write("                                        ");
                    Console.SetCursorPosition(boardXOffset - 1, 17);
                    Console.WriteLine($"│ Steps: {currentLevel.amountOfSteps}");
                    Console.SetCursorPosition((boardXOffset + 13) - 1, 17);
                    Console.WriteLine($"│ Level: {levelId}");
                    Console.SetCursorPosition((boardXOffset + 27) - 1, 17);
                    Console.WriteLine("│");
                    Console.SetCursorPosition(boardXOffset - 1, 18);
                    Console.WriteLine("│            │             │");
                    Console.SetCursorPosition(boardXOffset - 1, 19);
                    Console.WriteLine($"│ Transis: {currentLevel.amounyOfTransitions}");
                    Console.SetCursorPosition((boardXOffset + 13) - 1, 19);
                    Console.WriteLine("│");
                    Console.SetCursorPosition((boardXOffset + 27) - 1, 19);
                    Console.WriteLine("│");
                    Console.SetCursorPosition(boardXOffset - 1, 20);
                    Console.WriteLine("│");
                    Console.SetCursorPosition((boardXOffset + 13) - 1, 20);
                    Console.WriteLine($"│");
                    Console.SetCursorPosition((boardXOffset + 27) - 1, 20);
                    Console.WriteLine($"│");
                    Console.SetCursorPosition(boardXOffset - 1, 21);
                    Console.WriteLine("└────────────┴─────────────┘");

                    Console.SetCursorPosition(38, 25);
                    Console.Write("Command: ");
                }

                //Patern drawing
                if (debugerMode)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        for (int k = 0; k < points[i].possibleMinesPositions.Count; k++)
                        {
                            Console.SetCursorPosition(points[i].possibleMinesPositions[k][0] + boardXOffset, points[i].possibleMinesPositions[k][1] + boardYOffset);
                            Console.WriteLine("*");
                        }
                    }
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
                            Console.WriteLine(grid[x, y]);
                            Console.ResetColor();
                        }
                        else if (grid[x, y] == 'S')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.SetCursorPosition(Xdefault + boardXOffset, Ydefault + boardYOffset);
                            Console.WriteLine(grid[x, y]);
                            Console.ResetColor();
                        }
                        else if (grid[x, y] == 'M' && debugerMode)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.SetCursorPosition(x + boardXOffset, y + boardYOffset);
                            Console.WriteLine(grid[x, y]);
                            Console.ResetColor();
                        }
                    } 
                }

                // Check if mine is hit
                if (grid[Xdefault, Ydefault] == 'M')
                {
                    TimerRunning = false;
                    isAlive = false;


                    SoundPlayer explosion = new SoundPlayer(@$"soundEffects\explosion{Program.random.Next(1,4)}.wav");                                   
                    explosion.Play();
                    for (int frame = 1; frame <= 5; frame++)
                    {
                        DisplayExplosion(Xdefault + boardXOffset, Ydefault + boardYOffset, frame, Program.random);
                        Console.ResetColor();
                        Thread.Sleep(45);
                    }
                    Thread.Sleep(2500);

                }

                //Movement input & Big explosion
                while (isAlive)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true);

                        if (key.Key == ConsoleKey.Enter)
                        {
                            for (int i = 0; i < userInput.Length; i++)
                            {
                                if (userInput[i] == 'w' || userInput[i] == 'W')
                                {
                                    Ydefault--;
                                    currentLevel.amountOfSteps++;
                                }
                                else if (userInput[i] == 'd' || userInput[i] == 'D')
                                {
                                    Xdefault++;
                                    currentLevel.amountOfSteps++;
                                }
                                else if (userInput[i] == 'a' || userInput[i] == 'A')
                                {
                                    Xdefault--;
                                    currentLevel.amountOfSteps++;
                                }
                                else if (userInput[i] == 's' || userInput[i] == 'S')
                                {
                                    Ydefault++;
                                    currentLevel.amountOfSteps++;
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


                            currentLevel.consecutivePointsScore = (consecutivePointsTaken == 2 || consecutivePointsTaken == 3) ? currentLevel.consecutivePointsScore += 20 :
                                                     (consecutivePointsTaken == 4 || consecutivePointsTaken == 5) ? currentLevel.consecutivePointsScore += 50 :
                                                     (consecutivePointsTaken == 6) ? currentLevel.consecutivePointsScore += 75 :
                                                     (consecutivePointsTaken == 7) ? currentLevel.consecutivePointsScore += 100 : currentLevel.consecutivePointsScore += 0;


                            if (userInput != string.Empty)
                            {
                                currentLevel.amounyOfTransitions++;
                            }
                            userInput = string.Empty;
                            Console.SetCursorPosition(46, 25);
                            Console.WriteLine(userInput.PadRight(30));
                            break;
                        }
                        else if (key.Key == ConsoleKey.Backspace && userInput.Length > 0)
                        {
                            userInput = userInput.Substring(0, userInput.Length - 1);
                        }
                        else if (!char.IsControl(key.KeyChar) && userInput.Length < 30)
                        {
                            userInput += key.KeyChar;
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.SetCursorPosition(46, 25);
                        Console.WriteLine(userInput.PadRight(30));
                    }

                    //Check if time is bellow 0
                    if (seconds < 0)
                    {
                        TimerRunning = false;
                        isAlive = false;
                        SoundPlayer explosion = new SoundPlayer(@"soundEffects\bigExplosion.wav");
                        explosion.Play();
                        for (int frame = 1; frame <= 20; frame++)
                        {
                            DisplayExplosion(Xdefault + boardXOffset, Ydefault + boardYOffset, frame, Program.random);
                            Console.ResetColor();
                        }
                        Thread.Sleep(5);
                        for (int frame = 1; frame <= 25; frame++) 
                        {
                            DisplayExplosion(Xdefault + boardXOffset, Ydefault + boardYOffset, frame, Program.random);
                            Console.ResetColor();
                        }
                        Thread.Sleep(5);
                        for (int frame = 1; frame <= 30; frame++)
                        {
                            DisplayExplosion(Xdefault + boardXOffset, Ydefault + boardYOffset, frame, Program.random);
                            Console.ResetColor();

                        }
                        Thread.Sleep(5000);
                    }
                }

                // Check if a wall is hit
                if (!(Xdefault >= 0 && Xdefault < width && Ydefault >= 0 && Ydefault < height))
                {
                    TimerRunning = false;
                    isAlive = false;
                    break;
                }
       
                // Check if point is hit
                for (int i = 0; i < points.Count; i++)
                {
                    if (Xdefault == points[i].x && Ydefault == points[i].y)
                    {
                        SoundPlayer pointTaken = new SoundPlayer(@$"soundEffects\point{consecutivePointsTaken}.wav");
                        pointTaken.Play();
                        currentLevel.amountOfPoints += 1;
                        grid[points[i].x, points[i].y] = '*';
                        points.Remove(points[i]);
                    }
                }

                // Check if you got all points
                if (currentLevel.amountOfPoints == PointsFiguresAmount || !doesGridContainsPoints())
                {
                    levelId++;
                    levels.Add(currentLevel);
                    cts.Cancel();
                    break;
                } 
            }

            if (isAlive)
            {
                Console.Clear();
                goto newLevel;
            }
            else
            {
                if (levelId <= 1)
                {
                    Program.GameOver(0);
                    Environment.Exit(0);
                }
                else
                {
                    Program.GameOver(1);
                }
                Console.Clear();
                if (levels.Count >= 1)
                {
                    SurvivalModeFinalScore(levels);
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
                    if (grid[x, y] == 'P')
                    {
                        contains = true;
                    }
                }
            }

            return contains;
        }
        private static void DisplayExplosion(int centerX, int centerY, int frame, Random random)
        {
            char[] particles = { ',', '*', '.' };
            ConsoleColor[] colors = { ConsoleColor.DarkRed, ConsoleColor.Yellow, ConsoleColor.DarkYellow };


            for (int i = 0; i < 360; i += 20)
            {
                double angle = (i + random.Next(0, 360)) * Math.PI / 180.0;
                int distance = random.Next(1, frame * 2);
                int x = centerX + (int)(Math.Cos(angle) * distance);
                int y = centerY + (int)(Math.Sin(angle) * distance / 2);

                if (x >= 0 && x < Console.WindowWidth && y >= 0 && y < Console.WindowHeight)
                {
                    Console.SetCursorPosition(x, y);
                    Console.ForegroundColor = colors[random.Next(colors.Length)];
                    Console.Write(particles[random.Next(0, particles.Length)]);
                }
            }


        }
        private static async Task DisplayTimerAsync(CancellationToken token)
        {
            while (TimerRunning)
            {
                token.ThrowIfCancellationRequested();

                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
                string timerText = $"Time: {timeSpan:mm\\:ss}";

                Console.SetCursorPosition(47, 19);
                switch (seconds)
                {
                    case 1:
                        SoundPlayer tick1 = new SoundPlayer(@"soundEffects\tick1.wav");
                        tick1.Play();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;

                    case 2:
                        SoundPlayer tick2 = new SoundPlayer(@"soundEffects\tick2.wav");
                        tick2.Play();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;

                    case 3:
                        SoundPlayer tick3 = new SoundPlayer(@"soundEffects\tick3.wav");
                        tick3.Play();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                if (Console.CursorLeft == 47 && Console.CursorTop == 19)
                {
                    Console.Write(timerText);
                }
                Console.ResetColor();

                seconds--;
                await Task.Delay(1000);
            }
        }
        private static void SurvivalModeFinalScore(List<Level> levels)
        {
            int totalScore = 0;

            for (int i = 0; i < levels.Count; i++)
            {
                levels[i].score *= i + 1;
                double v = levels[i].amountOfPoints / (levels[i].amounyOfTransitions / 1.5);
                levels[i].score *= v;
                levels[i].score -= (levels[i].amountOfSteps / 2.5) * levels[i].amountOfPoints ;
                levels[i].score += levels[i].consecutivePointsScore; 
                int score = (int)levels[i].score;
                totalScore += score;
            }

            int n = 1;
            while (true)
            {

                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                    {
                        n++;
                        if (n > levels.Count)
                        {
                            n = levels.Count;
                        }
                        else
                        {                          
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition(57, 11);
                            Console.WriteLine(" ___");
                            Console.SetCursorPosition(57, 12);
                            Console.WriteLine("│ ► │");
                            Console.SetCursorPosition(57, 13);
                            Console.WriteLine(" ‾‾‾");
                            Thread.Sleep(50);
                            Console.SetCursorPosition(48, 12);
                            Console.WriteLine(" ");
                            Console.SetCursorPosition(49, 14);
                            Console.WriteLine("     ");
                            Console.SetCursorPosition(48, 16);
                            Console.WriteLine("      ");
                            Console.SetCursorPosition(48, 18);
                            Console.WriteLine("      ");
                            Console.ResetColor();
                        }

                    }
                    else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                    {
                        n--;
                        
                        if (n < 1)
                        {
                            n = 1;
                        }
                        else
                        {                   
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition(29, 11);
                            Console.WriteLine(" ___");
                            Console.SetCursorPosition(29, 12);
                            Console.WriteLine("│ ◄ │");
                            Console.SetCursorPosition(29, 13);
                            Console.WriteLine(" ‾‾‾");
                            Thread.Sleep(50);
                            Console.SetCursorPosition(48, 12);
                            Console.WriteLine(" ");
                            Console.SetCursorPosition(49, 14);
                            Console.WriteLine("     ");
                            Console.SetCursorPosition(48, 16);
                            Console.WriteLine("      ");
                            Console.SetCursorPosition(48, 18);
                            Console.WriteLine("      ");
                            Console.ResetColor();
                        }
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }

                if (n + 1 <= levels.Count)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;   
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.SetCursorPosition(57, 11);
                Console.WriteLine(" ___");
                Console.SetCursorPosition(57, 12);
                Console.WriteLine("│ ► │");
                Console.SetCursorPosition(57, 13);
                Console.WriteLine(" ‾‾‾");
                Console.ResetColor();

                if (n - 1 >= 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                
                Console.SetCursorPosition(29, 11);
                Console.WriteLine(" ___");
                Console.SetCursorPosition(29, 12);
                Console.WriteLine("│ ◄ │");
                Console.SetCursorPosition(29, 13);
                Console.WriteLine(" ‾‾‾");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(34, 8);
                Console.WriteLine(" █▀█ █▀▀ █▀ █ █ █   ▀█▀");
                Console.SetCursorPosition(34, 9);
                Console.WriteLine(" █▀▄ ██▄ ▄█ █▄█ █▄▄  █ ");
                Console.SetCursorPosition(35, 11);
                Console.WriteLine("┌───────────────────┐");

                Console.SetCursorPosition(35, 12);
                Console.WriteLine($"│      LEVEL {n}      │");
                Console.SetCursorPosition(35, 13);
                Console.WriteLine($"├───────────────────┤");               
                Console.SetCursorPosition(35, 14);
                Console.WriteLine($"│    TRANSIS: {levels[n-1].amounyOfTransitions:f0}");
                Console.SetCursorPosition(55, 14);
                Console.WriteLine("│");
                Console.SetCursorPosition(35, 15);
                Console.WriteLine("│                   │");
                
                Console.SetCursorPosition(35, 16);
                Console.WriteLine($"│     STEPS: {levels[n-1].amountOfSteps:f0}");
                Console.SetCursorPosition(55, 16);
                Console.WriteLine("│");
                Console.SetCursorPosition(35, 17);
                Console.WriteLine("│                   │");
                
                Console.SetCursorPosition(35, 18);
                Console.WriteLine($"│     SCORE: {(int)levels[n - 1].score}");
                Console.SetCursorPosition(55, 18);
                Console.WriteLine("│");
                Console.SetCursorPosition(35, 19);
                Console.WriteLine("└───────────────────┘");

                Console.SetCursorPosition(30, 20);
                Console.WriteLine("╔════════════════════════════╗");

                Console.SetCursorPosition(30, 21);
                Console.WriteLine($"║   LEVEL: {levels.Count:f0}");
                Console.SetCursorPosition(45, 21);
                Console.WriteLine($"SCORE: {totalScore}");
                Console.SetCursorPosition(59, 21);
                Console.WriteLine("║");
                Console.SetCursorPosition(30, 22);
                Console.WriteLine("╚════════════════════════════╝");
                Console.SetCursorPosition(36, 24);
                Console.WriteLine("PRESS ESCAPE TO EXIT");
            }
            
        }


    }
}