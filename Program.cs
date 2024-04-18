﻿
// ==============================================================================================================
// Player Class: Class to represent the players who will play the game
// ==============================================================================================================
using static System.Net.Mime.MediaTypeNames;

class Player
{
    // Define properties for Player Name and ID
    public string Name { get; set; }
    public int ID { get; set; }

    // Construtor for the Player class
    public Player(string name, int id)
    {
        Name = name;
        ID = id;
    }

    // Override ToString method to display the player name
    public override string ToString()
    {
        return $"Player: {Name}";
    }

    // Method for the player to make a turn
    public virtual async Task<int> Turn()
    {
        // Display player's turn message
        Display.PlayerTurn(Name, ID);
        int playerTurn;
        // Get player turn input from console and check for errors
        try
        {
            // Read data from console
            playerTurn = int.Parse(Console.ReadLine()) - 1;
            // Check if valid range
            if (playerTurn < 0 || playerTurn > 6)
                throw new ArgumentOutOfRangeException();
        }
        // Error handler for input outside of valid column range
        catch (ArgumentOutOfRangeException)
        {
            Console.WriteLine("Invalid move. Column no. out of range!");
            playerTurn = Turn().Result; // Repeat player turn
        }
        // Error handler for invalid format inputs
        catch
        {
            Console.WriteLine("Invalid move. Not a valid column no.!");
            playerTurn = Turn().Result; // Repeat player turn
        }
        return playerTurn; // Return column of player's turn
    }
}


// ==============================================================================================================
// AI Class: Derived from a player class. An AI is a player who has capability to make a move on its own
// ==============================================================================================================
class AI : Player
{
    // Construtor for the AI class calling the base constructor of Player class
    public AI(string name, int id) : base(name, id) { }

    // Method for the AI to make a turn. The method is overriden to give the AI
    // the ability to make its own move.
    public override async Task<int> Turn()
    {
        // Display AI's turn message
        Display.PlayerTurn(Name, ID);
        // Disable console inputs and add a delay
        Console.CancelKeyPress += (_, e) => e.Cancel = true;
        await Task.Delay(500);
        // Select a random number from 0-7 to represent the turn and display on console
        int playerTurn = (new Random()).Next(0, 7);
        Console.Write(playerTurn + 1);
        await Task.Delay(500);
        Console.WriteLine("");
        // Re-enable console inputs
        Console.CancelKeyPress += (_, e) => e.Cancel = false;
        return playerTurn; // Return column of AI's turn
    }
}

class Board
{
    public int[,] BoardState { get; set; }
    public static int Rows { get; private set; } = 7; // private set
    public static int Columns { get; private set; } = 7; // private set

    public Board()
    {
        BoardState = new int[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                BoardState[i, j] = 0;
            }
        }
    }

    public void UpdateBoard(Player player, int mark)
    {
        int nextCell = GetNextAvailCell(mark);
        if (nextCell >= 0)
        {
            BoardState[nextCell, mark] = player.ID;
        }
        else
        {
            Console.WriteLine("Invalid move. Column is already full!");
            Connect4Game.MakeTurn(player);
        }
    }


    private int GetNextAvailCell(int column)
    {
        for (int i = Rows - 1; i >= 0; i--)
        {
            if (BoardState[i, column] == 0)
                return i; 
        }
        return -1;
    }

    public bool IsBoardFull()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (BoardState[i, j] == 0)
                    return false; 
            }
        }
        return true;
    }

    public bool CheckWin(int player)
    {

        for (int j = 0; j < Rows - 3; j++)
        {
            for (int i = 0; i < Columns; i++)
            {
                if (BoardState[i, j] == player && BoardState[i, j + 1] == player &&
                    BoardState[i, j + 2] == player && BoardState[i, j + 3] == player)
                {
                    return true;
                }
            }
        }

        for (int i = 0; i < Columns - 3; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (BoardState[i, j] == player && BoardState[i + 1, j] == player &&
                    BoardState[i + 2, j] == player && BoardState[i + 3, j] == player)
                {
                    return true; 
                }
            }
        }

        for (int i = 3; i < Columns; i++)
        {
            for (int j = 0; j < Rows - 3; j++)
            {
                if (BoardState[i, j] == player && BoardState[i - 1, j + 1] == player &&
                    BoardState[i - 2, j + 2] == player && BoardState[i - 3, j + 3] == player)
                    return true;
            }
        }


        for (int i = 3; i < Columns; i++)
        {
            for (int j = 3; j < Rows; j++)
            {
                if (BoardState[i, j] == player && BoardState[i - 1, j - 1] == player &&
                    BoardState[i - 2, j - 2] == player && BoardState[i - 3, j - 3] == player)
                    return true;
            }
        }

        return false;
    }
}


class Connect4Game
{
    private static Player _player1 { get; set; }
    private static Player _player2 { get; set; }
    private static Board _board { get; set; }
    private static int _turn { get; set; } // current turn count
    private static bool _win { get; set; } // win condition
    private static bool _boardFull { get; set; } // board cells full
    private static int _mode { get; set; } // game mode [0: 2-Player , 1: vs. Computer] 

    static Connect4Game()
    {
        _turn = 0;
        _win = false;
        _boardFull = false;
        _mode = 0;
    }

