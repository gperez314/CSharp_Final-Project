// Name: COURSE FINAL TERM PROJECT
// Course Code: SODV1202
// Class: Introduction to Object Oriented Programming
// Author: Glenn Perez, Rod Stephen Espiritu
// ==============================================================================================================
// CONNECT-4 GAME DEVELOPMENT PROJECT
// ==============================================================================================================


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


// ==============================================================================================================
// Board Class: Represents the playing board where the players will cast their moves
// ==============================================================================================================
class Board
{
    // Define properties for BoardState and number of rows and columns
    public int[,] BoardState { get; set; }
    public static int Rows { get; private set; } = 7; // private set
    public static int Columns { get; private set; } = 7; // private set

    // Construtor for the Board class
    public Board()
    {
        // Create a 2D array to represent the board state
        BoardState = new int[Rows, Columns];

        // Initialize each board state element to zero
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                BoardState[i, j] = 0;
            }
        }
    }

    // Method to update the boardstate based on the player's move
    public void UpdateBoard(Player player, int mark)
    {
        // Get the next available cell in the selected column
        int nextCell = GetNextAvailCell(mark);
        // Check if there is an available cell in the selected column
        if (nextCell >= 0)
        {
            // Cell is available. Update board state.
            BoardState[nextCell, mark] = player.ID;
        }
        else
        {
            // Display invalid move message and ask the player to select a different turn
            Console.WriteLine("Invalid move. Column is already full!");
            Connect4Game.MakeTurn(player); // Repeat player's turn function
        }
    }

    // Method to get the next available cell in the selected column
    private int GetNextAvailCell(int column)
    {
        for (int i = Rows - 1; i >= 0; i--)
        {
            if (BoardState[i, column] == 0)
                return i; // Available cell found in the current column
        }
        return -1; // No more available cells
    }

    // Method to determine if the board is already full
    public bool IsBoardFull()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (BoardState[i, j] == 0)
                    return false; // Board is not yet full
            }
        }
        return true; // Board already full
    }

    // Method to check for the winning combinations in the current board
    // A CheckWin algorithm written in Java was found and utilized
    // Source: https://stackoverflow.com/questions/32770321/connect-4-check-for-a-win-algorithm
    public bool CheckWin(int player)
    {
        // Check for horizontal combinations
        for (int j = 0; j < Rows - 3; j++)
        {
            for (int i = 0; i < Columns; i++)
            {
                if (BoardState[i, j] == player && BoardState[i, j + 1] == player &&
                    BoardState[i, j + 2] == player && BoardState[i, j + 3] == player)
                {
                    return true; // combination found
                }
            }
        }

        // Check for vertical combinations
        for (int i = 0; i < Columns - 3; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if (BoardState[i, j] == player && BoardState[i + 1, j] == player &&
                    BoardState[i + 2, j] == player && BoardState[i + 3, j] == player)
                {
                    return true; // combination found
                }
            }
        }

        // Check for ascending diagonal combinations
        for (int i = 3; i < Columns; i++)
        {
            for (int j = 0; j < Rows - 3; j++)
            {
                if (BoardState[i, j] == player && BoardState[i - 1, j + 1] == player &&
                    BoardState[i - 2, j + 2] == player && BoardState[i - 3, j + 3] == player)
                    return true; // combination found
            }
        }

        // Check for descending diagonal combinations
        for (int i = 3; i < Columns; i++)
        {
            for (int j = 3; j < Rows; j++)
            {
                if (BoardState[i, j] == player && BoardState[i - 1, j - 1] == player &&
                    BoardState[i - 2, j - 2] == player && BoardState[i - 3, j - 3] == player)
                    return true; // combination found
            }
        }

        return false; // No winning combination found
    }
}


// ==============================================================================================================
// Connect4Game Class: The class which manages all the other classes to collectively create the game
// ==============================================================================================================
class Connect4Game
{
    // Define properties for the players and board objects
    // Define properties for static fields (turn, win, boardfull, mode)
    private static Player _player1 { get; set; }
    private static Player _player2 { get; set; }
    private static Board _board { get; set; }
    private static int _turn { get; set; } // current turn count
    private static bool _win { get; set; } // win condition
    private static bool _boardFull { get; set; } // board cells full
    private static int _mode { get; set; } // game mode [0: 2-Player , 1: vs. Computer] 

    // Construtor for the Connect4Game class
    static Connect4Game()
    {
        // Reset all static fields
        _turn = 0;
        _win = false;
        _boardFull = false;
        _mode = 0;
    }

