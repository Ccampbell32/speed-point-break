using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int mazeWidth = 10;
    public int mazeHeight = 10;
    public MazeCell[,] maze;
    public enum Direction { Up, Down, Left, Right }
    [Range(0f, 1f)]
    public float loopProbability = 0.1f; // Probability of creating a loop
    [Range(0f, 1f)]
    public float wallRemovalProbability = 0.05f; // Probability of removing a wall
    public bool isMazeReady = false;

    void Awake()
    {
        GenerateMaze(); // Generate the maze only once in Awake()
    }

    // Remove the duplicate GenerateMaze() call from Start()

    Vector2Int DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return new Vector2Int(0, 1);
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }

    List<Direction> GetUnvisitedNeighbors(int x, int y)
    {
        List<Direction> neighbors = new List<Direction>();
        if (x > 0 && !maze[x - 1, y].visited) neighbors.Add(Direction.Left);
        if (x < mazeWidth - 1 && !maze[x + 1, y].visited) neighbors.Add(Direction.Right);
        if (y > 0 && !maze[x, y - 1].visited) neighbors.Add(Direction.Down);
        if (y < mazeHeight - 1 && !maze[x, y + 1].visited) neighbors.Add(Direction.Up);
        return neighbors;
    }

    List<Direction> GetAllNeighbors(int x, int y)
    {
        List<Direction> neighbors = new List<Direction>();
        if (x > 0) neighbors.Add(Direction.Left);
        if (x < mazeWidth - 1) neighbors.Add(Direction.Right);
        if (y > 0) neighbors.Add(Direction.Down);
        if (y < mazeHeight - 1) neighbors.Add(Direction.Up);
        return neighbors;
    }

    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        int xDiff = primaryCell.x - secondaryCell.x;
        int yDiff = primaryCell.y - secondaryCell.y;

        if (xDiff == 1)
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
            maze[secondaryCell.x, secondaryCell.y].rightWall = false;
        }
        else if (xDiff == -1)
        {
            maze[primaryCell.x, primaryCell.y].rightWall = false;
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (yDiff == 1)
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
            maze[secondaryCell.x, secondaryCell.y].bottomWall = false;
        }
        else if (yDiff == -1)
        {
            maze[primaryCell.x, primaryCell.y].bottomWall = false;
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }
    }

    void RemoveRandomWalls()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                if (Random.value < wallRemovalProbability)
                {
                    int wallIndex = Random.Range(0, 4);
                    switch (wallIndex)
                    {
                        case 0: maze[x, y].topWall = false; break;
                        case 1: maze[x, y].bottomWall = false; break;
                        case 2: maze[x, y].leftWall = false; break;
                        case 3: maze[x, y].rightWall = false; break;
                    }
                }
            }
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
            List<Direction> allNeighbors = GetAllNeighbors(current.x, current.y);

            if (unvisitedNeighbors.Count > 0)
            {
                Direction randomNeighbor = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                Vector2Int next = current + DirectionToVector(randomNeighbor);
                BreakWalls(current, next);
                maze[next.x, next.y].visited = true;
                stack.Push(next);
            }
            else if (allNeighbors.Count > 0 && Random.value < loopProbability)
            {
                //Attempt to create a loop
                Direction randomNeighbor = allNeighbors[Random.Range(0, allNeighbors.Count)];
                Vector2Int next = current + DirectionToVector(randomNeighbor);
                if (next.x >= 0 && next.x < mazeWidth && next.y >= 0 && next.y < mazeHeight)
                {
                    BreakWalls(current, next);
                }
            }
            else
            {
                stack.Pop();
            }
        }
        //Random wall removal
        RemoveRandomWalls();
    }

    public MazeCell[,] GenerateMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = new MazeCell();
            }
        }

        CarvePath(0, 0);
        isMazeReady = true;
        return maze;
    }

    [System.Serializable]
    public class MazeCell
    {
        public bool visited;
        public bool topWall = true;
        public bool bottomWall = true;
        public bool leftWall = true;
        public bool rightWall = true;
    }
}