    public static void SetupGame()
    {
        _turn = 0;
        _win = false;
        _boardFull = false;
        _mode = 0;

        _board = new Board();

        Display.GetGameMode();
        _mode = GameMode(); 
        Display.ShowGameMode(_mode);
        Display.GetPlayerName(" Please enter Player 1's name: ", 1);
        _player1 = new Player(Console.ReadLine(), 1);

        if (_mode == 1)
        {
            Display.GetPlayerName(" Please enter Player 2's name: ", 2);
            _player2 = new Player(Console.ReadLine(), 2);
        }
        else
        {
            _player2 = new AI("Computer", 2);
        }
        Display.PrintBoard(_board.BoardState, _win);
    }

    public static int GameMode()
    {
        Console.Write("Please select game mode: ");
        try
        {
            _mode = int.Parse(Console.ReadLine());
            if (_mode != 1 && _mode != 2)
                throw new ArgumentOutOfRangeException();
        }
       
        catch
        {
            Console.WriteLine("Invalid game mode!");
            _mode = GameMode();
        }
        return _mode;
    }


    public static bool Play()
    {
        _turn++;
        if (_turn % 2 == 1)
            return !MakeTurn(_player1);
        else
            return !MakeTurn(_player2);
    }

    public static bool MakeTurn(Player _player)
    {
        _board.UpdateBoard(_player, _player.Turn().Result);
        _win = _board.CheckWin(_player.ID);
        _boardFull = _board.IsBoardFull();
        Display.PrintBoard(_board.BoardState, _win || _boardFull);

        if (_win)
        {
            if ((_mode != 1) && (_player.ID == 2))
                Display.LosttoAI(_player.Name, _player.ID);
            else
                Display.Winner(_player.Name, _player.ID);
        }
        if (_boardFull && !_win)
            Display.Draw();

        return (_win || _boardFull);
    }

    public static bool PlayAgain()
    {
        Console.Write("Do you want to play again (Y/N)? "); 
        string retry = Console.ReadLine().ToUpper();
        try
        {
            if (retry != "Y" && retry != "N")
                throw new ArgumentException();
        }
        
        catch
        {
            Console.WriteLine("Invalid option entered!");
            return PlayAgain();
        }
        return retry == "Y"; 
    }
}


public class Display
{
    public static void DisplayTitle()
    {
        Console.WriteLine("=========================================");
        Console.WriteLine("|                                       |");
        Console.WriteLine("|   CONNECT-4 GAME DEVELOPMENT PROJECT  |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("-----------------------------------------");
    }

    public static void GetGameMode()
    {
        Console.Clear();
        DisplayTitle();
        Console.WriteLine("| Glenn Perez  |  Rod Stephen Espiritu  |");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("| <<<<<<<<<<<< SELECT MODE >>>>>>>>>>>> |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("|          [1]: 2 Player                |");
        Console.WriteLine("|          [2]: vs. Computer            |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("=========================================");
    }

    public static void ShowGameMode(int mode)
    {
        Console.Clear();
        DisplayTitle();
        Console.WriteLine("| Glenn Perez  |  Rod Stephen Espiritu  |");
        if (mode == 1)
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("| <<<<<<<<<<  2-PLAYER MODE  >>>>>>>>>> | ");
            Console.WriteLine("-----------------------------------------");
        }
        else
        {
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("| <<<<<<<<  VS. COMPUTER MODE  >>>>>>>> | ");
            Console.WriteLine("-----------------------------------------");
        }
    }

    public static void GetPlayerName(string str, int id)
    {
        Console.WriteLine("");
        ColoredDisplay("   ", id);
        Console.Write(str);
    }

    public static void PrintBoard(int[,] board, bool win)
    {
        Console.Clear();
        DisplayTitle(); 

        for (int i = 0; i < Board.Rows; i++)
        {
            Console.Write("|    ");
            for (int j = 0; j < Board.Columns; j++)
            {
                Symbol(board[i, j]); 
                Console.Write("    ");
            }
            Console.Write("|");
            Console.WriteLine();
        }

        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("|    1    2    3    4    5    6    7    |");
        Console.WriteLine("-----------------------------------------");

        if (!win)
        {
            Console.WriteLine("| Please input column no. to cast move: |");
            Console.WriteLine("=========================================");
        }
    }

    public static async void PlayerTurn(string name, int id)
    {
        Display.ColoredDisplay(name + "'s", id);
        Console.Write(" turn: ");
    }

    public static void Winner(string name, int id)
    {
        Console.Write("| Congratulations! ");
        Display.ColoredDisplay(name, id);
        Console.WriteLine(" wins! ");
        Console.WriteLine("=========================================");
    }

    public static void LosttoAI(string name, int id)
    {
        Console.Write("| Sorry ");
        Display.ColoredDisplay(name, id); 
        Console.WriteLine(" wins! =( ");
        Console.WriteLine("=========================================");
    }

    public static void Draw()
    {
        Console.WriteLine("| The game is a draw!                   |");
        Console.WriteLine("=========================================");
    }

    public static void Symbol(int id)
    {
        if (id == 1)
            ColoredDisplay("X", id);
        else if (id == 2)
            ColoredDisplay("O", id);
        else
            Console.Write("-"); 
    }

    public static void ColoredDisplay(string str, int id)
    {
        if (id == 1)
            Console.BackgroundColor = ConsoleColor.Red;
        else
            Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(str);
        Console.ResetColor();
    }
}
class Program
{
    static void Main(string[] args)
    {
        bool run = true; 

        while (run)
        {
            Connect4Game.SetupGame();
            while (Connect4Game.Play());
            run = Connect4Game.PlayAgain();
        }
    }
}