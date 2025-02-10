using System;
using System.Collections.Generic;
using System.Media;
using MovingFigure;
using Survival;

namespace Timed
{
    class MovingFigure
    {
        public int x { get; set; }
        public int y { get; set; }
        private int Dx { get; set; }
        private int Dy { get; set; }
        public char figure { get; set; }
        private List<int[]> possiblePositions { get; set; }
        public MovingFigure(char figure)
        {
            this.figure = figure;
            this.possiblePositions = new List<int[]>();
            for (int X = 2; X < TimedMode.width -2; X++)
            {
                for (int Y = 2; Y < TimedMode.height -2; Y++)
                {
                    if (TimedMode.grid[X, Y] == 0)
                    {
                        this.possiblePositions.Add(new int[2] { X, Y });
                    }
                }
            }
            int[] currentPosition = this.possiblePositions[Program.random.Next(this.possiblePositions.Count)];
            this.x = currentPosition[0];
            this.y = currentPosition[1];
            TimedMode.grid[this.x, this.y] = (figure == 'P') ? 7 : (figure == 'M') ? 6 : 6;

            Dx = (Program.random.Next(1, 3) == 1) ? -1 : 1;
            Dy = (Program.random.Next(1, 3) == 1) ? -1 : 1;
        }
        public void Move()
        {
            if (TimedMode.grid[x + Dx, y + Dy] == 0)
            {
                TimedMode.grid[x, y] = 0;
                x += Dx;
                y += Dy;
                TimedMode.grid[this.x, this.y] = (figure == 'P') ? 7 : (figure == 'X') ? 6 : 6;
            }
            else
            {
                Dx *= -1;
                Dy *= -1;
            }         
        }
        public void CheckCollisionWithWall()
        {
            if (TimedMode.grid[x + Dx, y] != 0 && TimedMode.grid[x + Dx, y] != 8)
            {
                Dx *= -1;
            }

            if (TimedMode.grid[x , y + Dy] != 0 && TimedMode.grid[x, y + Dy] != 8)
            {
                Dy *= -1;
            }
        }
        public void CheckCollisionWithItself(MovingFigure other)
        {
            if (x == other.x && y == other.y)
            {
                Dx *= -1;
                Dy *= -1;
                other.Dx *= -1;
                other.Dy *= -1;
            }
        }
    }

    class TimedMode
    {
        static bool isAlive = true;
        static public bool isMoving = false;
        static int boardXOffset = 12;
        static int boardYOffset = 4;
        static public int width = 67;
        static public int height = 21;
        public static int consecutivePointsTaken = 0;
        static public int[,] grid = new int[width, height];
        static public int Xdefault = 33;
        static public int Ydefault = 10;
        static int idWall = 0;
        static int pointsAmount = Program.random.Next(10,15);
        static int pointsTaken = 0;
        static double score = 0;
        static int killersAmount = 3;
        static List<MovingFigure> movingFigures = new List<MovingFigure>();

        public void Run()
        {
            Program.LoadingScreen();
            Console.Clear();

            //Grid Init
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = 0;
                }
            }
            //Level Init
            LevelInit();
            //Point Init,
            for (int i = 0; i < pointsAmount; i++)
            {
                MovingFigure currentPoint = new MovingFigure('P');
                movingFigures.Add(currentPoint);
            }
            //Killer Init
            for (int i = 0; i < killersAmount; i++)
            {
                MovingFigure currentKiller = new MovingFigure('M');
                movingFigures.Add(currentKiller);
            }

