using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 500)]
    public int mazeWidth = 5, mazeHeight = 5;
    public int startX, startY;
    MazeCell[,] maze;
    Vector2Int currentCell;

    public MazeCell[,] GenerateMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell(x, y);
            }
        }

        startX = Mathf.Clamp(startX, 0, mazeWidth - 1);
        startY = Mathf.Clamp(startY, 0, mazeHeight - 1);

        CarvePath(startX, startY);

        // Ensure outer walls are closed
        for (int x = 0; x < mazeWidth; x++)
        {
            maze[x, 0].topWall = true;
            maze[x, mazeHeight - 1].topWall = true;
        }
        for (int y = 0; y < mazeHeight; y++)
        {
            maze[0, y].leftWall = true;
            maze[mazeWidth - 1, y].leftWall = true;
        }
        
        Debug.Log("Final Maze Wall States:");
        for (int y = 0; y < mazeHeight; y++)
        {
            string row = "";
            for (int x = 0; x < mazeWidth; x++)
            {
                row += $"({x},{y}): T{maze[x, y].topWall}, B{maze[x, y].bottomWall}, L{maze[x, y].leftWall}, R{maze[x, y].rightWall}  ";
            }
            Debug.Log(row);
        }

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
        return currentCell;
    }

    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        int xDiff = primaryCell.x - secondaryCell.x;
        int yDiff = primaryCell.y - secondaryCell.y;

        Debug.Log($"Breaking walls between ({primaryCell.x}, {primaryCell.y}) and ({secondaryCell.x}, {secondaryCell.y})");

        if (xDiff == 1)
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
            maze[secondaryCell.x, secondaryCell.y].rightWall = false;
            Debug.Log($"Left wall of ({primaryCell.x}, {primaryCell.y}) and right wall of ({secondaryCell.x}, {secondaryCell.y}) broken.");
        }
        else if (xDiff == -1)
        {
            maze[primaryCell.x, primaryCell.y].rightWall = false;
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
            Debug.Log($"Right wall of ({primaryCell.x}, {primaryCell.y}) and left wall of ({secondaryCell.x}, {secondaryCell.y}) broken.");
        }
        else if (yDiff == 1)
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
            maze[secondaryCell.x, secondaryCell.y].bottomWall = false;
            Debug.Log($"Top wall of ({primaryCell.x}, {primaryCell.y}) and bottom wall of ({secondaryCell.x}, {secondaryCell.y}) broken.");
        }
        else if (yDiff == -1)
        {
            maze[primaryCell.x, primaryCell.y].bottomWall = false;
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
            Debug.Log($"Bottom wall of ({primaryCell.x}, {primaryCell.y}) and top wall of ({secondaryCell.x}, {secondaryCell.y}) broken.");
        }
    }


    void CarvePath(int x, int y)
    {
        maze[x, y].visited = true;
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(x, y));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Direction> unvisitedNeighbors = GetUnvisitedNeighbors(current.x, current.y);

            if (unvisitedNeighbors.Count > 0)
            {
                Direction randomNeighbor = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                Vector2Int next = current + DirectionToVector(randomNeighbor);
                BreakWalls(current, next);
                maze[next.x, next.y].visited = true;
                stack.Push(next);
                Debug.Log($"Current: {current}, Next: {next}");
            }
            else
            {
                stack.Pop();
            }
        }
    }

    List<Direction> GetUnvisitedNeighbors(int x, int y)
    {
        List<Direction> neighbors = new List<Direction>();
        if (IsCellValid(x, y + 1)) neighbors.Add(Direction.Up);
        if (IsCellValid(x, y - 1)) neighbors.Add(Direction.Down);
        if (IsCellValid(x + 1, y)) neighbors.Add(Direction.Right);
        if (IsCellValid(x - 1, y)) neighbors.Add(Direction.Left);
        return neighbors;
    }

    Vector2Int DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return new Vector2Int(0, 1);
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Right: return new Vector2Int(1, 0);
            case Direction.Left: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }
  
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}