    // Method to set-up the game parameters
    public static void SetupGame()
    {
        // Reset all static fields
        _turn = 0;
        _win = false;
        _boardFull = false;
        _mode = 0;

        _board = new Board();  // Create a new board object

        // Display game mode window and get mode from the user
        Display.GetGameMode(); // Game mode visuals
        _mode = GameMode(); // Ask user to select game mode
        Display.ShowGameMode(_mode); // Display selected game mode visuals

        // Evaluate selected mode (0: 2-Player , 1: vs. Computer. 3: Computer vs. Computer)
        if (_mode == 1)
        {
            // 2-Player Mode selected.
            // Get Player 1 name and create Player 1 object
            Display.GetPlayerName(" Please enter Player 1's name: ", 1);
            _player1 = new Player(Console.ReadLine(), 1);
            // Get Player 2 name and create Player 2 object
            Display.GetPlayerName(" Please enter Player 2's name: ", 2);
            _player2 = new Player(Console.ReadLine(), 2);
        }
        else if (_mode == 2)
        {
            // vs. Computer Mode selected.
            // Get Player 1 name and create Player 1 object
            Display.GetPlayerName(" Please enter Player 1's name: ", 1);
            _player1 = new Player(Console.ReadLine(), 1);
            // Create AI object. (Name: Computer , ID: 2)
            _player2 = new AI("Computer", 2);
        }
        else if (_mode == 3)
        {
            // Computer vs. Computer Mode selected.
            // Create AI object. (Name: Computer1 , ID: 1)
            Display.GetPlayerName(" Please enter Player 1's name: ", 1);
            _player1 = new AI("Computer 1", 1);
            // Create AI object. (Name: Computer2 , ID: 2)
            _player2 = new AI("Computer 2", 2);
        }
        // Display Connect4 Board Window
        Display.PrintBoard(_board.BoardState, _win);
    }

    // Method to get the Game Mode from the user
    public static int GameMode()
    {
        // Get game mode input from console and check for errors
        Console.Write("Please select game mode: ");
        try
        {
            // Read data from console
            _mode = int.Parse(Console.ReadLine());
            // Check if valid range
            if (_mode != 1 && _mode != 2 && _mode != 3)
                throw new ArgumentOutOfRangeException();
        }
        // Error handler for invalid format inputs
        catch
        {
            Console.WriteLine("Invalid game mode!");
            _mode = GameMode(); // Repeat getting game mode
        }
        return _mode; // Return game mode
    }

    // Method to play the Connect4Game
    // Handle alternately assigning users' turn. Return boolean true if there is a winner/draw.
    public static bool Play()
    {
        _turn++; // Increment turn count
        // Assign turn to Player 1
        if (_turn % 2 == 1)
            return !MakeTurn(_player1); // Player 1 turn
        // Assign turn to Player 2
        else
            return !MakeTurn(_player2); // Player 2 turn
    }

    // Method to make a player's turn. Return boolean true if there is a winner/draw.
    public static bool MakeTurn(Player _player)
    {
        // Get player's turn and update board state
        _board.UpdateBoard(_player, _player.Turn().Result);
        // Check if there is already a winner
        _win = _board.CheckWin(_player.ID);
        // Check if the board is already full/ draw
        _boardFull = _board.IsBoardFull();
        // Display Connect4 Board Window
        Display.PrintBoard(_board.BoardState, _win || _boardFull);

        // Evaluate game result and display corresponding visuals
        if (_win)
        {
            // Console display for an AI winning in vs. Computer mode
            if ((_mode == 2) && (_player.ID == 2))
                Display.LosttoAI(_player.Name, _player.ID);
            // Console display for a player winning
            else
                Display.Winner(_player.Name, _player.ID);
        }
        // Console display for a draw
        if (_boardFull && !_win)
            Display.Draw();

        return (_win || _boardFull); // Return game result
    }

    // Method to replay the game
    public static bool PlayAgain()
    {
        // Ask if the user wants to replay the game
        Console.Write("Do you want to play again (Y/N)? ");
        // Get user input from console and check for errors
        string retry = Console.ReadLine().ToUpper();
        try
        {
            // Check if valid input
            if (retry != "Y" && retry != "N")
                throw new ArgumentException();
        }
        // Error handler for invalid format inputs
        catch
        {
            Console.WriteLine("Invalid option entered!");
            return PlayAgain();
        }
        return retry == "Y"; // return valid user input
    }
}


