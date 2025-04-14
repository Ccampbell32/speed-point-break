using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;
    public int startX, startY;
    MazeCell[,] maze;
    Vector2Int currentCell;

    public MazeCell[,] GenerateMaze() //Renamed for clarity
    {
        //Clear the maze data before generating a new one
        maze = new MazeCell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        //Ensure start position is within bounds
        startX = Mathf.Clamp(startX, 0, mazeWidth - 1);
        startY = Mathf.Clamp(startY, 0, mazeHeight - 1);

        CarvePath(startX, startY);
        return maze;
    }

    List<Direction> directions = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    List<Direction> GetRandomDirections()
    {
        List<Direction> dir = new List<Direction>(directions);
        List<Direction> rndDir = new List<Direction>();
        while (dir.Count > 0)
        {
            int rnd = Random.Range(0, dir.Count);
            rndDir.Add(dir[rnd]);
            dir.RemoveAt(rnd);
        }
        return rndDir;
    }

    bool IsCellValid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < mazeWidth && y < mazeHeight && !maze[x, y].visited;
    }

    Vector2Int CheckNeighbours()
    {
        List<Direction> rndDir = GetRandomDirections();
        Vector2Int neighbour = currentCell;

        for (int i = 0; i < rndDir.Count; i++)
        {
            switch (rndDir[i])
            {
                case Direction.Up: neighbour.y++; break;
                case Direction.Down: neighbour.y--; break;
                case Direction.Right: neighbour.x++; break;
                case Direction.Left: neighbour.x--; break;
            }
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }
        return currentCell; //No valid neighbors
    }

    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        int xDiff = primaryCell.x - secondaryCell.x;
        int yDiff = primaryCell.y - secondaryCell.y;

        if (xDiff == 1) maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        else if (xDiff == -1) maze[primaryCell.x, primaryCell.y].leftWall = false;
        else if (yDiff == 1) maze[secondaryCell.x, secondaryCell.y].topWall = false;
        else if (yDiff == -1) maze[primaryCell.x, primaryCell.y].topWall = false;
    }

    void CarvePath(int x, int y)
    {
        currentCell = new Vector2Int(x, y);
        List<Vector2Int> path = new List<Vector2Int>();
        bool deadEnd = false;

        while (!deadEnd)
        {
            Vector2Int nextCell = CheckNeighbours();
            if (nextCell == currentCell) //Dead end
            {
                if (path.Count > 0)
                {
                    currentCell = path[path.Count - 1];
                    path.RemoveAt(path.Count - 1);
                }
                else
                {
                    deadEnd = true;
                }
            }
            else
            {
                BreakWalls(currentCell, nextCell);
                maze[currentCell.x, currentCell.y].visited = true;
                currentCell = nextCell;
                path.Add(currentCell);
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class MazeCell
    {
        public bool visited;
        public int x, y;
        public bool topWall;
        public bool leftWall;
        public Vector2Int position => new Vector2Int(x, y);

        public MazeCell(int x, int y)
        {
            this.x = x;
            this.y = y;
            visited = false;
            topWall = leftWall = true;
        }
    }
}
