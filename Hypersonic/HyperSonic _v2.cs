using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

/*
 * https://www.codingame.com/ide/puzzle/hypersonic
 * The game is played on a grid 13 units wide and 11 units high. The coordinate X=0, Y=0 is the top left cell.
 * Each cell of the grid is either a floor or a box. Floor cells are indicated by a dot ( .), and boxes by a zero ( 0).
 * 
 */

/* Progressed from Wood League 3 --> Wood League 2*/
namespace MyNamespace2
{
public class HyperSonic_v2
{
    static void Main(string[] args)
    {
        // TODO: Take this out from here.
        // Initialization inputs
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]);
        int height = int.Parse(inputs[1]);
        int myId = int.Parse(inputs[2]);

        int gameLoop = 0;
        int bombTimer = 0;
        int xAxisToGetTo = 0;
        int yAxisToGetTo = 0;
        bool startingAtTopLeft = true;
        // Game loop
        while (true)
        {
            // Game loop inputs: Height, an integer entities for the amount of entities on the grid, Entities
            Board board = new Board
            {
                Height = height,
                Width = width
            };
            ReadBoard(height, board);
            ReadEntities(board, myId);

            var playerOne = board.Entities.First(x => x.Type == EntityType.Player && x.Owner == Owner.You); // get our player
            Console.Error.Write("owner:" + playerOne.Owner);
            Console.Error.Write(" x:" + playerOne.X);
            Console.Error.WriteLine(" y:" + playerOne.Y);
            
            if (gameLoop == 0 && playerOne.X != 0)
            {
                startingAtTopLeft = false;
            }
            var nearestCellWithBox = board.GetCellWithLotsOfBoxes(playerOne.Y, startingAtTopLeft);
            Console.Error.WriteLine("Nearest axis with Box - X:" + nearestCellWithBox.XAxis + ", Y:" + nearestCellWithBox.YAxis);

            if (bombTimer != 0)
            {
                bombTimer--;
            }

            if (gameLoop == 0)
            {
                xAxisToGetTo = playerOne.X;
                yAxisToGetTo = playerOne.Y;
            }

            // check if position reached if not keep going
            if (!(playerOne.X == xAxisToGetTo && playerOne.Y == yAxisToGetTo))
             {
                Console.Error.WriteLine("Keep moving");
                playerOne.Move(xAxisToGetTo.ToString(), yAxisToGetTo.ToString());
                continue;
            }

            // if next to a box wait there. 
            if (playerOne.X == nearestCellWithBox.XAxis)
            {
                if (bombTimer == 0) 
                {
                    playerOne.Bomb(playerOne.X.ToString(), playerOne.Y.ToString());
                    bombTimer = 7;
                    continue;
                } 

                playerOne.Move(xAxisToGetTo.ToString(), yAxisToGetTo.ToString());
                continue;
            }
            
            xAxisToGetTo = nearestCellWithBox.XAxis;
            yAxisToGetTo = nearestCellWithBox.YAxis;
            var xx = xAxisToGetTo.ToString();
            var y = yAxisToGetTo.ToString();
            
            playerOne.Move(xx, y);
            gameLoop++;
        }
    }

    /*
     */
    private static void ReadEntities(Board board, int yourId)
    {
        int entities = int.Parse(Console.ReadLine()); // Amount of entities on the grid
        for (int i = 0; i < entities; i++)
        {
            var inputs = Console.ReadLine().Split(' ');
            int entityType = int.Parse(inputs[0]);
            int owner = int.Parse(inputs[1]);
            int x = int.Parse(inputs[2]);
            int y = int.Parse(inputs[3]);
            int param1 = int.Parse(inputs[4]);
            int param2 = int.Parse(inputs[5]);

            Owner ownerValue = Owner.Enemy;
            if (owner == yourId)
            {
                ownerValue = Owner.You;
            }

            var entity = new Entity(entityType, ownerValue, x, y, param1,param2);
            board.Entities.Add(entity);
        }
    }

    private static void ReadBoard(int height, Board board)
    {
        // Height contains all rows
        for (int i = 0; i < height; i++)
        {
            string row = Console.ReadLine(); // represent each row.

            // Read each column in the row and add this to board
            var columns = row.ToCharArray();
            int x = 0;
            foreach (var column in columns)
            {
                // Determine if the cell is empty or has box
                if (column.ToString() == ".")
                {
                    board.AddCells(new Cell(x, i, CellState.EmptyCell));
                }
                else
                {
                    board.AddCells(new Cell(x, i, CellState.Box));
                }
                x++;
            }
        }
    }
}