// ==============================================================================================================
// Display Class: A class dedicated for displaying the game visuals in the console
// ==============================================================================================================
public class Display
{
    // Method to display the Game Title
    public static void DisplayTitle()
    {
        Console.WriteLine("=========================================");
        Console.WriteLine("|                                       |");
        Console.WriteLine("|   CONNECT-4 GAME DEVELOPMENT PROJECT  |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("-----------------------------------------");
    }

    // Method to display the Get Game Mode Window
    public static void GetGameMode()
    {
        Console.Clear();
        DisplayTitle();
        Console.WriteLine("| Glenn Perez  |  Rod Stephen Espiritu  |");
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("| <<<<<<<<<<<< SELECT MODE >>>>>>>>>>>> |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("|        [1]: 2 Player                  |");
        Console.WriteLine("|        [2]: vs. Computer              |");
        Console.WriteLine("|        [3]: Computer vs. Computer     |");
        Console.WriteLine("|                                       |");
        Console.WriteLine("=========================================");
    }

    // Method to display the Game Mode Window
    public static void ShowGameMode(int mode)
    {
        Console.Clear();
        DisplayTitle();
        Console.WriteLine("| Glenn Perez  |  Rod Stephen Espiritu  |");
        if (mode == 1)
        {
            // Display for 2-Player Mode
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("| <<<<<<<<<<  2-PLAYER MODE  >>>>>>>>>> | ");
            Console.WriteLine("-----------------------------------------");
        }
        else
        {
            // Display for vs. Computer Mode
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("| <<<<<<<<  VS. COMPUTER MODE  >>>>>>>> | ");
            Console.WriteLine("-----------------------------------------");
        }
    }

    // Method to display the Get Player Name string with the designated color
    public static void GetPlayerName(string str, int id)
    {
        Console.WriteLine("");
        ColoredDisplay("   ", id);
        Console.Write(str);
    }

    // Method to display the Connect4 GameBoard
    public static void PrintBoard(int[,] board, bool win)
    {
        Console.Clear(); // Clear console
        DisplayTitle(); // Display Game title

        // Reconstruct Game Board display with the correct symbols
        for (int i = 0; i < Board.Rows; i++)
        {
            Console.Write("|    ");
            for (int j = 0; j < Board.Columns; j++)
            {
                Symbol(board[i, j]); // colored symbol
                Console.Write("    ");
            }
            Console.Write("|");
            Console.WriteLine();
        }

        // Append column numbers at the end
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("|    1    2    3    4    5    6    7    |");
        Console.WriteLine("-----------------------------------------");

        // If there is no winner yet, append additional instruction to cast move
        if (!win)
        {
            Console.WriteLine("| Please input column no. to cast move: |");
            Console.WriteLine("=========================================");
        }
    }

    // Method to dispay the player's turn message with the player name colored
    public static async void PlayerTurn(string name, int id)
    {
        Display.ColoredDisplay(name + "'s", id); // colored name display
        Console.Write(" turn: ");
    }

    // Method to display the winning player message
    public static void Winner(string name, int id)
    {
        Console.Write("| Congratulations! ");
        Display.ColoredDisplay(name, id); // colored name display
        Console.WriteLine(" wins! ");
        Console.WriteLine("=========================================");
    }

    // Method to display the winning AI message
    public static void LosttoAI(string name, int id)
    {
        Console.Write("| Sorry ");
        Display.ColoredDisplay(name, id); // colored name display
        Console.WriteLine(" wins! =( ");
        Console.WriteLine("=========================================");
    }

    // Method to display the game draw message
    public static void Draw()
    {
        Console.WriteLine("| The game is a draw!                   |");
        Console.WriteLine("=========================================");
    }

    // Method to display the corresponding colored symbols
    public static void Symbol(int id)
    {
        if (id == 1)
            ColoredDisplay("X", id); // Player 1 symbol: "X"
        else if (id == 2)
            ColoredDisplay("O", id); // Player 1 symbol: "O"
        else
            Console.Write("-"); // Available symbol: "-"
    }

    // Method to color the desired message
    public static void ColoredDisplay(string str, int id)
    {
        // Set background color based from player ID
        // Player 1: Red, Player 2: Blue
        if (id == 1)
            Console.BackgroundColor = ConsoleColor.Red;
        else
            Console.BackgroundColor = ConsoleColor.Blue;
        // Set foreground color to black
        Console.ForegroundColor = ConsoleColor.Black;
        // Write the colored string on colored background
        Console.Write(str);
        // Reset background and foreground colors to default
        Console.ResetColor();
    }
}
// ==============================================================================================================


// ==============================================================================================================
// Program Class: Main Program
// ==============================================================================================================
class Program
{
    static void Main(string[] args)
    {
        // Initialize run boolean to true
        bool run = true;

        // Continue to run the program until run boolean is false
        while (run)
        {
            // Set-up the Game parameters
            Connect4Game.SetupGame();
            // Play the Connect 4 Game
            while (Connect4Game.Play()) ;
            // Ask the user whether to replay the game
            run = Connect4Game.PlayAgain();
        }
    }
}
// ==============================================================================================================
// ==============================================================================================================