using System;
using System.Threading;

namespace Queens
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Welcome to QUEENS THE GAME!");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You can choose how many players are gonna play. Enter the number of people: ");
            Console.ResetColor();
            string playerMode = Console.ReadLine();
            int players = PositiveIntegerValidation(playerMode, "Number of players");

            bool isEasy = false;
            bool isSingle = false;
            if (players == 1)
            {
                isSingle = true;
                Console.WriteLine("Choose Easy or Hard mode:");
                string mode = Console.ReadLine();
                while (mode.ToLower() != "easy" && mode.ToLower() != "hard")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The options are Easy or Hard mode. Try again:");
                    Console.ResetColor();
                    mode = Console.ReadLine();
                }
                if (mode.ToLower() == "easy")
                {
                    isEasy = true;
                }
            }


            Console.WriteLine("Let's build the board: ");
            Console.Write("Boarder number of rows: ");
            string rowsInput = Console.ReadLine();
            int rows = PositiveIntegerValidation(rowsInput, "Rows number");
            Console.Write("Boarder number of cols: ");
            string colsInput = Console.ReadLine();
            int cols = PositiveIntegerValidation(colsInput, "Cols number");

            string[,] board = new string[rows, cols];

            PrintBoard(board);

            while (true)
            {
                for (int i = 0; i < Math.Max(2, players); i++)
                {
                    if (isSingle && i == 1)
                    {
                        Console.WriteLine("The computer is thinking...");
                        Thread.Sleep(5000);
                        MakeTurn("Player 2", 2, board, true, isEasy);
                    }
                    else
                    {
                        MakeTurn($"Player {i + 1}", i + 1, board, false, false);
                    }
                    if (!IsTherePossibleMoves(board))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Player {i + 1} wins");
                        Console.ResetColor();

                        return;
                    }
                }
            }
        }
        private static int PositiveIntegerValidation(string input, string name)
        {
            int result = 0;
            while (true)
            {
                try
                {
                    result = int.Parse(input);
                    if ((name == "Rows number" || name == "Cols number") && result < 3)
                    {
                        throw new ArgumentOutOfRangeException($"{name} should be greater than 2: ");
                    }
                    if (result <= 0)
                    {
                        throw new Exception();
                    }
                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(ex.ParamName);
                    Console.ResetColor();
                    input = Console.ReadLine();
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{ name } should be positive whole number: ");
                    Console.ResetColor();
                    input = Console.ReadLine();
                }
                
            }
            return result;
        }

        private static int[] ValidateCoordinates(string playerInput, string[,] board)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            while (true)
            {
                string[] input = playerInput.Replace(" ", "").Split(';');
                if (playerInput.Contains(';') && input.Length == 2)
                {
                    string row = input[0];
                    string col = input[1];
                    int validateRow = PositiveIntegerValidation(row, "Row") - 1;
                    int validateCol = PositiveIntegerValidation(col, "Col") - 1;
                    if (validateRow < rows && validateCol < cols && board[validateRow, validateCol] == null)
                    {
                        return new int[] { validateRow, validateCol };
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Invalid input. Try again: ");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Coordinates should be in the format (row; col). Try again: ");
                    Console.ResetColor();

                }
                playerInput = Console.ReadLine();
            }
        }

        private static void MakeTurn(string playerName, int playerNum, string[,] board, bool autoPlay, bool isEasy)
        {
            int x, y;
            if (autoPlay)
            {
                x = AutoCoordinates(board, isEasy)[0];
                y = AutoCoordinates(board, isEasy)[1];
            }
            else
            {
                Console.Write($"{playerName}, choose a cell to place your Queen in the format (row; col): ");
                string playerInput = Console.ReadLine();
                playerInput.Replace('(', ' ');
                playerInput.Replace(')', ' ');

                int[] validatedCoordinates = ValidateCoordinates(playerInput, board);
                x = validatedCoordinates[0];
                y = validatedCoordinates[1];

            }
            board[x, y] = $"[ {playerNum} ]";
            Blocking(board, x, y, playerNum);
            Console.Clear();
            PrintBoard(board);
        }
        private static int[] AutoCoordinates(string[,] board, bool isEasy)
        {
            
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            int autoRow = 0;
            int autoCol = 0;

            //For Easy Mode
            if (isEasy)
            {
                var r = new Random();
                autoRow = r.Next(0, rows - 1);
                autoCol = r.Next(0, cols - 1);
                while (board[autoRow, autoCol] != null)
                {
                    autoRow = r.Next(0, rows - 1);
                    autoCol = r.Next(0, cols - 1);
                }
            }
            else //For Hard mode
            {
                var maxCells = int.MinValue;
                //get all free cells
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {
                        var currentBlockingCells = 0;

                        if (board[i, j] == null)
                        {
                            //finds how many free cells in cur col after step
                            for (int r = 0; r < rows; r++)
                            {
                                if (board[r, j] == null)
                                {
                                    currentBlockingCells++;
                                }
                            }
                            //finds how many free cells in cur row after step
                            for (int c = 0; c < cols; c++)
                            {
                                if (board[i, c] == null)
                                {
                                    currentBlockingCells++;
                                }
                            }
                            //blocks the current diagonals
                            for (int r = 0; r < board.GetLength(0); r++)
                            {
                                for (int c = 0; c < board.GetLength(1); c++)
                                {
                                    if (Math.Abs(r - i) == Math.Abs(c - j))
                                    {
                                        if (board[r, c] == null)
                                        {
                                            currentBlockingCells++;
                                        }
                                    }
                                }
                            }
                        }
                        if (maxCells < currentBlockingCells)
                        {
                            maxCells = currentBlockingCells;
                            autoRow = i;
                            autoCol = j;
                        }
                    }
                }
            }
            
            return new int[] { autoRow, autoCol };
        }
        private static void Blocking(string[,] board, int x, int y, int playerNum)
        {
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            //blocks the current col
            for (int i = 0; i < rows; i++)
            {
                ValidateBlockingCells(i, y, board);
            }
            //blocks the current row
            for (int i = 0; i < cols; i++)
            {
                ValidateBlockingCells(x, i, board);

            }
            //blocks the current diagonals
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (Math.Abs(i - x) == Math.Abs(j - y))
                    {
                        ValidateBlockingCells(i, j, board);
                    }
                }
                Console.WriteLine();
            }
        }
        private static void ValidateBlockingCells(int x, int y, string[,] board)
        {
            string currentCell = board[x, y];
            if (currentCell == null)
            {
                board[x, y] = $"[ * ]";
            }
        }
        private static bool IsTherePossibleMoves(string[,] board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private static void PrintBoard(string[,] board)
        {
            PrintLogo(board);
            Console.Write("    ");
            for (int i = 0; i < board.GetLength(1); i++)
            {
                Console.Write($"{i + 1:d2}.  ");
            }
            Console.WriteLine();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                Console.Write($"{i + 1:d2}.");
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write(board[i, j]);
                    if (board[i, j] == null)
                    {
                        Console.Write("[   ]");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void PrintLogo(string[,] board)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            int cols = board.GetLength(1);
            for (int i = 0; i < Math.Max(17, cols * 6); i++)
            {
                Console.Write('*');
            }
            Console.WriteLine();
            Console.Write("*");
            for (int i = 0; i < Math.Max((cols * 6 - 17) / 2, 0); i++)
            {
                Console.Write(" ");
            }
            Console.Write("QUEENS THE GAME");
            for (int i = 0; i < Math.Max((cols * 6 - 17) / 2, 0); i++)
            {
                Console.Write(" ");
            }
            Console.Write('*');
            Console.WriteLine();
            for (int i = 0; i < Math.Max(17, cols * 6); i++)
            {
                Console.Write('*');
            }
            Console.WriteLine();
            Console.ResetColor();
        }

    }
}