/* Represent the game state of current round. Represent each player and where they are and how many bombs
   they have. owner: 0 == you. owner:1 == enemy */
public class Entity
{
    public Entity(int type, Owner owner, int x, int y, int param1, int param2)
    {
        Type = DetermineEntityType(type);
        Owner = owner;
        X = x;
        Y = y;
        Param1 = param1;
        Param2 = param2;
    }

    public EntityType Type { get; set; } // 0 == players : 1 == bombs
    public Owner Owner { get; set; } // ID of player (0 or 1). If Bombs, bombs owner ID.
    public int X { get; set; }
    public int Y { get; set; }
    public int Param1 { get; set; } // player or bomb - player: number of bombs player can still place. Bomb: rounds to explode.
    public int Param2 { get; set; } // currently not needed

    public void Bomb(string x, string y)
    {
        Console.WriteLine("BOMB " + x + " " + y);
    }

    public void Move(string x, string y)
    {
        Console.WriteLine("MOVE " + x + " " + y);
    }

    public static EntityType DetermineEntityType(int entityType)
    {
        if (entityType == 0)
        {
            return EntityType.Player;
        }
        return EntityType.Bomb;
    }
}

public class Board
{
    public int Height { get; set; }
    public int Width { get; set; }
    public List<Entity> Entities { get; set; }
    public List<Cell> Cells { get; set; }
    public Board()
    {
        Cells = new List<Cell>();
        Entities = new List<Entity>();
    }

    public Cell GetCellFromX(int x, int y)
    {
        return Cells.SingleOrDefault(c => c.XAxis == x && c.YAxis == y);
    }

    public void AddCells(Cell cell)
    {
        Cells.Add(cell);
    }

    public Cell GetCellWithLotsOfBoxes(int currentYAxis, bool startingAtTop)
    {
        var nextYAxis = currentYAxis;
        var cellsWithBoxs = Cells.Where(x => x.YAxis == nextYAxis && x.CellState == CellState.Box);
        
        // Can't find any boxes in current row? Go to next row.
        while (!cellsWithBoxs.Any())
        {
            Console.Error.WriteLine("Go to next row");
            if (startingAtTop)
            {
                nextYAxis++;
            }
            else
            {
                nextYAxis--;
            }
            cellsWithBoxs = CheckIfRowHasBoxes(nextYAxis);
            
        }

        var emptyCells = Cells.Where(x => x.YAxis == nextYAxis && x.CellState == CellState.EmptyCell);

        // Determine cell score.
        foreach (var cell in emptyCells)
        {
            cell.GetRightNeighbour(Cells, Width, Height);
            cell.GetLeftNeighbour(Cells, Width, Height);

            if (cell.RightNeighbour?.CellState == CellState.Box)
            {
                cell.Score += 1;
            }
            if (cell.LeftNeighbour?.CellState == CellState.Box)
            {
                cell.Score += 1;
            }
            Console.Error.WriteLine("x:" + cell.XAxis + " y:" + cell.YAxis+ " Box score:" + cell.Score);
        }
        return emptyCells.OrderByDescending(x => x.Score).FirstOrDefault();
    }

    private IEnumerable<Cell> CheckIfRowHasBoxes(int rowNumber)
    {
        return Cells.Where(x => x.YAxis == rowNumber && x.CellState == CellState.Box);
    }
}

/* Represent each cell */
public class Cell
{
    public Cell(int xAxis, int yAxis, CellState cellState)
    {
        XAxis = xAxis;
        YAxis = yAxis;
        CellState = cellState;
    }

    public int XAxis;
    public int YAxis;
    public CellState CellState;
    public bool Bombed;
    public Cell RightNeighbour { get; set; }
    public Cell LeftNeighbour { get; set; }

    public int Score { get; set; }

    public void GetRightNeighbour(List<Cell> cells, int height, int width)
    {
        if (XAxis + 1 > width)
        {
            RightNeighbour = null;
        }
        RightNeighbour =  cells.SingleOrDefault(x => x.XAxis == XAxis + 1 && x.YAxis == YAxis);
    }

    public void GetLeftNeighbour(List<Cell> cells, int height, int width)
    {
        if (XAxis - 1 < 0)
        {
            LeftNeighbour = null;
        }
        LeftNeighbour =  cells.SingleOrDefault(x => x.XAxis == XAxis - 1 && x.YAxis == YAxis);
    }

}

public enum EntityType
{
    Player,
    Bomb,
    Item
}

public enum Owner
{
    Enemy,
    You
}

public enum CellState{
    EmptyCell,
    Box
}
}