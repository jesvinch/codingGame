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
namespace MyNamespace
{
public class HyperSonic_v1
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

        int bombTimer = 0;
        int xAxisToGetTo = 0;
        int yAxisToGetTo = 0;
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
            ReadEntities(board);

            var player1 = board.Entities.First(x => x.Type == EntityType.Player && x.Owner == Owner.You); // get our player
            Console.Error.Write("owner:" + player1.Owner);
            Console.Error.Write(" x:" + player1.X);
            Console.Error.WriteLine(" y:" + player1.Y);

            var nearestCellWithBox = board.GetCellWithLotsOfBoxes(player1.Y);
            Console.Error.WriteLine("Nearest axis with Box - X:" + nearestCellWithBox.XAxis + ", Y:" + nearestCellWithBox.YAxis);

            if (bombTimer != 0)
            {
                bombTimer--;
            }

            // check if position reached if not keep going
            if (!(player1.X == xAxisToGetTo && player1.Y == yAxisToGetTo))
            {
                Console.Error.WriteLine("Keep moving");
                player1.Move(xAxisToGetTo.ToString(), yAxisToGetTo.ToString());
                continue;
            }

            // if next to a box wait there. 
            if (player1.X == nearestCellWithBox.XAxis - 1 || player1.X == nearestCellWithBox.XAxis + 1)
            {
                if (bombTimer == 0)
                {
                    player1.Bomb(player1.X.ToString(), player1.Y.ToString());
                    bombTimer = 7;
                    continue;
                }

                player1.Move(xAxisToGetTo.ToString(), yAxisToGetTo.ToString());
                continue;
            }

            // Determine if you should move to right or left of box
            if (nearestCellWithBox.XAxis == 0)
            {
                xAxisToGetTo = nearestCellWithBox.XAxis + 1;
            }
            else
            {
                xAxisToGetTo = nearestCellWithBox.XAxis - 1;
            }

            yAxisToGetTo = nearestCellWithBox.YAxis;
            var xx = xAxisToGetTo.ToString();
            var y = yAxisToGetTo.ToString();

            player1.Move(xx, y);

            // BOMB and MOVE - one move
            // MOVE - one move. 
            // Bomb and wait till bomb goes off. Find best place to bomb
            //player1.Move(nearestCellWithBox.XAxis.ToString(), (nearestCellWithBox.YAxis - 1).ToString());
            //player1.Bomb(nearestCellWithBox.XAxis.ToString(), (nearestCellWithBox.YAxis - 1).ToString());


            // Console.WriteLine("BOMB " + xCoordinate + " " + yCoordinate);
            // Console.WriteLine("MOVE 6 5");
            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");
        }
    }
    private static void ReadEntities(Board board)
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

            var entity = new Entity(entityType, owner, x, y, param1, param2);
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
    public Entity(int type, int owner, int x, int y, int param1, int param2)
    {
        Type = DetermineEntityType(type);
        Owner = DetermineOwner(owner);
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

    public static Owner DetermineOwner(int owner)
    {
        if (owner == 0)
        {
            return Owner.You;
        }
        return Owner.Enemy;
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

    public Cell GetCellWithLotsOfBoxes(int currentPosition)
    {
        //int upperXRange = currentPosition + 2 > Width ? Width : currentPosition + 2;
        //int lowerXRange = currentPosition - 2 < 0 ? 0 : currentPosition;
        //var cellsWithinBombRange = Cells.Where(x => Enumerable.Range(lowerXRange, upperXRange).Contains(x.XAxis));
        //var cellsWithBoxs = cellsWithinBombRange.Where(x => x.CellState == CellState.Box);

        var cellsWithBoxs = Cells.Where(x => x.YAxis == currentPosition && x.CellState == CellState.Box);

        // Can't find any boxes in current row? Go to next row. 
        int nextYAxis = currentPosition + 1;
        while (!cellsWithBoxs.Any())
        {
            Console.Error.WriteLine("GO to next row");
            cellsWithBoxs = FindNextCellWithBox(nextYAxis);
            nextYAxis++;
        }
        // Determine cell score.
        foreach (var cell in cellsWithBoxs)
        {
            if (cell.GetRightNeighbourPlus2(Cells, Width, Height) != null)
            {
                if (cell.GetRightNeighbourPlus2(Cells, Width, Height).CellState == CellState.Box)
                {
                    cell.Score += 1;
                }
            }
            if (cell.GetLeftNeighbourPlus2(Cells, Width, Height) != null)
            {
                if (cell.GetLeftNeighbourPlus2(Cells, Width, Height).CellState == CellState.Box)
                {
                    cell.Score += 1;
                }
            }
            Console.Error.WriteLine("x:" + cell.XAxis + " y:" + cell.YAxis + " Box score:" + cell.Score);
        }
        return cellsWithBoxs.OrderByDescending(x => x.Score).FirstOrDefault();
    }

    private IEnumerable<Cell> FindNextCellWithBox(int yAxis)
    {
        return Cells.Where(x => x.YAxis == yAxis && x.CellState == CellState.Box);
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

    public int Score { get; set; }
    public Cell GetRightNeighbourPlus2(List<Cell> cells, int height, int width)
    {
        if (XAxis + 2 > width)
        {
            return null;
        }
        return cells.SingleOrDefault(x => x.XAxis == XAxis + 2 && x.YAxis == YAxis);
    }

    public Cell GetLeftNeighbourPlus2(List<Cell> cells, int height, int width)
    {
        if (XAxis - 2 < 0)
        {
            return null;
        }
        return cells.SingleOrDefault(x => x.XAxis == XAxis - 2 && x.YAxis == YAxis);
    }

}

public enum EntityType
{
    Player,
    Bomb
}

public enum Owner
{
    Enemy,
    You
}

public enum CellState
{
    EmptyCell,
    Box
}
}