            //Game loop
            while (isAlive)
            {        
                // Figures Drawing
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        char charToDraw = ' ';

                        if (grid[x, y] == 8)
                        {
                            charToDraw = 'S';
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                        }
                        else if (grid[x, y] == 7)
                        {
                            charToDraw = 'P';
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                        else if (grid[x, y] == 6)
                        {
                            charToDraw = 'M';
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }
                        else if (grid[x, y] == 3 || grid[x, y] == 1)
                        {
                            charToDraw = '│';
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (grid[x, y] == 2)
                        {
                            charToDraw = '_';
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (grid[x, y] == 4)
                        {
                            charToDraw = '‾';
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (grid[x, y] % 2 == 0 & grid[x, y] >= 10)
                        {
                            charToDraw = '─';
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (grid[x, y] % 2 == 1 & grid[x, y] >= 10)
                        {
                            charToDraw = '│';
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.SetCursorPosition(x + boardXOffset, y + boardYOffset);
                        Console.WriteLine(charToDraw);
                    }
                }

                // Movement Input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    int newX = Xdefault, newY = Ydefault;

                    // WASD movement
                    if (key.Key == ConsoleKey.D)
                    {
                        newX = Xdefault;
                        int bonusPoints = 0;
                        while (newX < width - 1 && (grid[newX + 1, Ydefault] == 0 || grid[newX + 1, Ydefault] == 7))
                        {
                            newX++;
                            isMoving = true;
                            bonusPoints = PlayerCollisionWithFigures(newX + 1, newY, bonusPoints);
                            if (Program.random.Next(1, 8) == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.SetCursorPosition(newX + boardXOffset, newY + boardYOffset);
                                Console.WriteLine("S");
                            }
                        }

                        if (bonusPoints > 0)
                        {
                            score += Math.Abs(Xdefault - newX) / 2;
                            SoundPlayer figureTaken = new SoundPlayer(@$"soundEffects\point{bonusPoints}.wav");
                            figureTaken.Play();
                        }

                        score += (bonusPoints == 1) ? 10 : (bonusPoints == 2) ? 30 : (bonusPoints == 3) ? 60 : (bonusPoints == 4) ? 120 : (bonusPoints == 5) ? 250 : (bonusPoints > 5) ? 300 : 0;

                        if (grid[newX + 1, Ydefault] != 0)
                        {
                            idWall = grid[newX + 1, Ydefault];
                            isMoving = false;
                        }
                    }
                    else if (key.Key == ConsoleKey.A)
                    {
                        newX = Xdefault;
                        int bonusPoints = 0;
                        while (newX > 0 && (grid[newX - 1, Ydefault] == 0 || grid[newX - 1, Ydefault] == 7))
                        {
                            newX--;
                            isMoving = true;
                            bonusPoints = PlayerCollisionWithFigures(newX - 1, newY, bonusPoints);
                            if (Program.random.Next(1, 8) == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.SetCursorPosition(newX + boardXOffset, newY + boardYOffset);
                                Console.WriteLine("S");
                            }
                        }

                        if (bonusPoints > 0)
                        {
                            score += Math.Abs(Xdefault - newX) / 2;
                            SoundPlayer figureTaken = new SoundPlayer(@$"soundEffects\point{bonusPoints}.wav");
                            figureTaken.Play();
                        }

                        score += (bonusPoints == 1) ? 10 : (bonusPoints == 2) ? 30 : (bonusPoints == 3) ? 60 : (bonusPoints == 4) ? 120 : (bonusPoints == 5) ? 250 : (bonusPoints > 5) ? 300 : 0;

                        if (grid[newX - 1, Ydefault] != 0)
                        {
                            idWall = grid[newX - 1, Ydefault];
                            isMoving = false;
                        }
                    }
                    else if (key.Key == ConsoleKey.W)
                    {
                        newY = Ydefault;
                        int bonusPoints = 0;
                        while (newY > 0 && (grid[Xdefault, newY - 1] == 0 || grid[Xdefault, newY - 1] == 7))
                        {
                            newY--;
                            isMoving = true;
                            bonusPoints = PlayerCollisionWithFigures(newX, newY - 1, bonusPoints);
                            if (Program.random.Next(1, 8) == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.SetCursorPosition(newX + boardXOffset, newY + boardYOffset);
                                Console.WriteLine("S");
                            }
                        }

                        if (bonusPoints > 0)
                        {
                            score += Math.Abs(Ydefault - newY);
                            SoundPlayer figureTaken = new SoundPlayer(@$"soundEffects\point{bonusPoints}.wav");
                            figureTaken.Play();
                        }

                        score += (bonusPoints == 1) ? 10 : (bonusPoints == 2) ? 30 : (bonusPoints == 3) ? 60 : (bonusPoints == 4) ? 120 : (bonusPoints == 5) ? 250 : (bonusPoints > 5) ? 300 : 0;

                        if (grid[Xdefault, newY - 1] != 0)
                        {
                            idWall = grid[Xdefault, newY - 1];
                        }
                    }
                    else if (key.Key == ConsoleKey.S)
                    {
                        newY = Ydefault;
                        int bonusPoints = 0;
                        while (newY < height - 1 && (grid[Xdefault, newY + 1] == 0 || grid[Xdefault, newY + 1] == 7))
                        {
                            newY++;
                            isMoving = true;
                            bonusPoints = PlayerCollisionWithFigures(newX, newY + 1, bonusPoints);
                            if (Program.random.Next(1, 8) == 1)
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.SetCursorPosition(newX + boardXOffset, newY + boardYOffset);
                                Console.WriteLine("S");
                            }
                        }
                        if (bonusPoints > 0)
                        {
                            score += Math.Abs(Ydefault - newY);
                            SoundPlayer figureTaken = new SoundPlayer(@$"soundEffects\point{bonusPoints}.wav");
                            figureTaken.Play();
                        }

                        score += (bonusPoints == 1) ? 10 : (bonusPoints == 2) ? 30 : (bonusPoints == 3) ? 60 : (bonusPoints == 4) ? 120 : (bonusPoints == 5) ? 250 : (bonusPoints > 5) ? 300 : 0;

                        if (grid[Xdefault, newY + 1] != 0)
                        {
                            idWall = grid[Xdefault, newY + 1];
                            isMoving = false;
                        }
                    }

                    // Arrow Movement
                    if (key.Key == ConsoleKey.RightArrow && (grid[Xdefault, Ydefault - 1] != 0 || grid[Xdefault, Ydefault + 1] != 0 ||
                        grid[Xdefault + 1, Ydefault + 1] != 0 ||
                        grid[Xdefault + 1, Ydefault - 1] != 0) &&
                        grid[newX + 1, Ydefault] == 0)
                    {
                        newX++;
                        if (grid[newX + 1, Ydefault] != 0)
                        {
                            idWall = grid[newX + 1, Ydefault];
                        }
                    }
                    else if (key.Key == ConsoleKey.LeftArrow && (grid[Xdefault, Ydefault - 1] != 0 || grid[Xdefault, Ydefault + 1] != 0 ||
                        grid[Xdefault - 1, Ydefault - 1] != 0 ||
                        grid[Xdefault - 1, Ydefault + 1] != 0)
                        && grid[newX - 1, Ydefault] == 0)
                    {
                        newX--;
                        if (grid[newX - 1, Ydefault] != 0)
                        {
                            idWall = grid[newX - 1, Ydefault];
                        }
                    }
                    else if (key.Key == ConsoleKey.UpArrow && (grid[Xdefault - 1, Ydefault] != 0 || grid[Xdefault + 1, Ydefault] != 0 ||
                        grid[Xdefault - 1, Ydefault - 1] != 0 ||
                        grid[Xdefault + 1, Ydefault - 1] != 0)
                        && grid[Xdefault, newY - 1] == 0)
                    {
                        newY--;
                        if (grid[Xdefault, newY - 1] != 0)
                        {
                            idWall = grid[Xdefault, newY - 1];
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow && (grid[Xdefault - 1, Ydefault] != 0 || grid[Xdefault + 1, Ydefault] != 0 ||
                        grid[Xdefault + 1, Ydefault + 1] != 0 ||
                        grid[Xdefault - 1, Ydefault + 1] != 0)
                        && grid[Xdefault, newY + 1] == 0)
                    {
                        newY++;
                        if (grid[Xdefault, newY + 1] != 0)
                        {
                            idWall = grid[Xdefault, newY + 1];
                        }
                    }


                    //Seting New Position
                    if (newX != Xdefault || newY != Ydefault)
                    {
                        grid[Xdefault, Ydefault] = 0;
                        Xdefault = newX;
                        Ydefault = newY;
                        grid[Xdefault, Ydefault] = 8;
                    }
                }
                // Moving Figures Behaviours
                foreach (var movingFigure in movingFigures)
                {
                    movingFigure.Move();
                    movingFigure.CheckCollisionWithWall();
                }
                // Figures Collisions With Themselfs
                for (int i = 0; i < movingFigures.Count; i++)
                {                  
                    for (int j = i + 1; j < movingFigures.Count; j++)
                    {
                        movingFigures[i].CheckCollisionWithItself(movingFigures[j]);
                    }
                }
                // Figures Collision With Idle Player
                for (int i = 0; i < movingFigures.Count; i++)
                {
                    if (Xdefault + 1 == movingFigures[i].x && Ydefault == movingFigures[i].y ||
                        Xdefault - 1 == movingFigures[i].x && Ydefault == movingFigures[i].y ||
                        Xdefault == movingFigures[i].x && Ydefault == movingFigures[i].y + 1 ||
                        Xdefault == movingFigures[i].x + 1 && Ydefault == movingFigures[i].y + 1 ||
                        Xdefault == movingFigures[i].x - 1 && Ydefault == movingFigures[i].y - 1 ||
                        Xdefault == movingFigures[i].x + 1 && Ydefault == movingFigures[i].y - 1 ||
                        Xdefault == movingFigures[i].x - 1 && Ydefault == movingFigures[i].y + 1 ||
                        Xdefault == movingFigures[i].x && Ydefault == movingFigures[i].y - 1 && !isMoving)
                    {
                        if (movingFigures[i].figure == 'P')
                        {
                            SoundPlayer figureTaken = new SoundPlayer(@$"soundEffects\point{1}.wav");
                            figureTaken.Play();
                            score += 10;
                            grid[movingFigures[i].x, movingFigures[i].y] = 0;
                            movingFigures.Remove(movingFigures[i]);
                        }
                        else if (movingFigures[i].figure == 'M')
                        {
                            isAlive = false;
                            SoundPlayer explosion = new SoundPlayer(@$"soundEffects\explosion{Program.random.Next(1, 4)}.wav");
                            explosion.Play();
                            for (int frame = 1; frame <= 5; frame++)
                            {
                                DisplayExplosion(Xdefault + boardXOffset, Ydefault + boardYOffset, frame, Program.random);
                                Console.ResetColor();
                                Thread.Sleep(45);
                            }
                            Thread.Sleep(2000);
                        }
                    }
                }

                //Check if you got all points
                if (pointsTaken == pointsAmount || !doesGridContainsPoints())
                {
                    isAlive = false;
                    Console.Clear();
                    NormalModeFinalScore();
                    break;
                }

            }
            if (!isAlive)
            {
                Program.GameOver(0);
                Environment.Exit(0);
            }
        }

        public void LevelInit()
        {
            //Board Init
            for (int i = 1; i < width-1; i++)
            {
                grid[i, 0] = 2;
                grid[i, height - 1] = 4;
            }
            for (int i = 1; i < height-1; i++)
            {
                grid[0, i] = 3;
                grid[width - 1, i] = 1;
            }
            grid[Xdefault, Ydefault] = 8;
            VerticalPlatform(33, 1, 1, 11);
            VerticalPlatform(33, 19, 1, 13);
            HorizontalPlatform(1, 10, 1, 12);
            HorizontalPlatform(65, 10, 1, 16);
            HorizontalPlatform(1, 14, 23, 18);
            HorizontalPlatform(43, 14, 23, 20);
            HorizontalPlatform(1, 6, 23, 22);
            HorizontalPlatform(43, 6, 23, 24);
            Task movingPlatform = MovingPLatform();

            static void HorizontalPlatform(int x, int y, int width, int id)
            {
                for (int i = x; i < width + x; i++)
                {
                    grid[i, y] = id;
                }
            }
            static void VerticalPlatform(int x, int y, int height, int id)
            {
               for (int i = y; i < height + y; i++)
               {
                   grid[x, i] = id;
               }
            }
            static void ClearHorizontalPlatform(int startX, int endX, int y, int id)
            {
                for (int x = startX; x <= endX; x++)
                {
                    grid[x, y] = 0;
                }
            }
            static void ClearVerticalPlatform(int startY, int endY, int x, int id)
            {
                for (int y = startY; y <= endY; y++)
                {
                    grid[x, y] = 0;
                }
            }
            static async Task MovingPLatform()
            {
                int platformState = 1;
                while (true)
                {
                    if (platformState == 1)
                    {
                        ClearVerticalPlatform(8,13, 31, 15);
                        ClearVerticalPlatform(8, 13, 35, 17);

                        if (idWall == 15)
                        {
                            grid[Xdefault, Ydefault] = 0;
                            Xdefault = (Ydefault == 7 || Ydefault == 8) ? 37 : (Ydefault == 9) ? Program.random.Next(34, 37) : (Ydefault == 10) ? 33 :
                                       (Ydefault == 11) ? Program.random.Next(30, 33) : (Ydefault == 12 || Ydefault == 13) ?  29 : 33;
                            Ydefault = 7;                          
                            grid[Xdefault, Ydefault] = 8;
                            idWall = 28;
                        }
                        else if (idWall == 17)
                        {
                            grid[Xdefault, Ydefault] = 0;
                            Xdefault = (Ydefault == 7 || Ydefault == 8) ? 37 : (Ydefault == 9) ? Program.random.Next(34, 37) : (Ydefault == 10) ? 33 :
                                       (Ydefault == 11) ? Program.random.Next(30, 33) : (Ydefault == 12 || Ydefault == 13) ? 29 : 33;
                            Ydefault = 13;
                            grid[Xdefault, Ydefault] = 8;
                            idWall = 26;
                        }
                        HorizontalPlatform(29, 8, 9, 26);
                        HorizontalPlatform(29, 12, 9, 28);

                        platformState = 2;
                    }
                    else if (platformState == 2)
                    {
                        ClearHorizontalPlatform(29,38,8, 26);
                        ClearHorizontalPlatform(29, 38, 12, 28);
                        if (idWall == 26)
                        {
                            grid[Xdefault, Ydefault] = 0;
                            Ydefault = (Xdefault == 37 || Xdefault == 38) ? 12 : (Xdefault >= 34 && Xdefault <= 37) ? 11 : (Xdefault == 33) ? 10 :
                                       (Xdefault >= 30 && Xdefault <= 33) ? 9 : (Xdefault == 28 || Xdefault == 29) ? 8 : 10;
                            Xdefault = 30;
                            grid[Xdefault, Ydefault] = 8;
                            idWall = 15;
                        }
                        else if (idWall == 28)
                        {
                            grid[Xdefault, Ydefault] = 0;
                            Ydefault = (Xdefault == 37 || Xdefault == 38) ? 12 : (Xdefault >= 34 && Xdefault <= 37) ? 11 : (Xdefault == 33) ? 10 :
                                       (Xdefault >= 30 && Xdefault <= 33) ? 9 : (Xdefault == 28 || Xdefault == 29) ? 8 : 10;
                            Xdefault = 36;
                            grid[Xdefault, Ydefault] = 8;
                            idWall = 17;
                        }
                        VerticalPlatform(31, 8, 5, 15);
                        VerticalPlatform(35, 8, 5, 17);
                        platformState = 1;
                    }

                  
                    await Task.Delay(1250);
                }
            }           
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
        public static int PlayerCollisionWithFigures(int newX, int newY, int bonusPoints)
        {
            for (int i = 0; i < movingFigures.Count; i++)
            {
                if (newX == movingFigures[i].x && newY == movingFigures[i].y)
                {
                    if (movingFigures[i].figure == 'P')
                    {                        
                        grid[movingFigures[i].x, movingFigures[i].y] = 0;
                        bonusPoints++;
                        pointsTaken++;
                        movingFigures.Remove(movingFigures[i]);
                    }
                    else if (movingFigures[i].figure == 'M')
                    {
                        isAlive = false;
                        SoundPlayer explosion = new SoundPlayer(@$"soundEffects\explosion{Program.random.Next(1, 4)}.wav");
                        explosion.Play();
                        for (int frame = 1; frame <= 5; frame++)
                        {
                            DisplayExplosion(newX + boardXOffset, newY + boardYOffset, frame, Program.random);
                            Console.ResetColor();
                            Thread.Sleep(45);
                        }
                        Thread.Sleep(2500);
                    }
                }
            }
            
            return bonusPoints;
        }

        private static bool doesGridContainsPoints()
        {
            bool contains = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == 7)
                    {
                        contains = true;
                    }
                }
            }

            return contains;
        }
        private static void NormalModeFinalScore()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(20, 11);
            Console.WriteLine(" █   █  █▀▀▀█  █  █    █   █  █▀▀▀█  █▄  █ █ █");
            Console.SetCursorPosition(20, 12);
            Console.WriteLine(" █▄▄▄█  █   █  █  █    █ █ █  █   █  █ █ █ ▀ ▀");
            Console.SetCursorPosition(20, 13);
            Console.WriteLine("   █    █▄▄▄█  ▀▄▄▀    █▄▀▄█  █▄▄▄█  █  ▀█ ▄ ▄");
            Console.SetCursorPosition(20, 15);
            Console.WriteLine($"\t\t\t\tSCORE:{score}");
            Console.ReadKey();
            Environment.Exit(0);
        }

    }
}
