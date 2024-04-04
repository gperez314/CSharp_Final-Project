﻿using System.Diagnostics;
using System.Xml.Linq;

class Player
{
    public string Name { get; set; }
    public int ID { get; set; }


    public Player(string name, int id)
    {
        Name = name;
        ID = id;
    }
    public override string ToString()
    {
        return $"Player: {Name}";
    }

    public int Turn()
    {
        Console.WriteLine($"{Name}: Please input the column to drop move");
        int playerTurn = int.Parse(Console.ReadLine());
        return playerTurn;
    }
}


class Connect4Game
{
    private static int _turn { get; set; }
    private static Player _player1 { get; set; }
    private static Player _player2 { get; set; }

    private static Board _board { get; set; }


    static Connect4Game()
    {
        _turn = 0;
    }

    public static void SetupGame()
    {
        Console.WriteLine("Please input player 1 name:");
        _player1 = new Player(Console.ReadLine(), 1);

        Console.WriteLine("Please input player 2 name:");
        _player2 = new Player(Console.ReadLine(), 2);

        _board = new Board();
    }

    private static bool MakeTurn(Player _player)
    {
        _board.UpdateBoard(_player, _player.Turn());
        return CheckWin(_player);
    }

    private static bool CheckWin(Player _player)
    {
        if (_turn > 5)
        {
            Console.WriteLine($"Congratulatioons {_player} wins!");
            return false;
        }
        return true;
    }


    public static bool Play()
    {
        _turn++;
        if (_turn % 2 == 1)
            return MakeTurn(_player1);
        else
            return MakeTurn(_player2);
    }

    public static bool PlayAgain()
    {
        Console.WriteLine("Do you want to play again (Y/N)?");
        return char.Parse(Console.ReadLine()) == 'Y';
    }

}

class Board
{
    public int[,] BoardState { get; set; }

    private static int _rows { get; set; } = 7;
    private static int _columns { get; set; } = 7;

    public Board()
    {
        BoardState = new int[_rows, _columns];

        // Initialize each element to zero
        for (int i = 0; i < _rows; i++)
        {
            for (int j = 0; j < _columns; j++)
            {
                BoardState[i, j] = 0;
            }
        }
    }

    public void UpdateBoard(Player player, int mark)
    {
        BoardState[0, mark] = player.ID;
        PrintBoard();
    }

    public void PrintBoard()
    {
        for (int i = 0; i < _rows; i++)
        {
            Console.Write("|  ");
            for (int j = 0; j < _columns; j++)
            {
                Console.Write(BoardState[i, j].ToString() + "  ");
            }
            Console.Write("|");
            Console.WriteLine();
        }
    }


}




class Program
{

    static void Main(string[] args)
    {
        bool run = true;
        while (run)
        {
            Console.Clear();
            Connect4Game.SetupGame();
            while (Connect4Game.Play()) ;
            run = Connect4Game.PlayAgain();
        }




    }
}