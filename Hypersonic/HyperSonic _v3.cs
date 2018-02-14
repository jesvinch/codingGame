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
    /* Wood league 2.
     * Boxes drop items now. 
     *      Item 1 == extra range in all directions.
     *      Item 2 == extra bomb.
     *      TODO: Score boxes based on items it contains. 
     */
public class HyperSonic_v3
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
            
            var yourBot = board.GetPlayerEntity(Owner.YourBot);
            if (gameLoop == 0 && yourBot.X != 0)
            {
                startingAtTopLeft = false;
            }

            if (bombTimer != 0)
            {
                bombTimer--;
            }
            if (gameLoop == 0)
            {
                xAxisToGetTo = yourBot.X;
                yAxisToGetTo = yourBot.Y;
            }

            var nearestCellWithBox = board.GetNextMostOptimalCell(yourBot.Y, startingAtTopLeft);
            
            yourBot.ExecuteAction(ref xAxisToGetTo, ref yAxisToGetTo, nearestCellWithBox, ref bombTimer);
            gameLoop++;
            #region DeleteNextTime
            //// Action 1: Check if desired position reached if not keep going
            //if (!(yourBot.X == xAxisToGetTo && yourBot.Y == yAxisToGetTo))
            //{
            //    Console.Error.WriteLine("Keep moving");
            //    yourBot.Move(xAxisToGetTo, yAxisToGetTo);
            //    continue;
            //}

            //// Action 2: if next to a box wait there. 
            //if (yourBot.X == nearestCellWithBox.XAxis)
            //{
            //    if (bombTimer == 0) 
            //    {
            //        yourBot.Bomb(yourBot.X, yourBot.Y);
            //        bombTimer = 7;
            //        continue;
            //    }

            //    yourBot.Move(xAxisToGetTo, yAxisToGetTo);
            //    continue;
            //}

            //// Action 3: Move to the the next best cell.
            //yourBot.Move(nearestCellWithBox.XAxis, nearestCellWithBox.YAxis);
            //xAxisToGetTo = nearestCellWithBox.XAxis;
            //yAxisToGetTo = nearestCellWithBox.YAxis;
            #endregion
        }
    }

    /*
     * Entities represent the things on the board other than boxes E.g players, bombs & items
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

            Owner ownerValue = Owner.EnemyBot;
            if (owner == yourId)
            {
                ownerValue = Owner.YourBot;
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

            // Read each cell in the row and add this to board
            var columns = row.ToCharArray();
            int x = 0;
            foreach (var column in columns)
            {
                // Determine if the cell is empty or has a box
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
    
    /* This method determines what to the entity should do next - Move to new location, bomb, or keep going */
    public void ExecuteAction(ref int xAxisToGetTo, ref int yAxisToGetTo, Cell nearestCellWithBox, ref int bombTimer)
    {
        if (Type != EntityType.Player)
        {
            return; 
        }

        // Action 1: if destination not reached, keep going!
        if (!(X == xAxisToGetTo && Y == yAxisToGetTo))
        {
            Console.Error.WriteLine("Keep moving.");
            Move(xAxisToGetTo, yAxisToGetTo);
            return;
        }

        // Action 2: if next to a box wait there. 
        if (X == nearestCellWithBox.XAxis && Y == nearestCellWithBox.YAxis)
        {
            if (bombTimer == 0)
            {
                Console.Error.WriteLine("Bomb");
                Bomb(X, Y);
                bombTimer = 7;
                return;
            }
            Console.Error.WriteLine("Keep moving..");
            Move(xAxisToGetTo, yAxisToGetTo);
            return;
        }

        // Action 3: Move to the the next best cell.
        Console.Error.WriteLine($"Move to: X: {nearestCellWithBox.XAxis} Y: {nearestCellWithBox.YAxis}");
        Move(nearestCellWithBox.XAxis, nearestCellWithBox.YAxis);
        xAxisToGetTo = nearestCellWithBox.XAxis;
        yAxisToGetTo = nearestCellWithBox.YAxis;
    }

    private void Bomb(int x, int y)
    {
        var stringX = x.ToString();
        var stringY = y.ToString();
        Console.WriteLine("BOMB " + stringX + " " + stringY);
    }

    private void Move(int x, int y)
    {
        var stringX = x.ToString();
        var stringY = y.ToString();
        Console.WriteLine("MOVE " + stringX + " " + stringY);
    }

    private static EntityType DetermineEntityType(int entityType)
    {
        if (entityType == 0)
        {
            return EntityType.Player;
        }
        if (entityType == 2)
        {
            return EntityType.Item;
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

    public Entity GetPlayerEntity(Owner owner)
    {
        var entity  = Entities.First(x => x.Type == EntityType.Player && x.Owner == owner);

        Console.Error.Write("Owner: " + entity.Owner);
        Console.Error.Write(" | x:" + entity.X);
        Console.Error.WriteLine(" | y:" + entity.Y);

        return entity;
    }

    public Cell GetNextMostOptimalCell(int currentYAxis, bool startingAtTop)
    {
        // Get all the boxes in current row.
        var nextYAxis = currentYAxis;
        var cellsWithBoxs = Cells.Where(x => x.YAxis == nextYAxis && x.CellState == CellState.Box);
        
        // Can't find any boxes in current row? Go to next row.
        while (!cellsWithBoxs.Any())
        {
            Console.Error.WriteLine("Go to next row");
            // If you start at the top left then you need to go down. If you start at bottom, you need to go up. TODO: Improve.
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

        // Find all the empty cells in the specified row and determine the score of each cell. 
        var emptyCells = Cells.Where(x => x.YAxis == nextYAxis && x.CellState == CellState.EmptyCell);
        DetermineScoreForCells(emptyCells);

        var cellWithHighestScore = emptyCells.OrderByDescending(x => x.Score).FirstOrDefault();

        Console.Error.WriteLine("Nearest axis with Box - X:" + cellWithHighestScore.XAxis + ", Y:" + cellWithHighestScore.YAxis);
        return cellWithHighestScore;
    }

    
    private void DetermineScoreForCells(IEnumerable<Cell> emptyCells)
    {
        foreach (var cell in emptyCells)
        {
            cell.GetRightNeighbour(Cells, Width, Height);
            cell.GetLeftNeighbour(Cells, Width, Height);
            cell.GetTopNeighbour(Cells, Width, Height);
            cell.GetBottomNeighbour(Cells, Width, Height);

            cell.ScoreCell();
            //Console.Error.WriteLine("x:" + cell.XAxis + " y:" + cell.YAxis + " Box score:" + cell.Score);
        }
    }

    private IEnumerable<Cell> CheckIfRowHasBoxes(int rowNumber)
    {
        return Cells.Where(x => x.YAxis == rowNumber && x.CellState == CellState.Box);
    }

    private void GetCellsWithItemsAndScoreThem()
    {
        List<Cell> cellsWithItems = new List<Cell>();

        var itemEntities = Entities.Where(e => e.Type == EntityType.Item);
        foreach (var item in itemEntities)
        {
            cellsWithItems.Add(Cells.First(c => c.XAxis == item.X && c.YAxis == item.Y));
        }
        
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
    private Cell RightNeighbour { get; set; }
    private Cell LeftNeighbour { get; set; }
    private Cell TopNeighbour { get; set; }
    private Cell BottomNeighbour { get; set; }

    public int Score { get; set; }

    public void GetRightNeighbour(IEnumerable<Cell> cells, int height, int width)
    {
        if (XAxis + 1 >= width)
        {
            RightNeighbour = null;
        }
        RightNeighbour =  cells.SingleOrDefault(x => x.XAxis == XAxis + 1 && x.YAxis == YAxis);
    }

    public void GetLeftNeighbour(IEnumerable<Cell> cells, int height, int width)
    {
        if (XAxis - 1 <= 0)
        {
            LeftNeighbour = null;
        }
        LeftNeighbour =  cells.SingleOrDefault(x => x.XAxis == XAxis - 1 && x.YAxis == YAxis);
    }
    public void GetTopNeighbour(IEnumerable<Cell> cells, int height, int width)
    {
        if (YAxis + 1 >= height)
        {
            TopNeighbour = null;
        }
        TopNeighbour = cells.SingleOrDefault(c => c.YAxis == YAxis + 1 && c.XAxis == XAxis);
    }

    public void GetBottomNeighbour(IEnumerable<Cell> cells, int height, int width)
    {
        if (YAxis - 1 <= 0)
        {
            TopNeighbour = null;
        }
        TopNeighbour = cells.SingleOrDefault(c => c.YAxis == YAxis - 1 && c.XAxis == XAxis);
    }

    /*
     * Scores for cell is allocated as follows:
     *      - For each box that can be destroyed by placing a bomb in the given cell +1 is given.
     *      - So if a two boxes can be destroyed by placing bomb in cellA then cellA will have score of 2.
     */
    public void ScoreCell()
    {
        if (RightNeighbour?.CellState == CellState.Box)
        {
            Score += 1;
        }
        if (LeftNeighbour?.CellState == CellState.Box)
        {
            Score += 1;
        }
        if (TopNeighbour?.CellState == CellState.Box)
        {
            Score += 1;
        }
        if (BottomNeighbour?.CellState == CellState.Box)
        {
            Score += 1;
        }
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
    EnemyBot,
    YourBot
}

public enum CellState{
    EmptyCell,
    Box
}
